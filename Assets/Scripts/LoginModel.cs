﻿using UnityEngine;
using System.Collections;

public class LoginModel : MonoBehaviour {

	private static string userEmail;
	private static string authKey;
	private static string userId;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// UserEmail getter, is static so it can be grabbed from anywhere 
	public static string UserEmail {
		get {
			Debug.Log("Model returning as userEmail: " + userEmail);
			return userEmail;
		}
		set {
			userEmail = value;
			Debug.Log ("Model set userEmail as: " + userEmail);
		}
	}
	
	// AuthKey getter, is static so it can be grabbed from anywhere
	public static string AuthKey {
		get {
			return authKey;
		}
		set {
			authKey = value;
		}
	}
	
	// UserId getter, is static so it can be grabbed from anywhere
	public static string UserId {
		get {
			return userId;
		}
		set {
			userId = value;
		}
	}
}
