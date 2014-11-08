using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Text;
using System.IO;
using System;

public class ElevatorMenu : MonoBehaviour {
	
	public GameObject currentRoomObject;
	private RoomData currentRoomData;
	public GameObject roomTemplate;
	public GameObject roomMenuTemplate;
	private GameObject roomMenu;
	public NetworkManager networkManager;
	private bool isChangingRoom;
	private RoomData[] allRooms = new RoomData[0];

	private const string loginURL = "http://beforeheaveniqp.herokuapp.com/api/user/login";
	private const string roomsURL = "http://beforeheaveniqp.herokuapp.com/api/rooms";
	private const string roomURL = "http://beforeheaveniqp.herokuapp.com/api/room";

	private string nextRoom;

	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;
	public int GuiEdgeBorder = 20;

	private string userEmail;
	private string userAuthKey;
	private string userId;

	private string newRoomName = "";
	private string newRoomGenre = "";
	
	// Use this for initialization
	IEnumerator Start () {

		isChangingRoom = false;

		userEmail = LoginScript.UserEmail;
		userAuthKey = LoginScript.AuthKey;
		userId = LoginScript.UserId;

		Debug.Log (userEmail);
		Debug.Log (userId);
	
		// Retrieve all the rooms currently on the database
		var headers = new Hashtable();
		headers.Add ("Content-Type", "application/json");
		headers.Add("X-User-Email", userEmail);
		headers.Add("X-User-Token", userAuthKey);
		WWW rooms = new WWW (roomsURL, null, headers);
		yield return rooms;
		Debug.Log (rooms.text);
		var roomsParsed = JSON.Parse (rooms.text);
		allRooms = new RoomData[roomsParsed["data"].AsArray.Count];
		int roomCount = 0;
		foreach(JSONNode data in roomsParsed["data"].AsArray){
			string[] memberIds = new string[data["member_ids"].AsArray.Count];
			int i = 0;
			Debug.Log("Starting memberIds loop");
			foreach(JSONNode members in data["member_ids"].AsArray){
				memberIds[i] = members["$oid"];
				Debug.Log ("Member " + i + ": " + members["$oid"]);
				i++;
			}
			RoomData roomData = new RoomData(data["_id"]["$oid"], data["name"].ToString(), data["genre"].ToString(), data["visits"].AsInt, memberIds);
			allRooms[roomCount] = roomData;
			roomCount++;
		}
		currentRoomData = new RoomData ("0", "Start", "none", 0, null);
		roomMenu = (GameObject)Instantiate (roomMenuTemplate);
		RoomConfigMenu rcm = roomMenu.GetComponent("RoomConfigMenu") as RoomConfigMenu;
		rcm.AuthKey = userAuthKey;
		rcm.UserEmail = userEmail;
		rcm.ThisRoom = currentRoomData;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private Rect elevatorWindowRect = new Rect (50, 50, Screen.width - 100, Screen.height - 100);
	private bool isElWindowVisible = false;
	private bool isCrWindowVisible = false;

	// Creates the main elevator window
	void OnGUI() {
		if (GUI.Button (new Rect (20, 20, 100, 20), "Elevator")) {
			isElWindowVisible = !isElWindowVisible;
		}
		if (isElWindowVisible) {
			elevatorWindowRect = GUI.Window (0, elevatorWindowRect, ElevatorWindowFunction, "Welcome to the elevator!");
		}
	}

	// This is the main elevator window
	void ElevatorWindowFunction (int windowID) {
		int exitButtonSize = 23;
		float width = elevatorWindowRect.width;
		float height = elevatorWindowRect.height;
		Rect createWindowRect = new Rect (width + GuiEdgeBorder, 
		                                  height + GuiEdgeBorder,
		                                  width * 1/3, height - 2*GuiEdgeBorder);

		// Exit button
		if (GUI.Button (new Rect (elevatorWindowRect.width - GuiEdgeBorder - exitButtonSize,
		                          GuiEdgeBorder, exitButtonSize, exitButtonSize), "X")) {
			isElWindowVisible = false;
		}

		// Populates a scroll view with all of the rooms currently in the database
		GUI.skin.scrollView = style;
		if(allRooms.Length > 0) {
			scrollPosition = GUI.BeginScrollView (
				new Rect (width / 3, 2 * GuiEdgeBorder, width / 2, height - GuiEdgeBorder),
				scrollPosition, 
				new Rect(0, 0, width / 2, 20*allRooms.Length));
			for (int i = 0; i < allRooms.Length; i++) {
				// If the current room has the same name as the next room, do not create the button for that room
				if (currentRoomData.Name != allRooms[i].Name) {
					// If one of the room buttons is pressed, join that room
					if(GUI.Button(new Rect(0, 20*i, width / 2, 20), allRooms[i].Name)) {
						isChangingRoom = true;
						isElWindowVisible = false;
						PhotonNetwork.LeaveRoom();

						nextRoom = allRooms[i].Name;

						Destroy (currentRoomObject);
						if (roomMenu != null){
							Debug.Log ("Destroying roomMenu");
							Destroy (roomMenu);
						}
						// Update currentRoomObject and Data
						currentRoomObject = (GameObject) Instantiate(roomTemplate);
						currentRoomData.RoomId = allRooms[i].RoomId;
						currentRoomData.Name = allRooms[i].Name;
						currentRoomData.Genre = allRooms[i].Genre;
						currentRoomData.Visits = allRooms[i].Visits;
						currentRoomData.Members = allRooms[i].Members;
						bool isNullArray = false;
						Debug.Log(currentRoomData.Name + " " + currentRoomData.Members.Length);
						if (currentRoomData.Members.Length == 0){
							isNullArray = true;
							Debug.Log ("Array is null");
						}
						Debug.Log(userId);
						if (!isNullArray && (Array.IndexOf (currentRoomData.Members, userId) >= 0)){
							Debug.Log ("Members array is not null: " + currentRoomData.Members[0]);
							roomMenu = (GameObject) Instantiate(roomMenuTemplate);
							RoomConfigMenu rcm = roomMenu.GetComponent("RoomConfigMenu") as RoomConfigMenu;
							rcm.ThisRoom = currentRoomData;
						}
						else {
							roomMenu = null;
						}
					}
				}
			}
			GUI.EndScrollView();
		}

		// Create Room button
		if (GUI.Button (new Rect(GuiEdgeBorder, GuiEdgeBorder, 100, 20), "Create Room")) {
			isCrWindowVisible = !isCrWindowVisible;
		}
		if (isCrWindowVisible) {
			int topLay = 25;
			GUI.BeginGroup (new Rect (GuiEdgeBorder, GuiEdgeBorder, width/3 - GuiEdgeBorder, height - 2*GuiEdgeBorder));
			newRoomName = GUI.TextField(new Rect(100, topLay, width/4 - 100, 20), newRoomName, 20);
			newRoomGenre = GUI.TextField (new Rect(100, 2*topLay, width/4 - 100, 20), newRoomGenre, 20);
			GUI.Label (new Rect(0, topLay, 100, 20), "Room Name: ");
			GUI.Label (new Rect(0, 2*topLay, 100, 20), "Room Genre: ");
			if (GUI.Button (new Rect(100, 3*topLay, 100, 20), "Submit Room")) {
				Debug.Log("Submit clicked");
				if (newRoomName.Trim() == "" || newRoomGenre.Trim() == "") {
					Debug.Log("Invalid Strings");
				}
				else {
					Debug.Log("Calling createRoom");
					StartCoroutine (createRoom (newRoomName, newRoomGenre));
				}
			}
			GUI.EndGroup();
		}
	}

	void Connect () {
		PhotonNetwork.ConnectUsingSettings ("BefoHev V001");
	}
	
	void OnJoinedLobby() {
		RoomOptions testRO = new RoomOptions ();
		// Join the room if it is already active on the server, otherwise create it
		if (isChangingRoom){
			Debug.Log (nextRoom);
			PhotonNetwork.JoinOrCreateRoom (nextRoom.Trim('"'), testRO, PhotonNetwork.lobby);
			isChangingRoom = false;
		}
	}

	IEnumerator createRoom(string newRoomName, string newRoomGenre){
		Debug.Log ("Create Room Called");
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

		Debug.Log (roomCreateForm.ToString ());

		byte[] rawData = roomCreateForm.data;

		WWW newRoomRequest = new WWW(roomURL, byteArray, headers);
		yield return newRoomRequest;
		Debug.Log(newRoomRequest.text);

		isCrWindowVisible = false;
	}
}
