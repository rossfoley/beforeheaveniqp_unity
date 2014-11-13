using UnityEngine;
using System.Collections;
using SimpleJSON;

public class LoginController : MonoBehaviour {

	private static string userEmail = "";
	private static string userPassword = "";
	private static string authKey;
	private static string userId;
	private const string loginURL = "http://beforeheaveniqp.herokuapp.com/api/users/login";
	private static bool successfulLogin = false;
	private static int loginStatus = 0;

	// Logs the user in using what the user typed into the email and password text fields
	public static IEnumerator login(string userEmailInput, string userPasswordInput){
		userEmail = userEmailInput;
		userPassword = userPasswordInput;
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

				// Sets the values of the login model
				LoginModel.UserEmail = userEmail;
				LoginModel.AuthKey = authKey;
				LoginModel.UserId = userId;
			}
			// If the login was unsuccessful, display the error message
			else {
				// TODO Error Messages
				Debug.Log("Invalid login credentials");
			}
		}
	}

	public static bool SuccessfulLogin {
		get {
			return successfulLogin;
		}
		set {
			successfulLogin = value;
		}
	}

	public static int LoginStatus {
		get {
			return loginStatus;
		}
		set {
			loginStatus = value;
		}
	}
}
