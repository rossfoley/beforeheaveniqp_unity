using UnityEngine;
using System.Collections;
using System.Net;

public class RoomConfigMenu : MonoBehaviour {

	private static RoomData thisRoom;
	private static bool userIsMember;
	private string newMemberEmail = "";

	private static int addMemberStatus;

	// ThisRoom getter and setter
	public static RoomData ThisRoom {
		get {
			return thisRoom;
		}
		set {
			thisRoom = value;
		}
	}

	public static bool UserIsMember {
		get {
			return userIsMember;
		}
		set {
			userIsMember = value;
		}
	}
	
	public static int AddMemberStatus {
		get {
			return addMemberStatus;
		}
		set {
			addMemberStatus = value;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if(userIsMember){
			// Sets up the GUI components of the window
			GUI.BeginGroup (new Rect(500, 300, 500, 300));
			GUI.Box (new Rect (0, 0, 500, 300), "Room Management");
			GUI.Label (new Rect (10, 10, 100, 20), thisRoom.Name);
			GUI.Label (new Rect (10, 30, 100, 20), thisRoom.Genre);
			GUI.Label (new Rect (10, 50, 100, 20), thisRoom.Visits.ToString());
			GUI.Label (new Rect (10, 70, 100, 20), "New member email");
			GUI.SetNextControlName ("email field");
			newMemberEmail = GUI.TextField (new Rect (100, 70, 100, 20), newMemberEmail);
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
			if(RoomModel.getInstance().CurrentRoom.Name.Trim ('"') != "starting room"){
				if(GUI.Button (new Rect(10, 130, 100, 20), "Delete Room")){
					RoomController.getInstance().deleteRoom();
				}
			}
			GUI.EndGroup();
		}
	}
}
