using UnityEngine;
using System.Collections;

public class LoginModel : MonoBehaviour {

	private static string username;
	private static string authKey;
	private static string userId;
	private static string[] friendIds;
	private static UserData[] friendData;
	private static string currentRoomId;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// UserEmail getter, is static so it can be grabbed from anywhere 
	public static string Username {
		get {
			return username;
		}
		set {
			username = value;
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

	public static UserData[] FriendData {
		get {
			return friendData;
		}
		set {
			friendData = value;
		}
	}

	public static string CurrentRoomId {
		get {
			return currentRoomId;
		}
		set {
			currentRoomId = value;
		}
	}
}
