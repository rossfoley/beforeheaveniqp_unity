using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		WWW www = new WWW ("http://localhost:8888/test2.ogg");  // start a download of the given URL
		AudioClip clip  = www.GetAudioClip(false, true); // 2D, streaming
		
		while(!clip.isReadyToPlay)
		{
			Debug.Log("Waiting");
		}
		
		audio.clip = clip;
		audio.Play();
		audio.time = 5; // skip to 5 seconds in
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
