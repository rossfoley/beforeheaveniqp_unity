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

	private int duration = 0;

	AudiosController ac;

	public Texture2D soundcloud_icon;
	string[] mp3link = new string[32];

	bool isMuted = false; float temp_vol = 0.0f;
	bool isPlaying = false;
	bool isActive= false;
	bool getSongs = false;

	//Variables for controlling the Audio progress bar
	Vector2 bar_pos = new Vector2(Screen.width / 2, Screen.height - (Screen.height / 8) + 20); 
	Vector2 bar_size = new Vector2((Screen.width/ 2) - 80, 20);
	float bar_tick= 0.0f;

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

	private void LoadAudio(){
		string input_url = AudiosController.currentSongURL + "?client_id=0cb45a6052596ee086177b11b29e8809"; 

		StartCoroutine(LoadAudioWWW(input_url));
	}

	private IEnumerator LoadAudioWWW(string input_url){
		WWW www = new WWW(input_url);
		while(!www.isDone){
			yield return www;
		}
		Debug.Log("URL Found");
		
		byte[] imageData = www.bytes;
		
		if(!LoadAudioFromData(imageData)){
			Debug.LogError("Couldn't load Audio bytes");
		} 

		nWaveOutDevice.Play();
		Resources.UnloadUnusedAssets();

		int previousSongPosition = ac.Current_song.Elapsed_time;
		StartCoroutine(AudiosController.getSongMeta(AudiosController.SongMetaIndex));
		int currentSongPosition = ac.Current_song.Elapsed_time;

		duration = duration - (currentSongPosition - previousSongPosition);

		nMainOutputStream.Seek(currentSongPosition * 100, SeekOrigin.Begin);
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
			//Debug.Log ("Calling update since meta load and isActive are both true");
			if(AudiosController.currentSongURL != null && AudiosController.currentSongURL != ""){
				bar_tick = 0.0f;
				//Load current song
				LoadAudio();

				//Set bar position to be the beginning of the song with respect to the bar's width
				bar_tick = ac.Current_song.Elapsed_time / (bar_size.x * 1000);
				Debug.Log("Bar Ticker Offset is: " + bar_tick);
				isActive = false;
			}
		}
		if(AudiosController.SongMeta_Load){
			//Increase the bar by how much time has passed multiplied by the ration of the bar width over the song's duration
			//bar_tick += 0.01f;
			bar_tick += ((bar_size.x * Time.deltaTime) / ac.Current_song.Duration);
			Debug.Log(Time.deltaTime + " : " + bar_tick);
		}
	}


	void OnGUI(){

		if(LoginController.SuccessfulLogin){
			GUI.Box(new Rect(10, Screen.height - (Screen.height / 8), Screen.width - 20, Screen.height / 8), "");
			GUI.Label(new Rect(20, Screen.height - (Screen.height / 8), 100, 100), soundcloud_icon);
			GUI.Label(new Rect(120, Screen.height - (Screen.height / 8), Screen.width - 10, 50), new GUIContent("Current Song: " + AudiosController.CurrentSongName));
			GUI.Label(new Rect((Screen.width / 4) + 20, Screen.height - (Screen.height / 8), Screen.width - 10, 50), new GUIContent("Genre: " + ac.Current_song.Genre));
			GUI.Label(new Rect((Screen.width * 2)/ 4, Screen.height - (Screen.height / 8), Screen.width - 10, 50), new GUIContent("Duration: " + ac.Current_song.Duration/1000.0f));

			// draw the background:
			GUI.BeginGroup (new Rect (bar_pos.x, bar_pos.y, bar_size.x, bar_size.y));
			GUI.Box (new Rect (0,0, bar_size.x, bar_size.y), new GUIContent(""));
				
				// draw the filled-in part:
				GUI.BeginGroup (new Rect (0, 0, bar_size.x * bar_tick, bar_size.y));
					GUI.Box (new Rect (0,0, bar_size.x, bar_size.y), new GUIContent(""));
				GUI.EndGroup ();
			
			GUI.EndGroup ();

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
		//Debug.Log ("Disconnected from photon called");
		nMainOutputStream.Dispose ();
		nVolumeStream.Dispose ();
		tmpStr.Dispose ();
		nWaveOutDevice.Dispose ();
	}
}