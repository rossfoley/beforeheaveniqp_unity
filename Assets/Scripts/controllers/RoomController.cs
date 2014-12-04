using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Text;
using System.Net;

public class RoomController : MonoBehaviour {

	private static RoomController instance = null;
	private string userEmail;
	private string userAuthKey;
	private string userId;
	private const string defaultRoom = "defaultRoom";
	private const string jazzRoom = "jazzRoom";
	
	private bool isChangingRoom;

	public static RoomController getInstance(){
		if(instance == null){
			instance = new RoomController();
		}
		return instance;
	}

	public string DefaultRoom {
		get {
			return defaultRoom;
		}
	}

	public string JazzRoom {
		get {
			return jazzRoom;
		}
	}

	//Constant URLs
	private const string roomsURL = "http://beforeheaveniqp.herokuapp.com/api/rooms";
	private const string roomSearchURL = "http://beforeheaveniqp.herokuapp.com/api/rooms/search/";

	// Use this for initialization
	void Start () {
		instance = this;

		// Grabs the user's login information from LoginModel
		userEmail = LoginModel.UserEmail;
		userAuthKey = LoginModel.AuthKey;
		userId = LoginModel.UserId;

		// Boolean used to check if it is in the process of switching rooms
		isChangingRoom = false;
		
		// Retrieve all the rooms currently on the database
		StartCoroutine(getRooms (""));
	}

	// Gets all the rooms from the database
	// string searchTerm - The search term that is passed to the database, if it is an empty string,
	// all the rooms are returned
	public IEnumerator getRooms(string searchTerm){
		// Set up the request
		Hashtable headers = new Hashtable();
		headers.Add ("Content-Type", "application/json");
		headers.Add("X-User-Email", userEmail);
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
			RoomData roomData = new RoomData(data["_id"]["$oid"], data["name"].ToString(), data["genre"].ToString(), data["visits"].AsInt, memberIds);
			RoomModel.getInstance().AllRooms[roomCount] = roomData;
			roomCount++;
		}
	}

	public IEnumerator createRoom(string newRoomName, string newRoomGenre, string roomPreset){
		RoomConfigMenu.CreateRoomStatus = 1; //Creating
		// Set up the request
		WWWForm roomCreateForm = new WWWForm();
		var newRoomData = new Hashtable();
		newRoomData.Add ("name", newRoomName);
		newRoomData.Add ("genre", newRoomGenre);
		var headers = new Hashtable();
		headers.Add ("Content-Type", "application/json");
		headers.Add ("X-User-Email", userEmail);
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
		request.Headers.Add("x-user-email", userEmail);
		request.Headers.Add("x-user-token", userAuthKey);
		request.ContentLength = 0;
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}

		NetworkManager.getInstance().kickAll();
	}

	public void addBandMember(string roomId, string newMemberEmail) {
		RoomConfigMenu.UpdateRoomStatus = 1;
		
		// Create the put request for adding the new band member
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/rooms/" + roomId + "/add_band_member/") as System.Net.HttpWebRequest;
		
		request.KeepAlive = true;
		
		request.Method = "PUT";
		
		request.ContentType = "application/json";
		request.Headers.Add("x-user-email", userEmail);
		request.Headers.Add("x-user-token", userAuthKey);
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{\"new_member_email\": \"" + newMemberEmail + "\"}");
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
			Debug.Log ("Invalid email entered");
			RoomConfigMenu.UpdateRoomStatus = -2;
		}
	}

	public void updateRoom (string newRoomName, string newRoomGenre, string newRoomData) {
		var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/rooms/" + RoomModel.getInstance().CurrentRoom.RoomId) as System.Net.HttpWebRequest;
		request.KeepAlive = true;
		Debug.Log ("CURRENT ROOMAROO! " + RoomModel.getInstance ().CurrentRoom.RoomId);
		request.Method = "PUT";
		
		request.ContentType = "application/json";
		request.Headers.Add("x-user-email", LoginModel.UserEmail );
		request.Headers.Add("x-user-token", LoginModel.AuthKey );
		
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{ \"room_data\": { \"name\": \""  + newRoomName + "\", \"genre\": \"" + newRoomGenre + "\" } }");
		request.ContentLength = byteArray.Length;
		using (var writer = request.GetRequestStream()){writer.Write(byteArray, 0, byteArray.Length);}
		string responseContent=null;
		using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
			using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
				responseContent = reader.ReadToEnd();
			}
		}
	}
}
