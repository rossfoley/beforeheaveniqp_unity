using UnityEngine;
using System.Collections;
using System.IO;

using NAudio;
using NAudio.Wave;
using SimpleJSON;

public class StreamAudioAndSetTime : MonoBehaviour {
	private IWavePlayer nWaveOutDevice;
	private WaveStream nMainOutputStream;
	private WaveChannel32 nVolumeStream;
	private MemoryStream tmpStr;

	public string o_username;
	public string o_password;

	private bool LoadAudioFromData(byte[] data){
		try{
			tmpStr = new MemoryStream(data);
			nMainOutputStream = new Mp3FileReader(tmpStr);
			nVolumeStream = new WaveChannel32(nMainOutputStream);

			nWaveOutDevice = new WaveOut();
			nWaveOutDevice.Init(nVolumeStream);

			return true;
			}
		catch(System.Exception ex){
			Debug.LogWarning("Error! " + ex.Message);
		}
		return false;
	}

	private void DebugLoadAudio(){
		//WWW www = new WWW("https://ia802508.us.archive.org/5/items/testmp3testfile/mpthreetest.mp3");
		WWW www = new WWW("https://ec-media.soundcloud.com/g4Xt6e05MIcR.128.mp3?f10880d39085a94a0418a7ef69b03d522cd6dfee9399eeb9a52200996ffdb9349ddb954ba046f08712a18829fd04ed9574d7111478fe44bdfdfa96d2f7f5267555de5d6e28&AWSAccessKeyId=AKIAJNIGGLK7XA7YZSNQ&Expires=1415057882&Signature=0I%2F1rQNJwi7KZ0xyYbHb4Md%2B4CM%3D");
		while(!www.isDone);
		Debug.Log("URL Found");

		byte[] imageData = www.bytes;
		if(!LoadAudioFromData(imageData)){
			Debug.LogError("Couldn't load Audio bytes");
		}

		nWaveOutDevice.Play();
		Resources.UnloadUnusedAssets();
	}

	private void LoadAudio(string input_url){
		//WWW www = new WWW("https://api.soundcloud.com/tracks/174468803/stream?client_id=0cb45a6052596ee086177b11b29e8809");
		WWW www = new WWW(input_url);
		while(!www.isDone);
		Debug.Log("URL Found");
		
		byte[] imageData = www.bytes;
		if(!LoadAudioFromData(imageData)){
			Debug.LogError("Couldn't load Audio bytes");
		}
		
		nWaveOutDevice.Play();
		Resources.UnloadUnusedAssets();
	}

	IEnumerator LoginUser(string username, string enteredPassword){
		string url = "http://beforeheaveniqp.herokuapp.com/api/user/login";
		string roomsUrl = "http://beforeheaveniqp.herokuapp.com/api/rooms";
		string songUrl = "http://beforeheaveniqp.herokuapp.com/api/rooms/:room_id/current_song";

		WWWForm form = new WWWForm();
		form.AddField("email", username);
		form.AddField("password", enteredPassword);

		WWW login = new WWW(url, form);

		yield return login;
		var parsed = JSON.Parse(login.text); Debug.Log(parsed);

		//headers = form.headers;

		string userEmail = (parsed ["data"] ["email"]).ToString().Trim('"');
		string userAuthKey = (parsed ["data"] ["authentication_token"]).ToString ().Trim ('"');
		Debug.Log(userEmail + "" + userAuthKey);

		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Email", userEmail);
		headers.Add("X-User-Token", userAuthKey);
		WWW mp3stream = new WWW (roomsUrl, null, headers);

		yield return mp3stream;

		var mp3parsed = JSON.Parse(mp3stream.text); Debug.Log(mp3parsed);

		string[] mp3link = new string[16];
		int counter = 0;
		foreach(JSONNode data in mp3parsed["data"].AsArray){
			Debug.Log(data.ToString());
			mp3link[counter] = data["current_song"].ToString();
			counter++;
		}
		Debug.Log(mp3link[0]);
		LoadAudio(mp3link[0].ToString().Trim('"'));
	}

	IEnumerator WaitForRequest(WWW www){
		yield return www;
		Debug.Log(www.text);
		// check for errors
		if (www.error == null){
			Debug.Log("WWW Ok!: " + www.text);
			Debug.Log("WWW Ok!: " + www.data);
		} else {
			Debug.Log("WWW Error: " + www.error);
		}
	} 

	// Use this for initialization
	void Start () {
		o_username = "butts@ss.com";
		o_password = "butts123";
		StartCoroutine(LoginUser(o_username, o_password));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	void OnDisconnectedFromPhoton(){
		Debug.Log ("Disconnected from photon called");
		nMainOutputStream.Close ();
		nVolumeStream.Close ();
		tmpStr.Close ();
		nWaveOutDevice.Stop ();
	}
}
