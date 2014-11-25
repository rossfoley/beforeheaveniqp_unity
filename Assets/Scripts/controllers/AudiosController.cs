using UnityEngine;
using System.Collections;
using System.Runtime;

using SimpleJSON;

public sealed class AudiosController : MonoBehaviour {
	static AudiosController instance = null;
	private static AudioModel[] AudioList;
	private static AudioModel current_song;

	static string RoomURL = "http://beforeheaveniqp.herokuapp.com/api/rooms";
	static string sampleRoom = "";
	public static bool SuccessfulLoad = false;
	public static bool SongMetaLoaded = false;

	public static string currentSongURL = "";

	private bool isActive= false;
	private static bool callGetSongMeta = false;
	private static int songMetaIndex = -1;

	public static AudiosController getInstance(){
		if(instance == null){
			instance = new AudiosController();
		}
		return instance;
	}

	public AudioModel[] audioList {
		get {
			return AudioList;
		}
	}

	public AudioModel Current_song {
		get {
			return current_song;
		}
		set {
			current_song = value;
		}
	}

	public static bool Successful_Load {
		get {
			return SuccessfulLoad;
		}
		set{
			SuccessfulLoad = value;
		}
	}

	public static bool SongMeta_Load {
		get {
			return SongMetaLoaded;
		}
		set{
			SongMetaLoaded = value;
		}
	}

	public static IEnumerator getSongData(){
		Hashtable headers = new Hashtable();
		Debug.Log ("CurrentRoomID = " + RoomModel.getInstance().CurrentRoom.RoomId);
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Email", LoginModel.UserEmail);
		headers.Add("X-User-Token", LoginModel.AuthKey);

		WWW allSongs = new WWW (RoomURL, null, headers);

		Debug.Log ("Waiting on allSongs");

		yield return allSongs;

		var allSongsParsed = JSON.Parse (allSongs.text);

		int counter = 0;

		Debug.Log ("CurrentSongURL = " + currentSongURL);

		foreach(JSONNode data in allSongsParsed["data"].AsArray){
			Debug.Log(counter);
			Debug.Log("Current song url = " + data["current_song"].ToString());
			AudioList[counter] = new AudioModel("A","B","C", data["current_song"].ToString().Trim('"'), data["_id"]["$oid"], 0, 0);

			currentSongURL = data["current_song"];

			if(AudioList[counter].Url == null){
				AudioList[counter].Url = "";
			}
		
			if(AudioList[counter].Room_id == null){
				AudioList[counter].Room_id = "0";
			}
		
			Debug.Log(AudioList[counter].Url);
			Debug.Log(AudioList[counter].Room_id);
			counter++;
		}
		SuccessfulLoad = true;

		int i = 0;
		Debug.Log("DEBUG: " + AudioList[0].ToString());
		while(!AudioList[i].Room_id.Equals(RoomModel.getInstance().CurrentRoom.RoomId)){
			if(i > AudioList.Length){
				break;
			}
			i++;
		}
		Debug.Log("AudioList ID" + AudioList[i].Room_id + ":" + RoomModel.getInstance ().CurrentRoom.RoomId);
		callGetSongMeta = true;
		songMetaIndex = i;

		//StartCoroutine(getSongMeta(i));
	}
	
	public static IEnumerator getSongMeta(int index){

		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Email", LoginModel.UserEmail);
		headers.Add("X-User-Token", LoginModel.AuthKey);
		//Test acquisition of song meta
		string current_id = RoomModel.getInstance().CurrentRoom.RoomId;
		Debug.Log("Oid of first room:" + current_id);
		
		string song_str = "http://beforeheaveniqp.herokuapp.com/api/rooms/"+current_id+"/current_song";
		Debug.Log(song_str);
		WWW song_url = new WWW(song_str, null, headers);
		yield return song_url;
		var song_parsed = JSON.Parse(song_url.text);
		Debug.Log(song_parsed);

		AudioList[index].Elapsed_time = int.Parse(song_parsed["data"]["elapsed_time"].ToString().Trim('"'));
		Debug.Log("AudioList[" + index +"] Elapsed Time: " + AudioList[index].Elapsed_time);

		currentSongURL = song_parsed["data"]["song"]["stream_url"];
		Debug.Log ("Current song url from song meta = " + currentSongURL);

		Debug.Log("DEBUG AudioList[" + index + "]: " + AudioList[index].Elapsed_time);
		current_song = AudioList[index];

		current_song.Duration = song_parsed["data"]["song"]["duration"].AsInt;

		Debug.Log("Current Song Elapsed Time: " + current_song.Elapsed_time);
		SongMetaLoaded = true;
	}

	void printAudioList(AudioModel[] List){
		foreach(AudioModel am in List){
			Debug.Log(am.ToString());
		}
	}

	// Use this for initialization
	void Start () {
		isActive = true;
		current_song = new AudioModel("Empty", "Empty", "Empty", "Empty", "Empty", 0, 0);
	}

	void Awake(){
		AudioList = new AudioModel[50];
	}

	void OnJoinedRoom(){
		StartCoroutine(getSongData());
		Debug.Log("AudioController : Changed Rooms");
		
		if(SuccessfulLoad){
			int i = 0;
			Debug.Log("DEBUG: " + AudioList[0].ToString());
			while(!AudioList[i].Room_id.Equals(RoomModel.getInstance().CurrentRoom.RoomId)){
				if(i > AudioList.Length){
					break;
				}
				i++;
			}
			//current_song = AudioList[i];
			Debug.Log("AudioList ID" + AudioList[i].Room_id + ":" + RoomModel.getInstance().CurrentRoom.RoomId);
			Debug.Log("Current Song: " + current_song.Url);
			StartCoroutine(getSongMeta(i));
		}
	}

	void OnJoinedLobby(){
		currentSongURL = "";
		SongMetaLoaded = false;
	}

	// Update is called once per frame
	void Update () {
		if(LoginController.SuccessfulLogin && isActive){
			//AudioList = new AudioModel[20];
			//StartCoroutine(getSongData());
			isActive = false;
		}
		if (callGetSongMeta){
			callGetSongMeta = false;
			StartCoroutine(getSongMeta(songMetaIndex));
		}
	}
}
