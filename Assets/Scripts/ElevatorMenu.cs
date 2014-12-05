using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class ElevatorMenu : MonoBehaviour {

	private string searchField = "";
	private Rect elevatorWindowRect = new Rect(50, 50, Screen.width - 100, Screen.height - 100);
	private int guiEdgeBorder = GUIController.GuiEdgeBorder;

	private static RoomData[] allRooms;
	private static RoomData currentRoom;
	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;

	public static RoomData[] AllRooms {
		get { return allRooms; }
		set { allRooms = value; }
	}
	
	public static RoomData CurrentRoom {
		get { return currentRoom; }
		set { currentRoom = value; }
	}

	void OnGUI () {
		elevatorWindowRect = GUI.Window (0, elevatorWindowRect, 
		                                 ElevatorWindowFunction, "Welcome to the elevator!");
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
		GUILayout.BeginArea (new Rect(0, 0, elevatorWindowRect.width, 23));
		GUILayout.BeginHorizontal();
			GUILayout.Space(elevatorWindowRect.width-23);
			if (GUILayout.Button("X")) {
				GUIController.ElWindowVisible = false;
			}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();

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
					GUILayout.BeginArea(new Rect(0, 40*i, width/2, 40));
					if(GUI.Button(new Rect(0, 0, width / 2, 20), allRooms[i].Name)) {
						GUIController.ElWindowVisible = false;

						NetworkManager.getInstance().changeRoom(allRooms[i]);


					}
					GUILayout.EndArea();
				}
			}
			GUI.EndScrollView();
		}
	}



}
