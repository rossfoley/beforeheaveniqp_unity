using UnityEngine;
using System.Collections;
using System.Net;
using SimpleJSON;

public class LoginScript : MonoBehaviour {

	public GameObject networkManager;
	public GameObject elevator;
	public GameObject startingRoom;
	private static string userEmail = "";
	private string userPassword = "";
	private static string authKey;
	private static string userId;
	private const string loginURL = "http://beforeheaveniqp.herokuapp.com/api/user/login";
	private bool successfulLogin = false;
	private bool loggingIn = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (!successfulLogin){
			GUI.BeginGroup (new Rect (50, 50, 500, 500));
			GUI.Label (new Rect (0, 0, 100, 20), "Email: ");
			userEmail = GUI.TextField (new Rect(110, 0, 200, 20), userEmail);
			GUI.Label (new Rect (0, 20, 100, 20), "Password: ");
			userPassword = GUI.PasswordField (new Rect(110, 20, 200, 20), userPassword, '*');
			if (GUI.Button (new Rect (0, 40, 50, 20), "Login")) {
				if (userEmail.Trim () == "" || userPassword.Trim () == ""){
					// TODO Error message
					Debug.Log ("No user email or password inputted");
				}
				else {
					StartCoroutine(login ());
				}
			}
			if (GUI.Button (new Rect(0, 60, 50, 20), "Bypass")){
				userEmail = "awhan@wpi.edu";
				userPassword = "hiandy257";
				StartCoroutine(login ());
			}
			if (loggingIn){
				GUI.Label (new Rect (120, 40, 150, 20), "Logging in...");
			}
			GUI.EndGroup ();
		}
	}

	IEnumerator login(){
		loggingIn = true;
		WWW login;
		// Login to get an authentication token
		WWWForm loginForm = new WWWForm ();
		loginForm.AddField ("email", userEmail);
		loginForm.AddField ("password", userPassword);
		login = new WWW(loginURL, loginForm);
		yield return login;
		var parsed = JSON.Parse (login.text);

		if ((parsed["status"]).ToString ().Trim('"') == "ok"){
			successfulLogin = true;
			authKey = (parsed ["data"] ["authentication_token"]).ToString ().Trim ('"');
			userId = (parsed ["data"] ["_id"] ["$oid"]).ToString ().Trim ('"');
			// After a successful login, activate the networkManager
			startingRoom.SetActive(true);
			elevator.SetActive(true);
			networkManager.SetActive(true);
		}
		else {
			// TODO Error Messages
			Debug.Log("Invalid login credentials");
		}
	}

	public static string UserEmail {
		get {
			return userEmail;
		}
	}

	public static string AuthKey {
		get {
			return authKey;
		}
	}

	public static string UserId {
		get {
			return userId;
		}
	}
}
