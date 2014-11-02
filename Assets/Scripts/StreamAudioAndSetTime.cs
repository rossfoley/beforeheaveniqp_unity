using UnityEngine;
using System.Collections;
using System.IO;

using NAudio;
using NAudio.Wave;

public class StreamAudioAndSetTime : MonoBehaviour {
	private IWavePlayer nWaveOutDevice;
	private WaveStream nMainOutputStream;
	private WaveChannel32 nVolumeStream;

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
	// Use this for initialization
	void Start () {
		LoadAudio();
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
