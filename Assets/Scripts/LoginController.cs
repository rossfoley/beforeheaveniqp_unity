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
		if (userEmail == "") {
			loginStatus = 2;
			return true;
		}
		if (userPassword == "") {
			loginStatus = 3;
			return true;
		}
		loginStatus = 1;
		WWW login;
		// Login to get an authentication token
		WWWForm loginForm = new WWWForm ();
		loginForm.AddField ("email", userEmail);
		loginForm.AddField ("password", userPassword);
		login = new WWW(loginURL, loginForm);
		yield return login;
		Debug.Log (login.text);
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

				// Construct the array that contains all of the friend IDs
				string[] friends = new string[parsed ["data"] ["friend_ids"].AsArray.Count];
				Debug.Log ("Size of friends = " + parsed ["data"] ["friend_ids"].AsArray.Count);
				string friendId;
				int i = 0;
				foreach(JSONNode data in (parsed ["data"] ["friend_ids"]).AsArray){
					friends[i] = data["$oid"];
					i++;
				}
				// Sets the values of the login model
				LoginModel.UserEmail = userEmail;
				LoginModel.AuthKey = authKey;
				LoginModel.UserId = userId;
				LoginModel.FriendIds = friends;
				Debug.Log("Friend 0 = " + LoginModel.FriendIds[0]);
			}
			// If the login was unsuccessful, display the error message
			else {
				// TODO Error Messages
				Debug.Log("Invalid login credentials");
			}
		}
	}

	// TODO Finish the getFriends function
	public static void getFriends(){
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/user/" + LoginModel.UserId +"/get_friend/") as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		
		request.Method = "GET";
		
		request.ContentType = "application/json";
		request.Headers.Add("x-user-email", LoginModel.UserEmail);
		request.Headers.Add("x-user-token", LoginModel.AuthKey);
		request.ContentLength = 0;
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}

		var parsed = JSON.Parse (responseContent);
	}

	// Adds the given friend email to the list of friends of the current user
	public static void addFriend(string friendEmail){
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/users/" + LoginModel.UserId +"/add_friend/") as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		
		request.Method = "PUT";
		
		request.ContentType = "application/json";
		request.Headers.Add("x-user-email", LoginModel.UserEmail);
		request.Headers.Add("x-user-token", LoginModel.AuthKey);
		
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{\"new_friend_email\": \"" + friendEmail + "\"}");
		request.ContentLength = byteArray.Length;
		using (var writer = request.GetRequestStream()){writer.Write(byteArray, 0, byteArray.Length);}
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
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
