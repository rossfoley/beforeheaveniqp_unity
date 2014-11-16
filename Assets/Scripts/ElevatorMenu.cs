using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class ElevatorMenu : MonoBehaviour {
	
	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;
	public int guiEdgeBorder = 20;

	// Text field strings
	private string newRoomName = "";
	private string newRoomGenre = "";
	private string searchField = "";
	private int updateCounter = 0;

	private static int createRoomStatus = 0;
	private static RoomData[] allRooms;
	private static RoomData currentRoom;

	public static int CreateRoomStatus {
		get {
			return createRoomStatus;
		}
		set {
			createRoomStatus = value;
		}
	}

	public static RoomData[] AllRooms {
		get {
			return allRooms;
		}
		set {
			allRooms = value;
		}
	}

	public static RoomData CurrentRoom {
		get {
			return currentRoom;
		}
		set {
			currentRoom = value;
		}
	}

	private Rect elevatorWindowRect = new Rect (50, 50, Screen.width - 100, Screen.height - 100);

	// Boolean for whether or not the elevator menu is currently visible
	private bool isElWindowVisible = false;

	// Boolean for whether or not create room group is currently visible
	private bool isCrWindowVisible = false;

	// Creates the elevator window and create room buttons
	void OnGUI() {

		createFriendList ();

		GUILayout.BeginArea (new Rect (guiEdgeBorder, guiEdgeBorder, 400, 60));

		// If the elevator window is visible, create the GUI window
		if (isElWindowVisible) {
			elevatorWindowRect = GUI.Window (0, elevatorWindowRect, ElevatorWindowFunction, "Welcome to the elevator!");
		}

		GUILayout.BeginHorizontal();

		// When the elevator button is clicked, switch the visibility of the elevator menu
		if (GUILayout.Button ("Elevator")) {
			// If the elevator window is going to appear, update allRooms by getting all the rooms with no search string
			if (!isElWindowVisible){
				StartCoroutine(RoomController.getInstance().getRooms(""));
			}
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
				if (newRoomName.Trim() == "" || newRoomGenre.Trim() == "") {
					//TODO Error
					Debug.Log("Invalid Strings");
				}
				else {
					StartCoroutine (RoomController.getInstance().createRoom (newRoomName, newRoomGenre));
				}
			}
			switch(createRoomStatus){
			case 1:
				GUI.Label (new Rect(100, 4*topLay, 100, 20), "Creating...");
				break;
			case 2:
				createRoomStatus = 0;
				isCrWindowVisible = false;
				break;
			case -1:
				GUI.Label (new Rect(100, 4*topLay, 100, 20), "Creation error.");
				break;
			}
			GUI.EndGroup();
		}
	}

	// Creates the friend's list and populates with the current user's friends
	void createFriendList() {
		if (updateCounter == 0){
			LoginController.getFriends ();
			updateCounter = 250;
		}
		else {
			updateCounter--;
		}
		GUILayout.BeginArea (new Rect (500, 100, 600, 300));
		GUI.Label (new Rect(200, 0, 200, 20), "Friends List");
		// Populates a scroll view with all of the rooms currently in the database
		GUI.skin.scrollView = style;
		if(LoginModel.FriendData.Length > 0) {
			scrollPosition = GUI.BeginScrollView (
				new Rect (200, 2 * guiEdgeBorder, 200, 500 - guiEdgeBorder),
				scrollPosition, 
				new Rect(0, 0, 200, 20*LoginModel.FriendData.Length));
			for (int i = 0; i < LoginModel.FriendIds.Length; i++) {
				if(GUI.Button(new Rect(0, 20*i, 200, 20), LoginModel.FriendData[i].UserEmail)) {
					// TODO yield return?
					Debug.Log ("Button pressed");
					string roomName = LoginController.getCurrentRoomOfUser(LoginModel.FriendData[i].UserId);
					Debug.Log ("Friend room name " + roomName);
					RoomController.getInstance().getRooms("");
					RoomData friendRD = null;
					int j = 0;
					foreach (RoomData rd in RoomModel.getInstance().AllRooms){
						if (rd.Name.Trim ('"').Equals(roomName)){
							friendRD = rd;
							Debug.Log ("friendRD = " + friendRD.RoomId);
							break;
						}
						j++;
					}
					if (friendRD != null){
						NetworkManager.getInstance().changeRoom(friendRD);
					}
				}
			}
			GUI.EndScrollView();
		}
		GUILayout.EndArea ();
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
			StartCoroutine(RoomController.getInstance().getRooms(searchField.Trim ()));
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
				if (RoomModel.getInstance().CurrentRoom.Name != allRooms[i].Name) {
					// If one of the room buttons is pressed, join that room
					if(GUI.Button(new Rect(0, 20*i, width / 2, 20), allRooms[i].Name)) {
						isElWindowVisible = false;

						NetworkManager.getInstance().changeRoom(allRooms[i]);


					}
				}
			}
			GUI.EndScrollView();
		}
	}



}
