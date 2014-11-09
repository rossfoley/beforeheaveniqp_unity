﻿using UnityEngine;
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
	public Texture2D soundcloud_icon;
	string[] mp3link = new string[16];

	bool isPlaying = false;

	private bool LoadAudioFromData(byte[] data){
		try{
			tmpStr = new MemoryStream(data);
			nMainOutputStream = new Mp3FileReader(tmpStr);
			nVolumeStream = new WaveChannel32(nMainOutputStream);

			nWaveOutDevice = new WaveOut();
			nWaveOutDevice.Init(nVolumeStream);
			//nMainOutputStream.Seek(2000000,SeekOrigin.Begin);

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
		
		//
		Resources.UnloadUnusedAssets();
	}

	IEnumerator LoginUser(string username, string enteredPassword){
		string url = "http://beforeheaveniqp.herokuapp.com/api/user/login";
		string roomsUrl = "http://beforeheaveniqp.herokuapp.com/api/rooms";

		WWWForm form = new WWWForm();
		form.AddField("email", username);
		form.AddField("password", enteredPassword);
		WWW login = new WWW(url, form);

		yield return login;
		var parsed = JSON.Parse(login.text); Debug.Log(parsed);

		string userEmail = (parsed ["data"] ["email"]).ToString().Trim('"');
		string userAuthKey = (parsed ["data"] ["authentication_token"]).ToString ().Trim ('"');
		//Debug.Log(userEmail + "" + userAuthKey);

		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");
		headers.Add("X-User-Email", userEmail);
		headers.Add("X-User-Token", userAuthKey);
		WWW mp3stream = new WWW (roomsUrl, null, headers);

		yield return mp3stream;

		var mp3parsed = JSON.Parse(mp3stream.text); 
		Debug.Log(mp3parsed);

		int counter = 0;
		foreach(JSONNode data in mp3parsed["data"].AsArray){
			Debug.Log(data.ToString());
			mp3link[counter] = data["current_song"].ToString().Trim('"');
			counter++;
		}

		LoadAudio(mp3link[0]);
		Debug.Log(mp3link[0]);
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

	void OnGUI(){
		/*
		ElevatorMenu em = gameObject.GetComponent<ElevatorMenu>();
		RoomData rd = em.getCurrentRoom();
		*/

		GUI.Box(new Rect(10, Screen.height - (Screen.height / 8), Screen.width - 20, Screen.height / 8), "");
		GUI.Label(new Rect(20, Screen.height - (Screen.height / 8), 100, 100), soundcloud_icon);
		GUI.Label(new Rect(120, Screen.height - (Screen.height / 8), Screen.width - 10, 50), new GUIContent("Current Song: " + mp3link[0]));
		if(GUI.Button(new Rect(120, Screen.height - (Screen.height / 8) + 20, 50, 50), "Play")){
			if(!isPlaying){
				nWaveOutDevice.Play();
			}
			isPlaying = !isPlaying;
		}

		if(GUI.Button(new Rect(175, Screen.height - (Screen.height / 8) + 20, 50, 50), "Pause")){
			if(isPlaying){
				nWaveOutDevice.Pause();
			}
			isPlaying = !isPlaying;
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
	
	void OnDisconnectedFromPhoton(){
		Debug.Log ("Disconnected from photon called");
		nMainOutputStream.Close ();
		nVolumeStream.Close ();
		tmpStr.Close ();
		nWaveOutDevice.Stop ();
	}
}
