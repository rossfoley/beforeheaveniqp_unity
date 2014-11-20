using UnityEngine;
using System.Collections;
using System.IO;

using NAudio;
using NAudio.Wave;
using SimpleJSON;

public class AudioView : MonoBehaviour {
	private IWavePlayer nWaveOutDevice;
	private WaveStream nMainOutputStream;
	private WaveChannel32 nVolumeStream;
	private MemoryStream tmpStr;

	AudiosController ac;

	public Texture2D soundcloud_icon;
	string[] mp3link = new string[32];

	bool isPlaying = false;
	bool isActive= false;

	private bool LoadAudioFromData(byte[] data){
		try{
			tmpStr = new MemoryStream(data);
			nMainOutputStream = new Mp3FileReader(tmpStr);
			nVolumeStream = new WaveChannel32(nMainOutputStream);

			nWaveOutDevice = new WaveOut();
			nWaveOutDevice.Init(nVolumeStream);
			nMainOutputStream.Seek(ac.Current_song.Elapsed_time * 100, SeekOrigin.Begin);

			return true;
			}
		catch(System.Exception ex){
			Debug.LogWarning("Error! " + ex.Message);
		}
		return false;
	}

	private void LoadAudio(string input_url){
		WWW www = new WWW(input_url);
		while(!www.isDone);
		Debug.Log("URL Found");

		byte[] imageData = www.bytes;

		if(!LoadAudioFromData(imageData)){
			Debug.LogError("Couldn't load Audio bytes");
		}

		Resources.UnloadUnusedAssets();
	}

	void Start(){
		ac = AudiosController.getInstance();
		isActive = true;
	}

	// Update is called once per frame
	void Update () {
		if(AudiosController.SongMeta_Load && isActive){
			if(ac.Current_song.Url != null){

				//Load current song
				Debug.Log("Current Song is not Null, here's proof: " + AudiosController.currentSongURL);
				LoadAudio(AudiosController.currentSongURL + "?client_id=0cb45a6052596ee086177b11b29e8809");
				Debug.Log("Current song Elapsed Time(2): " + ac.Current_song.Elapsed_time);

				nWaveOutDevice.Play();
			}
			isActive = false;
		}
	}

	void OnGUI(){
		/*
		ElevatorMenu em = gameObject.GetComponent<ElevatorMenu>();
		RoomData rd = em.getCurrentRoom();
		*/
		if(LoginController.SuccessfulLogin){
			GUI.Box(new Rect(10, Screen.height - (Screen.height / 8), Screen.width - 20, Screen.height / 8), "");
			GUI.Label(new Rect(20, Screen.height - (Screen.height / 8), 100, 100), soundcloud_icon);
			GUI.Label(new Rect(120, Screen.height - (Screen.height / 8), Screen.width - 10, 50), new GUIContent("Current Song: " + mp3link[0]));

			/*
			if(GUI.Button(new Rect(120, Screen.height - (Screen.height / 8) + 20, 50, 50), "Play")){
				if(!isPlaying){
					nWaveOutDevice.Play();
					isPlaying = !isPlaying;
				}

			}

			if(GUI.Button(new Rect(175, Screen.height - (Screen.height / 8) + 20, 50, 50), "Pause")){
				if(isPlaying){
					nWaveOutDevice.Pause();
					isPlaying = !isPlaying;
				}

			}
			*/
			if(GUI.Button(new Rect(230, Screen.height - (Screen.height / 8) + 20, 50, 50), "V++")){
				if(nVolumeStream.Volume >= 1.0f){
					nVolumeStream.Volume = 1.0f;
				}
				nVolumeStream.Volume += 0.1f;
				Debug.Log(nVolumeStream.Volume);
			}

			if(GUI.Button(new Rect(285, Screen.height - (Screen.height / 8) + 20, 50, 50), "V--")){
				if(nVolumeStream.Volume <= 0.0f){
					nVolumeStream.Volume = 0.0f;
				}
				nVolumeStream.Volume -= 0.1f;
				Debug.Log(nVolumeStream.Volume);
			}
		}
	}

	void OnJoinedLobby(){
		isPlaying = false;
		isActive = true;
		if(AudiosController.Successful_Load){
			if(nMainOutputStream != null){
				//Stop previous song
				nMainOutputStream.Close();
				nVolumeStream.Close();
				tmpStr.Close();
				nWaveOutDevice.Stop();
			}
		}
	}

	void OnDisconnectedFromPhoton(){
		Debug.Log ("Disconnected from photon called");
		nMainOutputStream.Close ();
		nVolumeStream.Close ();
		tmpStr.Close ();
		nWaveOutDevice.Stop ();
	}
}
