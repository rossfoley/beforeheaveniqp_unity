﻿using UnityEngine;
using System.Collections;

public class UserData : MonoBehaviour {

	private string userId;
	private string currentRoomId;
	private string userEmail;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public UserData(string userId, string currentRoomId, string userEmail){
		this.userId = userId;
		this.currentRoomId = currentRoomId;
		this.userEmail = userEmail;
	}

	public string UserId {
		get {
			return this.userId;
		}
		set {
			userId = value;
		}
	}

	public string CurrentRoomId {
		get {
			return this.currentRoomId;
		}
		set {
			currentRoomId = value;
		}
	}

	public string UserEmail {
		get {
			return this.userEmail;
		}
		set {
			userEmail = value;
		}
	}
}
