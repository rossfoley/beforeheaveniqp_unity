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
	private static string currentSongName = "";

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

	public static string CurrentSongName {
		get {
			return currentSongName;
		}
		set {
			currentSongName = value;
		}
	}

	public static IEnumerator getSongData(){
		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Username", LoginModel.Username);
		headers.Add("X-User-Token", LoginModel.AuthKey);

		WWW allSongs = new WWW (RoomURL, null, headers);

		Debug.Log ("Waiting on allSongs");

		yield return allSongs;

		var allSongsParsed = JSON.Parse (allSongs.text);

		int counter = 0;

		Debug.Log ("CurrentSongURL = " + currentSongURL);

		foreach(JSONNode data in allSongsParsed["data"].AsArray){
			AudioList[counter] = new AudioModel("A","B","C", data["current_song"].ToString().Trim('"'), data["_id"]["$oid"], 0, 0);

			currentSongURL = data["current_song"];

			if(AudioList[counter].Url == null){
				AudioList[counter].Url = "";
			}
		
			if(AudioList[counter].Room_id == null){
				AudioList[counter].Room_id = "0";
			}
		
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
		callGetSongMeta = true;
		songMetaIndex = i;
	}
	
	public static IEnumerator getSongMeta(int index){

		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Username", LoginModel.Username);
		headers.Add("X-User-Token", LoginModel.AuthKey);
		//Test acquisition of song meta
		string current_id = RoomModel.getInstance().CurrentRoom.RoomId;

		string song_str = "http://beforeheaveniqp.herokuapp.com/api/rooms/"+current_id+"/current_song";
		WWW song_url = new WWW(song_str, null, headers);
		yield return song_url;
		var song_parsed = JSON.Parse(song_url.text);

		AudioList[index].Elapsed_time = int.Parse(song_parsed["data"]["elapsed_time"].ToString().Trim('"'));

		currentSongURL = song_parsed["data"]["song"]["stream_url"];

		current_song = AudioList[index];

		current_song.Duration = song_parsed["data"]["song"]["duration"].AsInt;
		
		currentSongName = song_parsed["data"]["song"]["title"];

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

		if(SuccessfulLoad){
			int i = 0;
			while(!AudioList[i].Room_id.Equals(RoomModel.getInstance().CurrentRoom.RoomId)){
				if(i > AudioList.Length){
					break;
				}
				i++;
			}
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
			isActive = false;
		}
		if (callGetSongMeta){
			callGetSongMeta = false;
			StartCoroutine(getSongMeta(songMetaIndex));
		}
	}

	public static int SongMetaIndex {
		get {
			return songMetaIndex;
		}
		set {
			songMetaIndex = value;
		}
	}
}

