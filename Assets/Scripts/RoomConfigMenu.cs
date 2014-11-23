using UnityEngine;
using System.Collections;
using System.Net;

public class RoomConfigMenu : MonoBehaviour {

	private static RoomData thisRoom;
	private static bool userIsMember;
	private string newMemberEmail = "";
	private string newRoomName = ""; //RoomModel.getInstance().CurrentRoom.Name;
	private string newRoomGenre = ""; //RoomModel.getInstance().CurrentRoom.Genre;
	
	private static int createRoomStatus = 0;
	private static int addMemberStatus;
	private static int updateRoomStatus;

	private Rect configWindowRect = new Rect(50, 50, Screen.width - 150, Screen.height - 150);

	private int guiEdgeBorder = GUIController.GuiEdgeBorder;
	
	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;

	// ThisRoom getter and setter
	public static RoomData ThisRoom {
		get { return thisRoom; }
		set { thisRoom = value; }
	}

	public static bool UserIsMember {
		get { return userIsMember; }
		set { userIsMember = value; }
	}
	
	public static int AddMemberStatus {
		get { return addMemberStatus; }
		set { addMemberStatus = value; }
	}
		
	public static int CreateRoomStatus {
		get { return createRoomStatus; }
		set { createRoomStatus = value; }
	}

	void OnGUI() {
		configWindowRect = GUI.Window (0, configWindowRect, 
		                                 ConfigWindowFunction, "Room Configuration");
	}

	void ConfigWindowFunction (int windowID) {

		// Add a new band member to the current room
		addMember();

		// Create a new room
		// TODO: Format create room GUI
		//createRoom();
	}

	void addMember () {
		GUI.BeginGroup (new Rect(0, 0, 500, 300));
		GUI.Box (new Rect (0, 0, 500, 300), "Room Management");
		GUI.Label (new Rect (10, 10, 100, 20), thisRoom.Name);
		GUI.Label (new Rect (10, 30, 100, 20), thisRoom.Genre);
		GUI.Label (new Rect (10, 50, 100, 20), thisRoom.Visits.ToString());
		GUI.Label (new Rect (10, 70, 100, 20), "New member email");
		GUI.Label (new Rect (100, 140, 100, 20), "New room name");
		GUI.Label (new Rect (100, 180, 100, 20), "New room genre");
		GUI.SetNextControlName ("email field");
		newMemberEmail = GUI.TextField (new Rect (100, 70, 100, 20), newMemberEmail);
		newRoomName = GUI.TextField (new Rect (200, 140, 100, 20), newRoomName);
		newRoomGenre = GUI.TextField (new Rect (200, 180, 100, 20), newRoomGenre);
		
		if (GUI.Button (new Rect (30, 90, 50, 20), "Submit") || 
		    (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "email field")){
			// If no email is entered, do not go through with the request
			if (newMemberEmail.Trim() == ""){
				addMemberStatus = -1;
				//TODO Error
				Debug.Log("Email not entered");
			}
			// Put request for a new band member
			else {
				RoomController.getInstance().addBandMember(thisRoom.RoomId, newMemberEmail);
			}
		}
		
		if (GUI.Button (new Rect (150, 220, 100, 20), "Update Room") || 
		    (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "email field")){
			Debug.Log ("Update Button Clicked");
			
			if (newRoomName.Trim() == "" && newRoomGenre.Trim() == "") {
				updateRoomStatus = -1;
				Debug.Log ("no room name");
			} else {
				RoomController.getInstance().updateRoom(newRoomName, newRoomGenre, "");
				thisRoom.Name = newRoomName;
				thisRoom.Genre = newRoomGenre;
				RoomModel.getInstance().CurrentRoom = thisRoom;
				newRoomName = "";
				newRoomGenre = "";
			}
		}
		string status;
		switch(addMemberStatus){
		case 1: 
			status = "Adding...";
			break;
		case 2:
			status = "Added";
			break;
		case -1:
			status = "Email not entered";
			break;
		case -2:
			status = "Invalid email entered";
			break;
		default:
			status = "";
			break;
		}
		GUI.Label (new Rect (10, 110, 200, 20), status);
		
		if (RoomModel.getInstance().CurrentRoom.Name.Trim ('"') != "starting room") {
			if (GUI.Button (new Rect(10, 130, 100, 20), "Delete Room")) {
				RoomController.getInstance().deleteRoom();
			}
		}
		
		GUI.EndGroup();
	}

	void CreateRoom () {
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
			GUIController.CrWindowVisible = false;
			break;
		case -1:
			GUI.Label (new Rect(100, 4*topLay, 100, 20), "Creation error.");
			break;
		}
		GUI.EndGroup();
	}

}
