using UnityEngine;
using System.Collections;

using SimpleJSON;

public sealed class AudiosController : MonoBehaviour {
	static AudiosController instance = null;
	private static AudioModel[] AudioList;
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

	public IEnumerator getSongData(){
		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Email", LoginModel.UserEmail);
		headers.Add("X-User-Token", LoginModel.AuthKey);
		WWW mp3stream = new WWW (RoomURL, null, headers);
		
		yield return mp3stream;
		var mp3parsed = JSON.Parse(mp3stream.text); 
		Debug.Log(mp3parsed);

		/*
		WWW song_meta = new WWW (sampleRoom, null, headers);
		yield return song_meta;
		var song_meta_parsed = JSON.Parse(song_meta.text); 
		Debug.Log(song_meta_parsed);
		*/

		int counter = 0;
		foreach(JSONNode data in mp3parsed["data"].AsArray){
			Debug.Log(counter);
			Debug.Log(data.ToString());
			AudioList[counter] = new AudioModel("A","B","C",data["current_song"].ToString().Trim('"'), data["_id"]["$oid"], 0);
			Debug.Log(AudioList[counter].Url + AudioList[counter].Room_id);
			counter++;
		}
		//printAudioList(AudioList);
		SuccessfulLoad = true;
	}

	void printAudioList(AudioModel[] List){
		foreach(AudioModel am in List){
			Debug.Log(am.ToString());
		}
	}

	// Use this for initialization
	void Start () {
		isActive = true;
	}

	void Awake(){
		AudioList = new AudioModel[50];
	}
	
	// Update is called once per frame
	void Update () {
		if(LoginController.SuccessfulLogin && isActive){
			//AudioList = new AudioModel[20];
			StartCoroutine(getSongData());
			isActive = false;
		}
	}
}
