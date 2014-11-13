﻿using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Text;

public class RoomController : MonoBehaviour {

	private static RoomController instance = null;
	private string userEmail;
	private string userAuthKey;
	private string userId;

	//Unity GameObject Links
	public GameObject currentRoomObject;
	public GameObject roomTemplate;
	public GameObject roomMenuTemplate;

	private GameObject roomMenu;
	public NetworkManager networkManager;
	private bool isChangingRoom;

	private string nextRoom;

	public static RoomController getInstance(){
		if(instance == null){
			instance = new RoomController();
		}
		return instance;
	}

	//Constant URLs
	private const string loginURL = "http://beforeheaveniqp.herokuapp.com/api/users/login";
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

		// Sets up the room config menu
		roomMenu = (GameObject)Instantiate (roomMenuTemplate);
		RoomConfigMenu rcm = roomMenu.GetComponent("RoomConfigMenu") as RoomConfigMenu;
		rcm.AuthKey = userAuthKey;
		rcm.UserEmail = userEmail;
		rcm.ThisRoom = RoomModel.getInstance().CurrentRoom;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Gets all the rooms from the database
	// string searchTerm - The search term that is passed to the database, if it is an empty string,
	// all the rooms are returned
	public IEnumerator getRooms(string searchTerm){
		//Set credentials
		userEmail = LoginModel.UserEmail;
		userAuthKey = LoginModel.AuthKey;
		userId = LoginModel.UserId;

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
		RoomModel hi = RoomModel.getInstance ();
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

	public IEnumerator createRoom(string newRoomName, string newRoomGenre){
		ElevatorMenu.CreateRoomStatus = 1; //Creating
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
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{\"room_data\": {\"name\": \"" + newRoomName + "\",\"genre\": \"" + newRoomGenre + "\"} }");
		
		StringBuilder data = new StringBuilder();
		data.Append("{\n");
		data.Append("\t\"name\":");
		data.Append(" \"" + newRoomName + "\",\n");
		data.Append("\t\"genre\":");
		data.Append(" \"" + newRoomGenre + "\"\n");
		data.Append("}");
		
		roomCreateForm.AddField("room_data", data.ToString ());
		
		
		byte[] rawData = roomCreateForm.data;
		
		WWW newRoomRequest = new WWW(roomsURL, byteArray, headers);
		yield return newRoomRequest;
		if (!string.IsNullOrEmpty(newRoomRequest.error)) {
			//TODO login.error returns the string of the error, so catch the different types of errors and do different error messages
			//with the switch statement in OnGUI()
			Debug.Log ("room creation error");
			ElevatorMenu.CreateRoomStatus = -1;

		}
		else{
			// After the room is created, hide the create room window and reset the related variables.
			newRoomName = "";
			newRoomGenre = "";
			ElevatorMenu.CreateRoomStatus = 2;
		}
	}


	// TODO Split out into location manager?
	public void changeRoom (int i)
	{
		isChangingRoom = true;
		PhotonNetwork.LeaveRoom();
		
		nextRoom = RoomModel.getInstance().getRoom(i).Name;
		
		Destroy (currentRoomObject);
		if (roomMenu != null){
			Destroy (roomMenu);
		}
		// Update currentRoomObject and Data
		if(roomTemplate == null){
			Debug.Log ("null roomtemplate");
		}
		currentRoomObject = (GameObject) Instantiate(roomTemplate);
		RoomModel.getInstance().CurrentRoom.RoomId = RoomModel.getInstance().AllRooms[i].RoomId;
		RoomModel.getInstance().CurrentRoom.Name = RoomModel.getInstance().AllRooms[i].Name;
		RoomModel.getInstance().CurrentRoom.Genre = RoomModel.getInstance().AllRooms[i].Genre;
		RoomModel.getInstance().CurrentRoom.Visits = RoomModel.getInstance().AllRooms[i].Visits;
		RoomModel.getInstance().CurrentRoom.Members = RoomModel.getInstance().AllRooms[i].Members;

		// Create a new room config menu if the user is a part of the members of the room
		if (RoomModel.getInstance().roomHasMembers() && RoomModel.getInstance ().userIsMember()){
			roomMenu = (GameObject) Instantiate(roomMenuTemplate);
			RoomConfigMenu rcm = roomMenu.GetComponent("RoomConfigMenu") as RoomConfigMenu;
			rcm.ThisRoom = RoomModel.getInstance().CurrentRoom;
		}
		// If the user is not a member, they can not see the room config menu
		else {
			roomMenu = null;
		}
	}
	
	void OnJoinedLobby() {
		RoomOptions testRO = new RoomOptions ();
		// Join the room if it is already active on the server, otherwise create it
		if (isChangingRoom){
			PhotonNetwork.JoinOrCreateRoom (nextRoom.Trim('"'), testRO, PhotonNetwork.lobby);
			isChangingRoom = false;
		}
	}

	void Connect () {
		PhotonNetwork.ConnectUsingSettings ("BefoHev V001");
	}
	
}
