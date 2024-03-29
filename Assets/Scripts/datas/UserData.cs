﻿using UnityEngine;
using System.Collections;

public class UserData{

	private string userId;
	private string currentRoomId;
	private string username;
	private bool isOnline;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public UserData(string userId, string currentRoomId, string userEmail){
		this.userId = userId;
		this.currentRoomId = currentRoomId;
		this.username = userEmail;
		isOnline = false;
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

	public string Username {
		get {
			return this.username;
		}
		set {
			username = value;
		}
	}

	public bool IsOnline {
		get {
			return this.isOnline;
		}
		set {
			isOnline = value;
		}
	}
}
