using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Text;
using System.Net;

public class RoomController : MonoBehaviour {

	private static RoomController instance = null;
	private string username;
	private string userAuthKey;
	private string userId;

	public enum roomPresets {
		defaultRoom = 0,
		jazzRoom = 1,
	}
	
	private bool isChangingRoom;

	public static RoomController getInstance(){
		if(instance == null){
			instance = new RoomController();
		}
		return instance;
	}

	//Constant URLs
	private const string roomsURL = "http://beforeheaveniqp.herokuapp.com/api/rooms";
	private const string roomSearchURL = "http://beforeheaveniqp.herokuapp.com/api/rooms/search/";

	// Use this for initialization
	void Start () {
		instance = this;

		// Grabs the user's login information from LoginModel
		username = LoginModel.Username;
		userAuthKey = LoginModel.AuthKey;
		userId = LoginModel.UserId;

		Debug.Log ("Username = " + LoginModel.Username);
		Debug.Log ("AuthKey = " + LoginModel.AuthKey);

		// Boolean used to check if it is in the process of switching rooms
		isChangingRoom = false;
		
		// Retrieve all the rooms currently on the database
		StartCoroutine(getRooms (""));
		getAllPlaylists();
	}

	// Gets all the rooms from the database
	// string searchTerm - The search term that is passed to the database, if it is an empty string,
	// all the rooms are returned
	public IEnumerator getRooms(string searchTerm){
		// Set up the request
		Hashtable headers = new Hashtable();
		headers.Add ("Content-Type", "application/json");
		headers.Add("X-User-Username", username);
		headers.Add("X-User-Token", userAuthKey);
		WWW rooms;
		// If the search term is empty, grab all the rooms on server
		if(searchTerm == ""){
			rooms = new WWW(roomsURL, null, headers);
			yield return rooms;
		}
		// If there is a search term, send the request to the search URL with the search term
		else{
			rooms = new WWW(roomSearchURL + searchTerm, null, headers);
			yield return rooms;
		}

		//TODO Eventually remove, but handy for debugging in the mean-time. Prints text of all the rooms and their members
		Debug.Log (rooms.text);

		Debug.Log (rooms.error);
		var roomsParsed = JSON.Parse (rooms.text);
		// Set AllRooms to what was returned from the request
		RoomModel.getInstance().AllRooms = new RoomData[roomsParsed["data"].AsArray.Count];
		int roomCount = 0;
		foreach(JSONNode data in roomsParsed["data"].AsArray){
			string[] memberIds = new string[data["member_ids"].AsArray.Count];
			int i = 0;
			foreach(JSONNode members in data["member_ids"].AsArray){
				memberIds[i] = members["$oid"];
				i++;
			}
			// Build the roomData and place it in the allRooms array
			RoomData roomData;
			if (data["playlist"].ToString () != "\"null\""){
				roomData = new RoomData(data["_id"]["$oid"], data["name"].ToString(), data["genre"].ToString(), data["playlist"]["id"].ToString (), data["visits"].AsInt, memberIds, data["unity_data"].AsInt);
			}
			else {
				roomData = new RoomData(data["_id"]["$oid"], data["name"].ToString(), data["genre"].ToString(), "", data["visits"].AsInt, memberIds, data["unity_data"].AsInt);
			}
			RoomModel.getInstance().AllRooms[roomCount] = roomData;
			roomCount++;
		}
	}

	public IEnumerator createRoom(string newRoomName, string newRoomGenre, int roomPreset){
		RoomConfigMenu.CreateRoomStatus = 1; //Creating
		// Set up the request
		WWWForm roomCreateForm = new WWWForm();
		var newRoomData = new Hashtable();
		newRoomData.Add ("name", newRoomName);
		newRoomData.Add ("genre", newRoomGenre);
		var headers = new Hashtable();
		headers.Add ("Content-Type", "application/json");
		headers.Add ("X-User-Username", username);
		headers.Add ("X-User-Token", userAuthKey);
		
		// TODO
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{\"room_data\": {\"name\": \"" + newRoomName + "\",\"genre\": \"" + newRoomGenre + "\",\"unity_data\": \"" + roomPreset + "\"} }");

		WWW newRoomRequest = new WWW(roomsURL, byteArray, headers);
		yield return newRoomRequest;
		if (!string.IsNullOrEmpty(newRoomRequest.error)) {
			//TODO login.error returns the string of the error, so catch the different types of errors and do different error messages
			//with the switch statement in OnGUI()
			Debug.Log ("room creation error");
			RoomConfigMenu.CreateRoomStatus = -1;

		}
		else {
			// After the room is created, hide the create room window and reset the related variables.
			newRoomName = "";
			newRoomGenre = "";
			RoomConfigMenu.CreateRoomStatus = 2;
		}
	}

	public void deleteRoom(){
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/rooms/" + RoomModel.getInstance().CurrentRoom.RoomId) as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		
		request.Method = "DELETE";
		
		request.ContentType = "application/json";
		request.Headers.Add("x-user-Username", username);
		request.Headers.Add("x-user-token", userAuthKey);
		request.ContentLength = 0;
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}
		GUIController.CrWindowVisible = false;

