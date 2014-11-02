using UnityEngine;
using System.Collections;
using System.IO;

using NAudio;
using NAudio.Wave;

public class StreamAudioAndSetTime : MonoBehaviour {
	private IWavePlayer nWaveOutDevice;
	private WaveStream nMainOutputStream;
	private WaveChannel32 nVolumeStream;

	public string o_username;
	public string o_password;

	private bool LoadAudioFromData(byte[] data){
		try{
			MemoryStream tmpStr = new MemoryStream(data);
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

	private void LoadAudio(){
		WWW www = new WWW("https://ia802508.us.archive.org/5/items/testmp3testfile/mpthreetest.mp3");
		while(!www.isDone);
		Debug.Log("URL Found");

		byte[] imageData = www.bytes;
		if(!LoadAudioFromData(imageData)){
			Debug.LogError("Couldn't load Audio bytes");
		}

		nWaveOutDevice.Play();
		Resources.UnloadUnusedAssets();
	}

	public void LoginUser(string username, string enteredPassword){
		WWWForm form = new WWWForm();
		form.AddField("username", username);
		form.AddField("password", enteredPassword);
		byte[] rawData = form.data;

		Hashtable headers = new Hashtable();
		headers = form.headers;

		string url = "http://beforeheaveniqp.herokuapp.com/";

		headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("username:password"));
		WWW www = new WWW(url, form);

		StartCoroutine(WaitForRequest(www));
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
		LoginUser(o_username, o_password);
		//LoadAudio();
		/*
		WWW www = new WWW ("http://localhost:8888/test2.ogg");  // start a download of the given URL
		AudioClip clip  = www.GetAudioClip(false, true); // 2D, streaming
		
		while(!clip.isReadyToPlay)
		{
			Debug.Log("Waiting");
		}
		
		audio.clip = clip;
		audio.Play();
		audio.time = 5; // skip to 5 seconds in
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
