using UnityEngine;
using System.Collections;
using System.Net;

public class RoomConfigMenu : MonoBehaviour {

	private static RoomData thisRoom;
	private static bool userIsMember;
	private string newMemberEmail = "";
	private string updateRoomName = "";
	private string updateRoomGenre = "";
	private string newRoomName = "";
	private string newRoomGenre = "";
	private string roomPreset = ""; 
	
	private static int createRoomStatus = 0;
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
	
	public static int UpdateRoomStatus {
		get { return updateRoomStatus; }
		set { updateRoomStatus = value; }
	}
		
	public static int CreateRoomStatus {
		get { return createRoomStatus; }
		set { createRoomStatus = value; }
	}

	void OnEnable (){
		Debug.Log("Enable Called");
		newMemberEmail = "";
		newRoomGenre = "";
		newRoomName = "";
		updateRoomName = RoomModel.getInstance().CurrentRoom.Name;
		updateRoomGenre = RoomModel.getInstance().CurrentRoom.Genre;
	}

	void OnGUI() {
		configWindowRect = GUI.Window (0, configWindowRect, 
		                                 ConfigWindowFunction, "Room Configuration");
	}

	void ConfigWindowFunction (int windowID) {

		// Exit button
		GUILayout.BeginArea (new Rect(0, 0, configWindowRect.width, 23));
		GUILayout.BeginHorizontal();
			GUILayout.Space(configWindowRect.width-23);
			if (GUILayout.Button("X")) {
				GUIController.CrWindowVisible = false;
			}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		GUILayout.BeginArea (new Rect(configWindowRect.width/3+guiEdgeBorder,
		                              guiEdgeBorder,
		                              configWindowRect.width/3-guiEdgeBorder,
		                              configWindowRect.height-guiEdgeBorder));
		GUILayout.BeginVertical();
		// Add a new band member to the current room
		AddMember();

		GUILayout.FlexibleSpace();

		// Create a new room
		CreateRoom();

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	void AddMember () {
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label ("Edit Room");
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.Label ("Visits: ");
			GUILayout.Label (thisRoom.Visits.ToString());
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.Label ("Room Name: ");
			updateRoomName = GUILayout.TextField (updateRoomName);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.Label ("Room Genre: ");
			updateRoomGenre = GUILayout.TextField (updateRoomGenre);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.Label ("New member email: ");
			GUI.SetNextControlName ("email field");
			newMemberEmail = GUILayout.TextField (newMemberEmail);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Update Room") || 
		    (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "email field")){
			Debug.Log ("Update Button Clicked");
			
			// Put request for a new band member
			if (newMemberEmail.Trim() != "") {
				RoomController.getInstance().addBandMember(thisRoom.RoomId, newMemberEmail);
			}

			// Update room name and room genre
			if (updateRoomName.Trim() != thisRoom.Name || updateRoomGenre.Trim() != thisRoom.Genre) {
				if (updateRoomName.Trim() == "" && updateRoomGenre.Trim() == "") {
					updateRoomStatus = -2;
					Debug.Log ("No room name given");
				} 
				else {
					Debug.Log("Updating Room");
					RoomController.getInstance().updateRoom(updateRoomName, updateRoomGenre, "");
					thisRoom.Name = updateRoomName;
					thisRoom.Genre = updateRoomGenre;
					RoomModel.getInstance().CurrentRoom = thisRoom;
					newRoomName = "";
					newRoomGenre = "";
				}
			}
		}
		string status;
		switch(updateRoomStatus) {
		case 1: 
			status = "Adding...";
			break;
		case 2:
			status = "Added";
			break;
		case -1:
			status = "Invalid email entered";
			break;
		case -2:
			status = "Invalid room name";
			break;
		default:
			status = "";
			break;
		}
		GUILayout.Label (status);
		GUILayout.EndHorizontal();
		
		if (RoomModel.getInstance().CurrentRoom.Name.Trim ('"') != "Starting Room") {
			if (GUILayout.Button ("Delete Room")) {
				RoomController.getInstance().deleteRoom();
			}
		}

		GUILayout.EndVertical();
	}

	void CreateRoom () {		
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label ("Create Room");
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.Label ("Room Name: ");
			newRoomName = GUILayout.TextField (newRoomName);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.Label ("Room Genre: ");
			newRoomGenre = GUILayout.TextField (newRoomGenre);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Submit Room")) {
			if (newRoomName.Trim() == "" || newRoomGenre.Trim() == "") {
				//TODO Error
				Debug.Log("Invalid Strings");
			}
			else {
				StartCoroutine (RoomController.getInstance().createRoom (newRoomName, newRoomGenre, 
				                                                         RoomController.getInstance().JazzRoom));
			}
		}
		switch(createRoomStatus) {
		case 1:
			GUILayout.Label ("Creating...");
			break;
		case 2:
			createRoomStatus = 0;
			GUIController.CrWindowVisible = false;
			break;
		case -1:
			GUILayout.Label ("Creation error.");
			break;
		}

		GUILayout.EndHorizontal();
	}
}
