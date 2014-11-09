using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Text;
using System.IO;
using System;

public class ElevatorMenu : MonoBehaviour {

	//Unity GameObject Links
	public GameObject currentRoomObject;
	public GameObject roomTemplate;
	public GameObject roomMenuTemplate;

	private RoomData currentRoomData;

	private GameObject roomMenu;
	public NetworkManager networkManager;
	private bool isChangingRoom;
	private RoomData[] allRooms = new RoomData[0];

	//Constant URLs
	private const string loginURL = "http://beforeheaveniqp.herokuapp.com/api/users/login";
	private const string roomsURL = "http://beforeheaveniqp.herokuapp.com/api/rooms";
	private const string roomSearchURL = "http://beforeheaveniqp.herokuapp.com/api/rooms/search/";

	private string nextRoom;

	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;
	public int guiEdgeBorder = 20;

	private string userEmail;
	private string userAuthKey;
	private string userId;

	private string newRoomName = "";
	private string newRoomGenre = "";
	private string searchField = "";

	public RoomData getCurrentRoom(){
		return currentRoomData;
	}

	// Use this for initialization
	void Start () {

		// Boolean used to check if it is in the process of switching rooms
		isChangingRoom = false;

		// Grabs the user's login information from LoginScript
		userEmail = LoginScript.UserEmail;
		userAuthKey = LoginScript.AuthKey;
		userId = LoginScript.UserId;

		Debug.Log (userEmail);
		Debug.Log (userId);
	
		// Retrieve all the rooms currently on the database
		StartCoroutine(getRooms (""));

		// Creates a dummy currentRoomData for the starting room
		currentRoomData = new RoomData ("0", "Start", "none", 0, null);

		// Sets up the room config menu
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

	// Boolean for whether or not the elevator menu is currently visible
	private bool isElWindowVisible = false;

	// Boolean for whether or not create room group is currently visible
	private bool isCrWindowVisible = false;

	// Creates the elevator window and create room buttons
	void OnGUI() {

		GUILayout.BeginArea (new Rect (guiEdgeBorder, guiEdgeBorder, 400, 60));

		// If the elevator window is visible, create the GUI window
		if (isElWindowVisible) {
			elevatorWindowRect = GUI.Window (0, elevatorWindowRect, ElevatorWindowFunction, "Welcome to the elevator!");
		}

		GUILayout.BeginHorizontal();

		// When the elevator button is clicked, switch the visibility of the elevator menu
		if (GUILayout.Button ("Elevator")) {
			isElWindowVisible = !isElWindowVisible;
		}
		// Create Room button
		if (GUILayout.Button ("Create Room")) {
			isCrWindowVisible = !isCrWindowVisible;
		}

		GUILayout.EndHorizontal();
		GUILayout.EndArea();

		// If the create room window is visible, create + display all the GUI elements of the window
		if (isCrWindowVisible) {
			int topLay = 25;
			GUI.BeginGroup (new Rect (guiEdgeBorder, guiEdgeBorder, Screen.width/3 - guiEdgeBorder, Screen.height - 2*guiEdgeBorder));
			newRoomName = GUI.TextField(new Rect(100, topLay, Screen.width/4 - 100, 20), newRoomName, 20);
			newRoomGenre = GUI.TextField (new Rect(100, 2*topLay, Screen.width/4 - 100, 20), newRoomGenre, 20);
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

	// This is the main elevator window
	void ElevatorWindowFunction (int windowID) {
		int exitButtonSize = 23;
		float width = elevatorWindowRect.width;
		float height = elevatorWindowRect.height;
		Rect createWindowRect = new Rect (width + guiEdgeBorder, 
		                                  height + guiEdgeBorder,
		                                  width * 1/3, height - 2*guiEdgeBorder);

		// Exit button
		if (GUI.Button (new Rect (elevatorWindowRect.width - guiEdgeBorder - exitButtonSize,
		                          guiEdgeBorder, exitButtonSize, exitButtonSize), "X")) {
			isElWindowVisible = false;
		}

		//Search field for rooms
		GUILayout.BeginArea(new Rect(guiEdgeBorder, 2*guiEdgeBorder, elevatorWindowRect.width/3 - 2*guiEdgeBorder, 50));
		GUILayout.BeginVertical();

		// Sets up the search GUI elements in the elevator menu
		GUI.SetNextControlName("search field");
		searchField = GUILayout.TextField(searchField);
		if(GUILayout.Button("Search") ||
		   Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "search field"){
			StartCoroutine(getRooms(searchField.Trim ()));
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
		
		// Populates a scroll view with all of the rooms currently in the database
		GUI.skin.scrollView = style;
		if(allRooms.Length > 0) {
			scrollPosition = GUI.BeginScrollView (
				new Rect (width / 3, 2 * guiEdgeBorder, width / 2, height - guiEdgeBorder),
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
						if (currentRoomData.Members.Length == 0){
							isNullArray = true;
						}

						// Create a new room config menu if the user is a part of the members of the room
						if (!isNullArray && (Array.IndexOf (currentRoomData.Members, userId) >= 0)){
							roomMenu = (GameObject) Instantiate(roomMenuTemplate);
							RoomConfigMenu rcm = roomMenu.GetComponent("RoomConfigMenu") as RoomConfigMenu;
							rcm.ThisRoom = currentRoomData;
						}
						// If the user is not a member, they can not see the room config menu
						else {
							roomMenu = null;
						}
					}
				}
			}
			GUI.EndScrollView();
		}
	}

	void Connect () {
		PhotonNetwork.ConnectUsingSettings ("BefoHev V001");
	}
	
	void OnJoinedLobby() {
		RoomOptions testRO = new RoomOptions ();
		// Join the room if it is already active on the server, otherwise create it
		if (isChangingRoom){
			PhotonNetwork.JoinOrCreateRoom (nextRoom.Trim('"'), testRO, PhotonNetwork.lobby);
			isChangingRoom = false;
		}
	}

	IEnumerator createRoom(string newRoomName, string newRoomGenre){
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

		// After the room is created, hide the create room window
		isCrWindowVisible = false;
	}

	// Gets all the rooms from the database
	// string searchTerm - The search term that is passed to the database, if it is an empty string,
	// all the rooms are returned
	IEnumerator getRooms(string searchTerm){
		// Set up the request
		var headers = new Hashtable();
		headers.Add ("Content-Type", "application/json");
		headers.Add("X-User-Email", userEmail);
		headers.Add("X-User-Token", userAuthKey);
		WWW rooms;
		// If the search term is empty, grab all the rooms on server
		if(searchTerm == ""){
			rooms = new WWW (roomsURL, null, headers);
			yield return rooms;
		}
		// If there is a search term, send the request to the search URL with the search term
		else{
			rooms = new WWW(roomSearchURL + searchTerm, null, headers);
			yield return rooms;
		}
		Debug.Log (rooms.text);
		var roomsParsed = JSON.Parse (rooms.text);
		// Set allRooms to what was returned from the request
		allRooms = new RoomData[roomsParsed["data"].AsArray.Count];
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
			allRooms[roomCount] = roomData;
			roomCount++;
		}

	}
}
