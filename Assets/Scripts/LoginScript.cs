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
	private const string loginURL = "http://beforeheaveniqp.herokuapp.com/api/users/login";
	private bool successfulLogin = false;
	private int loginStatus = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		// If the user has not logged in yet, display the GUI elements of the login screen
		if (!successfulLogin){
			GUI.BeginGroup (new Rect (50, 50, 500, 500));

			// Email label
			GUI.Label (new Rect (0, 0, 100, 20), "Email: ");

			// Email text field
			userEmail = GUI.TextField (new Rect(110, 0, 200, 20), userEmail);

			// Password label
			GUI.Label (new Rect (0, 20, 100, 20), "Password: ");

			// Password text field
			userPassword = GUI.PasswordField (new Rect(110, 20, 200, 20), userPassword, '*');

			// If the login button is clicked, check for input and then call the login() function if the user 
			// has inputted both an email and password
			if (GUI.Button (new Rect (0, 40, 50, 20), "Login")) {
				if (userEmail.Trim () == "" || userPassword.Trim () == ""){
					// TODO Error message
					Debug.Log ("No user email or password inputted");
				}
				else {
					StartCoroutine(login ());
				}
			}
			// Used for debug so logging can be done quickly
			if (GUI.Button (new Rect(0, 60, 50, 20), "Bypass")){
				userEmail = "awhan@wpi.edu";
				userPassword = "hiandy257";
				StartCoroutine(login ());
			}
			switch(loginStatus){
			case 1:
				// If the user is currently loggin in, display the logging in message
				GUI.Label (new Rect (120, 40, 150, 20), "Logging in...");
				break;
			case -1:
				// If an error with login occurred, display the error message
				GUI.Label (new Rect(120, 40, 150, 20), "Error");
				break;
			}
			GUI.EndGroup ();
		}
	}

	// Logs the user in using what the user typed into the email and password text fields
	IEnumerator login(){
		loginStatus = 1;
		WWW login;
		// Login to get an authentication token
		WWWForm loginForm = new WWWForm ();
		loginForm.AddField ("email", userEmail);
		loginForm.AddField ("password", userPassword);
		login = new WWW(loginURL, loginForm);
		yield return login;
		if (!string.IsNullOrEmpty(login.error)) {
			//TODO login.error returns the string of the error, so catch the different types of errors and do different error messages
			//with the switch statement in OnGUI()
			Debug.Log ("login error");
			loginStatus = -1;
		}
		// If the login was successful, grab the authentication key and display the starting room, elevator menu, and start the network manager
		else{
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
			// If the login was unsuccessful, display the error message
			else {
				// TODO Error Messages
				Debug.Log("Invalid login credentials");
			}
		}
	}

	// UserEmail getter, is static so it can be grabbed from anywhere 
	public static string UserEmail {
		get {
			return userEmail;
		}
	}

	// AuthKey getter, is static so it can be grabbed from anywhere
	public static string AuthKey {
		get {
			return authKey;
		}
	}

	// UserId getter, is static so it can be grabbed from anywhere
	public static string UserId {
		get {
			return userId;
		}
	}
}
