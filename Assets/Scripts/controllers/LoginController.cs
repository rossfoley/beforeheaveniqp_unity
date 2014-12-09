using UnityEngine;
using System.Collections;
using SimpleJSON;

public class LoginController : MonoBehaviour {

	private static string username = "";
	private static string userPassword = "";
	private static string authKey;
	private static string userId;
	private const string loginURL = "http://beforeheaveniqp.herokuapp.com/api/users/login";
	private static bool successfulLogin = false;
	private static int loginStatus = 0;

	// Logs the user in using what the user typed into the username and password text fields
	public static IEnumerator login(string usernameInput, string userPasswordInput){
		username = usernameInput;
		userPassword = userPasswordInput;
		if (username == "") {
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
		Debug.Log (username);
		loginForm.AddField ("username", username);
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

				// Sets the values of the login model
				LoginModel.Username = username;
				LoginModel.AuthKey = authKey;
				LoginModel.UserId = userId;
				updateOnlineStatus(true);
			}
			// If the login was unsuccessful, display the error message
			else {
				// TODO Error Messages
				Debug.Log("Invalid login credentials");
			}
		}
	}

	// Gets friends and stores the data in the LoginModel
	public static void getFriends(){
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/users/" + LoginModel.UserId +"/get_friends/") as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		
		request.Method = "GET";
		
		request.ContentType = "application/json";
		request.Headers.Add("X-User-Username", LoginModel.Username);
		request.Headers.Add("X-User-Token", LoginModel.AuthKey);
		request.ContentLength = 0;
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}
		var parsed = JSON.Parse (responseContent);
		LoginModel.FriendData = new UserData[parsed ["data"].AsArray.Count];
		int i = 0;
		string username = "";
		string currentRoomId = "";
		string userId = "";
		foreach(JSONNode data in (parsed ["data"]).AsArray){
			username = data ["username"];
			currentRoomId = data ["current_room_id"];
			userId = data ["_id"] ["$oid"];
			UserData ud = new UserData(userId, currentRoomId, username);
			LoginModel.FriendData[i] = ud;
			i++;
		}
	}
	
	// Adds the given friend username to the list of friends of the current user
	public static void addFriend(string friendUsername){
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/users/" + LoginModel.UserId +"/add_friend/") as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		
		request.Method = "PUT";
		
		request.ContentType = "application/json";
		request.Headers.Add("x-user-username", LoginModel.Username);
		request.Headers.Add("x-user-token", LoginModel.AuthKey);
		
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{\"new_friend_username\": \"" + friendUsername + "\"}");
		request.ContentLength = byteArray.Length;
		using (var writer = request.GetRequestStream()){writer.Write(byteArray, 0, byteArray.Length);}
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}
	}

	public static void removeFriend(string friendUsername){
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/users/" + LoginModel.UserId +"/remove_friend/") as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		
		request.Method = "PUT";
		
		request.ContentType = "application/json";
		request.Headers.Add("x-user-username", LoginModel.Username);
		request.Headers.Add("x-user-token", LoginModel.AuthKey);
		
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{\"new_friend_username\": \"" + friendUsername + "\"}");
		request.ContentLength = byteArray.Length;
		using (var writer = request.GetRequestStream()){writer.Write(byteArray, 0, byteArray.Length);}
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}
	}

	public static void updateCurrentRoom(){

		LoginModel.CurrentRoomId = RoomModel.getInstance ().CurrentRoom.RoomId;

		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/users/" + LoginModel.UserId + "/current_room/") as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		
		request.Method = "PUT";
		
		request.ContentType = "application/json";
		request.Headers.Add("x-user-username", LoginModel.Username);
		request.Headers.Add("x-user-token", LoginModel.AuthKey);
		
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{ \"room_id\": \"" + LoginModel.CurrentRoomId + "\"}");
		request.ContentLength = byteArray.Length;
		using (var writer = request.GetRequestStream()){writer.Write(byteArray, 0, byteArray.Length);}
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}
	}

	public static string getCurrentRoomOfUser(string userId){
		var request = System.Net.WebRequest.Create ("http://beforeheaveniqp.herokuapp.com/api/users/" + userId + "/current_room/") as System.Net.HttpWebRequest;
		request.KeepAlive = true;
	
		request.Method = "GET";
		
		request.ContentType = "application/json";
		request.Headers.Add ("x-user-username", LoginModel.Username);
		request.Headers.Add ("x-user-token", LoginModel.AuthKey);
		request.ContentLength = 0;
		string responseContent = null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
					responseContent = reader.ReadToEnd ();
			}
		}
		var parsed = JSON.Parse (responseContent);
		return parsed ["data"] ["name"];
	}

	public static bool isUserOnline(string userId){
		var request = System.Net.WebRequest.Create ("http://beforeheaveniqp.herokuapp.com/api/users/" + userId + "/is_online/") as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		
		request.Method = "GET";
		
		request.ContentType = "application/json";
		request.Headers.Add ("x-user-username", LoginModel.Username);
		request.Headers.Add ("x-user-token", LoginModel.AuthKey);
		request.ContentLength = 0;
		string responseContent = null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd ();
			}
		}
		var parsed = JSON.Parse (responseContent);
		return parsed ["data"].AsBool;
	}

	public static void updateOnlineStatus(bool status){
		var request = System.Net.WebRequest.Create ("http://beforeheaveniqp.herokuapp.com/api/users/" + LoginModel.UserId + "/is_online/") as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		
		request.Method = "PUT";
		
		request.ContentType = "application/json";
		request.Headers.Add ("x-user-username", LoginModel.Username);
		request.Headers.Add ("x-user-token", LoginModel.AuthKey);
		request.ContentLength = 0;
		string responseContent = null;
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{ \"is_online\": \"" + status + "\"}");
		request.ContentLength = byteArray.Length;
		using (var writer = request.GetRequestStream()) {
			writer.Write(byteArray, 0, byteArray.Length);
		}
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
