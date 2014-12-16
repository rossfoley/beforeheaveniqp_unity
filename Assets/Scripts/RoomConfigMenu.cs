using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
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

	// Room preset structures
	int enumLength = RoomController.roomPresets.GetValues(typeof(RoomController.roomPresets)).Length;
	int presetIndex = 0;
	int chosenPreset = 0;
	string[] presets;
	bool[] presetValues;
	
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

		presets = new string[enumLength];
		presetValues = new bool[enumLength];
		
		foreach (var value in RoomController.roomPresets.GetValues(typeof(RoomController.roomPresets))) {
			presets[presetIndex] = value.ToString ();
			presetValues[presetIndex] = false;
			presetIndex++;
		}
		presetValues[0] = true;
		presetIndex = 0;
		Debug.Log ("HAPPENED");
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
		
		GUILayout.BeginArea (new Rect(guiEdgeBorder,
		                              guiEdgeBorder,
		                              configWindowRect.width/2-guiEdgeBorder,
		                              configWindowRect.height - guiEdgeBorder));
		GUILayout.BeginVertical();
		// Create a new room

		CreateRoom();

		GUILayout.Space(4*guiEdgeBorder);

		if(RoomModel.getInstance().userIsMember()){
			// Add a new band member to the current room
			AddMember();
		}
		else{
			RoomDetails();
		}


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
				updateRoomName = GUILayout.TextField (updateRoomName.Trim ('"'));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label ("Room Genre: ");
				updateRoomGenre = GUILayout.TextField (updateRoomGenre.Trim ('"'));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label ("New member username: ");
				GUI.SetNextControlName ("email field");
				newMemberEmail = GUILayout.TextField (newMemberEmail);
			GUILayout.EndHorizontal();

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
						RoomController.getInstance().updateRoom(updateRoomName, updateRoomGenre, "", "");
						thisRoom.Name = updateRoomName;
						thisRoom.Genre = updateRoomGenre;
						RoomModel.getInstance().CurrentRoom = thisRoom;
						NetworkManager.getInstance ().updateAll(updateRoomName, updateRoomGenre);
						newRoomName = "";
						newRoomGenre = "";
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
		
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (RoomModel.getInstance().CurrentRoom.Name.Trim ('"') != "Starting Room") {
			if (GUILayout.Button ("Delete Room")) {
				RoomController.getInstance().deleteRoom();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (RoomModel.getInstance().CurrentRoom.Name.Trim ('"') != "Starting Room") {
			if (GUILayout.Button ("Refresh Playlist")) {
				RoomController.getInstance().getUpdatedPlaylist();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	void RoomDetails(){
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Room Name: ");
		GUILayout.Label (thisRoom.Name.Trim ('"'));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Room Genre: ");
		GUILayout.Label (thisRoom.Genre.Trim ('"'));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Visits: ");
		GUILayout.Label (thisRoom.Visits.ToString());
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	void CreateRoom () {
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label ("Create Room");
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();

				GUILayout.BeginHorizontal();
					GUILayout.Label ("Room Name: ");
					newRoomName = GUILayout.TextField (newRoomName);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Label ("Room Genre: ");
					newRoomGenre = GUILayout.TextField (newRoomGenre);
				GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			for (int i = 0; i <= presets.Length - 1; i++) {
				if (GUILayout.Toggle(presetValues[i], presets[i])) {
					setAllPresetsFalse();
					presetValues[i] = true;
					chosenPreset = i;
				}
			}
			GUILayout.EndVertical();

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Submit Room")) {
			if (newRoomName.Trim() == "" || newRoomGenre.Trim() == "") {
				//TODO Error
				Debug.Log("Invalid Strings");
			}
			else {
				StartCoroutine (RoomController.getInstance().createRoom (newRoomName, newRoomGenre, chosenPreset));
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

	void setAllPresetsFalse () {
		for (int j = 0; j <= presetValues.Length - 1; j++) {
			presetValues[j] = false;
		}
	}
}
