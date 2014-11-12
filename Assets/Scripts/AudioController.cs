using UnityEngine;
using System.Collections;

using SimpleJSON;

public class AudioController : MonoBehaviour {
	static AudioModel[] AudioList = new AudioModel[20];
	static string RoomURL = "http://beforeheaveniqp.herokuapp.com/api/rooms";

	public IEnumerator getSongData(){
		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Email", LoginModel.UserEmail);
		headers.Add("X-User-Token", LoginModel.AuthKey);
		WWW mp3stream = new WWW (roomURL, null, headers);
		
		yield return mp3stream;
		var mp3parsed = JSON.Parse(mp3stream.text); 
		Debug.Log(mp3parsed);
		
		WWW song_meta = new WWW (sampleRoom, null, headers);
		yield return song_meta;
		var song_meta_parsed = JSON.Parse(song_meta.text); 
		Debug.Log(song_meta_parsed);
		
		int counter = 0;
		foreach(JSONNode data in mp3parsed["data"].AsArray){
			Debug.Log(data.ToString());
			AudioList[counter].Url = data["current_song"].ToString().Trim('"');
			counter++;
		}
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(getSongData());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