		NetworkManager.getInstance().kickAll();
	}

	public void addBandMember(string roomId, string newMemberUsername) {
		RoomConfigMenu.UpdateRoomStatus = 1;
		
		// Create the put request for adding the new band member
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/rooms/" + roomId + "/add_band_member/") as System.Net.HttpWebRequest;
		
		request.KeepAlive = true;
		
		request.Method = "PUT";
		
		request.ContentType = "application/json";
		request.Headers.Add("x-user-username", username);
		request.Headers.Add("x-user-token", userAuthKey);
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{\"new_member_username\": \"" + newMemberUsername + "\"}");
		request.ContentLength = byteArray.Length;
		using (var writer = request.GetRequestStream()){
			writer.Write(byteArray, 0, byteArray.Length);
		}
		string responseContent=null;
		try {
			using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
				using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
					responseContent = reader.ReadToEnd();
					RoomConfigMenu.UpdateRoomStatus = 2;
				}
			}
		}
		// If a WebException is caught, display an error message
		catch(WebException e){
			//TODO Error message
			Debug.Log ("Invalid username entered");
			RoomConfigMenu.UpdateRoomStatus = -2;
		}
	}

	public void updateRoom (string newRoomName, string newRoomGenre, string newRoomData, string newRoomPlaylist) {
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/rooms/" + RoomModel.getInstance().CurrentRoom.RoomId) as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		Debug.Log ("CURRENT ROOMAROO! " + RoomModel.getInstance ().CurrentRoom.RoomId);
		request.Method = "PUT";

		request.ContentType = "application/json";
		request.Headers.Add("x-user-username", LoginModel.Username);
		request.Headers.Add("x-user-token", LoginModel.AuthKey );

		Debug.Log ("Request JSON = " + "{ \"room_data\": { \"name\": \""  + newRoomName.Trim ('"') + "\", \"genre\": \"" + newRoomGenre.Trim ('"') + "\", \"playlist\": " + newRoomPlaylist + "} }");
		Debug.Log ("Request Auth Key = " + LoginModel.AuthKey);
		Debug.Log ("Request URL = http://beforeheaveniqp.herokuapp.com/api/rooms/" + RoomModel.getInstance().CurrentRoom.RoomId);

		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{ \"room_data\": { \"name\": \""  + newRoomName + "\", \"genre\": \"" + newRoomGenre + "\", \"unity_data\" : \"" + newRoomData + "\", \"playlist\": " + newRoomPlaylist + "} }");
		//byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{ \"room_data\": { \"name\": \""  + newRoomName + "\", \"genre\": \"" + newRoomGenre + "\", \"unity_data\" : \"farts\" } }");
		request.ContentLength = byteArray.Length;
		using (var writer = request.GetRequestStream()){writer.Write(byteArray, 0, byteArray.Length);}
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}

	}

	public void getAllPlaylists(){
		var request = System.Net.WebRequest.Create ("http://api.soundcloud.com/me/playlists.json?oauth_token=" + LoginModel.SoundcloudAccessToken) as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		Debug.Log ("Request = http://api.soundcloud.com/me/playlists.json?oauth_token=" + LoginModel.AuthKey);
		request.Method = "GET";
		request.ContentType = "application/json";
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}

		var parsedPlaylist = JSON.Parse (responseContent);
		var playlists = new string[parsedPlaylist.AsArray.Count];
		var i = 0;
		foreach (JSONNode playlist in parsedPlaylist.AsArray){
			playlists[i] = playlist ["title"].ToString ();
			Debug.Log ("Adding playlist name " + playlist["title"].ToString ());
			i++;
		}
		LoginModel.PlaylistNames = playlists;
	}

	public void getUpdatedPlaylist(){
		var request = System.Net.WebRequest.Create ("http://api.soundcloud.com/me/playlists.json?oauth_token=" + LoginModel.SoundcloudAccessToken) as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		Debug.Log ("Request = http://api.soundcloud.com/me/playlists.json?oauth_token=" + LoginModel.AuthKey);
		request.Method = "GET";
		request.ContentType = "application/json";
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}

		var parsedPlaylist = JSON.Parse (responseContent);
		Debug.Log ("ParsedPlaylist = " + parsedPlaylist);
		Debug.Log ("Return = " + responseContent);

		foreach (JSONNode playlist in parsedPlaylist.AsArray){
			if (playlist["id"].ToString() == RoomModel.getInstance().CurrentRoom.PlaylistId){
				Debug.Log ("Updating playlist");
				updateRoom(RoomModel.getInstance().CurrentRoom.Name.Trim ('"'), RoomModel.getInstance().CurrentRoom.Genre.Trim ('"'), RoomModel.getInstance().CurrentRoom.RoomPreset.ToString(), playlist.ToString());
			}
		}
	}

	public void switchPlaylist(string newPlaylist){
		var request = System.Net.WebRequest.Create ("http://api.soundcloud.com/me/playlists.json?oauth_token=" + LoginModel.SoundcloudAccessToken) as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		Debug.Log ("Request = http://api.soundcloud.com/me/playlists.json?oauth_token=" + LoginModel.AuthKey);
		request.Method = "GET";
		request.ContentType = "application/json";
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}
		
		var parsedPlaylist = JSON.Parse (responseContent);
		Debug.Log ("ParsedPlaylist = " + parsedPlaylist);
		Debug.Log ("Return = " + responseContent);
		
		foreach (JSONNode playlist in parsedPlaylist.AsArray){
			if (playlist["title"].ToString() == newPlaylist){
				Debug.Log ("Switching playlist");
				updateRoom(RoomModel.getInstance().CurrentRoom.Name.Trim ('"'), RoomModel.getInstance().CurrentRoom.Genre.Trim ('"'), RoomModel.getInstance().CurrentRoom.RoomPreset.ToString(), playlist.ToString());
			}
		}
	}
}
