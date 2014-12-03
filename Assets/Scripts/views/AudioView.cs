using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;

using NAudio;
using NAudio.Wave;
using SimpleJSON;

public class AudioView : MonoBehaviour {
	private IWavePlayer nWaveOutDevice;
	private WaveStream nMainOutputStream;
	private WaveChannel32 nVolumeStream;
	private MemoryStream tmpStr;

	private int duration;

	AudiosController ac;

	public Texture2D soundcloud_icon;
	string[] mp3link = new string[32];

	bool isMuted = false; float temp_vol = 0.0f;
	bool isPlaying = false;
	bool isActive= false;
	bool getSongs = false;

	private bool LoadAudioFromData(byte[] data){
		try{
			tmpStr = new MemoryStream(data);
			nMainOutputStream = new Mp3FileReader(tmpStr);
			nVolumeStream = new WaveChannel32(nMainOutputStream);

			nWaveOutDevice = new WaveOut();
			nWaveOutDevice.Init(nVolumeStream);
			nMainOutputStream.Seek(ac.Current_song.Elapsed_time * 100, SeekOrigin.Begin);

			duration = AudiosController.getInstance().Current_song.Duration - ac.Current_song.Elapsed_time;

			Debug.Log("Duration = " + duration);

			StartCoroutine(loadNextSong());

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

	private IEnumerator loadNextSong(){
		string currentRoomId = RoomModel.getInstance().CurrentRoom.RoomId;
		int waitTime = (duration/1000) + 1;

		yield return new WaitForSeconds(waitTime);
		if (currentRoomId == RoomModel.getInstance ().CurrentRoom.RoomId){
			Debug.Log ("Finished waiting for song to end");
			AudiosController.SongMeta_Load = false;
			isActive = true;
			AudiosController.currentSongURL = "";
			getSongs = true;

			//Stop previous song
			nMainOutputStream.Dispose();
			nVolumeStream.Dispose();
			tmpStr.Dispose();
			nWaveOutDevice.Dispose();
		}
	}
	
	void Start(){
		ac = AudiosController.getInstance();
		isActive = true;
	}

	// Update is called once per frame
	void Update () {
		if (getSongs){
			getSongs = false;
			StartCoroutine(AudiosController.getSongData());
		}
		if(AudiosController.SongMeta_Load && isActive){
			Debug.Log ("Calling update since meta load and isActive are both true");
			if(AudiosController.currentSongURL != null && AudiosController.currentSongURL != ""){

				//Load current song
				Debug.Log("Current Song is not Null, here's proof: " + AudiosController.currentSongURL);
				LoadAudio(AudiosController.currentSongURL + "?client_id=0cb45a6052596ee086177b11b29e8809");
				Debug.Log("Current song Elapsed Time(2): " + ac.Current_song.Elapsed_time);
				nWaveOutDevice.Play();
				isActive = false;
			}
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


			if(GUI.Button(new Rect(120, Screen.height - (Screen.height / 8) + 20, 50, 50), "Mute")){
				if(!isMuted){
					temp_vol = nVolumeStream.Volume;
					nVolumeStream.Volume = 0.0f;
					isMuted = true;
				}else{
					nVolumeStream.Volume = temp_vol;
					isMuted = false;
				}

			}

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

	void OnJoinedRoom(){
		isPlaying = false;
		isMuted = false;
		isActive = true;
		if(nMainOutputStream != null){
			//Stop previous song
			nMainOutputStream.Dispose();
			nVolumeStream.Dispose();
			tmpStr.Dispose();
			nWaveOutDevice.Dispose();
		}
		if(AudiosController.Successful_Load){

		}
	}

	void OnDisconnectedFromPhoton(){
		Debug.Log ("Disconnected from photon called");
		nMainOutputStream.Dispose ();
		nVolumeStream.Dispose ();
		tmpStr.Dispose ();
		nWaveOutDevice.Dispose ();
	}
}