using UnityEngine;
using System.Collections;

public class LoginModel : MonoBehaviour {

	private static string userEmail = "";
	private static string authKey;
	private static string userId;
	private static string[] friendIds;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// UserEmail getter, is static so it can be grabbed from anywhere 
	public static string UserEmail {
		get {
			return userEmail;
		}
		set {
			userEmail = value;
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

	public static string[] FriendIds {
		get {
			return friendIds;
		}
		set {
			friendIds = value;
		}
	}
}
