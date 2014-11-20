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

	private bool isActive= false;

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
	}

	public IEnumerator getSongData(){
		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Email", LoginModel.UserEmail);
		headers.Add("X-User-Token", LoginModel.AuthKey);
		WWW mp3stream = new WWW (RoomURL, null, headers);
		
		yield return mp3stream;
		var mp3parsed = JSON.Parse(mp3stream.text); 
		Debug.Log(mp3parsed);

		int counter = 0;
		foreach(JSONNode data in mp3parsed["data"].AsArray){
			Debug.Log(counter);
			Debug.Log(data.ToString());
			AudioList[counter] = new AudioModel("A","B","C",data["current_song"].ToString().Trim('"'), data["_id"]["$oid"], 0);

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
		//printAudioList(AudioList);
		SuccessfulLoad = true;
	}

	private IEnumerator getSongMeta(int index){

		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Email", LoginModel.UserEmail);
		headers.Add("X-User-Token", LoginModel.AuthKey);
		//Test acquisition of song meta
		string current_id = ElevatorMenu.CurrentRoom.RoomId;
		Debug.Log("Oid of first room:" + current_id);
		
		string song_str = "http://beforeheaveniqp.herokuapp.com/api/rooms/"+current_id+"/current_song";
		Debug.Log(song_str);
		WWW song_url = new WWW(song_str, null, headers);
		yield return song_url;
		var song_parsed = JSON.Parse(song_url.text);
		Debug.Log(song_parsed);

		AudioList[index].Elapsed_time = int.Parse(song_parsed["data"]["elapsed_time"].ToString().Trim('"'));
		Debug.Log("AudioList[" + index +"] Elapsed Time: " + AudioList[index].Elapsed_time);

		Debug.Log("DEBUG AudioList[" + index + "]: " + AudioList[index].Elapsed_time);
		current_song = AudioList[index];
		Debug.Log("Current Song Elapsed Time: " + current_song.Elapsed_time);
	}

	void printAudioList(AudioModel[] List){
		foreach(AudioModel am in List){
			Debug.Log(am.ToString());
		}
	}

	// Use this for initialization
	void Start () {
		isActive = true;
		current_song = new AudioModel("Empty", "Empty", "Empty", "Empty", "Empty", 0);
	}

	void Awake(){
		AudioList = new AudioModel[50];
	}

	void OnJoinedLobby(){
		StartCoroutine(getSongData());
		Debug.Log("AudioController : Changed Rooms");

		if(SuccessfulLoad){
			int i = 0;
			//Debug.Log("DEBUG: " + AudioList[0].ToString());
			while(!AudioList[i].Room_id.Equals(ElevatorMenu.CurrentRoom.RoomId)){
				if(i > AudioList.Length){
					break;
				}
				i++;
			}
			Debug.Log("AudioList ID" + AudioList[i].Room_id + ":" + ElevatorMenu.CurrentRoom.RoomId);
			StartCoroutine(getSongMeta(i));
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(LoginController.SuccessfulLogin && isActive){
			//AudioList = new AudioModel[20];
			//StartCoroutine(getSongData());
			isActive = false;
		}
	}
}
