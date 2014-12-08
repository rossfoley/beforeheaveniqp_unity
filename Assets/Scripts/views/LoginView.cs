using UnityEngine;
using System.Collections;
using System.Net;
using SimpleJSON;

public class LoginView : MonoBehaviour {

	public GameObject networkManager;
	public GameObject startingRoom;
	public GameObject roomController;
	public GameObject chat;
	public GameObject guiController;

	public Texture2D loginBackground;

	string userEmail = "";
	string userPassword = "";
	string friendEmail = "";
	bool loggedIn = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		// If the user has not logged in yet, display the GUI elements of the login screen
		if (!LoginController.SuccessfulLogin){

			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), loginBackground);

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
				StartCoroutine(LoginController.login (userEmail, userPassword));
			}
			// Used for debug so logging can be done quickly
			if (GUI.Button (new Rect(0, 60, 50, 20), "Bypass")){
				userEmail = "nathan@abc.com";
				userPassword = "nintendo";
				StartCoroutine(LoginController.login(userEmail, userPassword));
			}

			if (GUI.Button (new Rect(0, 80, 50, 20), "Register")){
				Application.OpenURL("http://beforeheaveniqp.herokuapp.com/users/sign_up");
			}

			switch(LoginController.LoginStatus){
			case 3: 
				// If the user does not enter a password, display the error message
				GUI.Label (new Rect (120, 40, 150, 20), "Enter a password");
				break;
			case 2:
				// If the user does not enter a email, display the error message
				GUI.Label (new Rect (120, 40, 150, 20), "Enter an email");
				break;
			case 1:
				// If the user is currently loggin in, display the logging in message
				GUI.Label (new Rect (120, 40, 150, 20), "Logging in...");
				break;
			case -1:
				// If an error with login occurred, display the error message
				GUI.Label (new Rect(120, 40, 150, 20), "Invalid credentials");
				break;
			}
			GUI.EndGroup ();
		}
		else if (!loggedIn && LoginController.SuccessfulLogin){
			// After a successful login, activate the networkManager
			startingRoom.SetActive(true);
			chat.SetActive(true);
			networkManager.SetActive(true);
			roomController.SetActive(true);
			guiController.SetActive(true);
			loggedIn = true;
		}
	}
}
