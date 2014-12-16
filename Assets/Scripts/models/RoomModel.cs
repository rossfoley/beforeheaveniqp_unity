using UnityEngine;
using System.Collections;
using System;

public class RoomModel : MonoBehaviour {

	private static RoomModel instance = null;
	private RoomData currentRoom;
	private RoomData[] allRooms = new RoomData[0];
	private GameObject currentPhysicalRoom;

	public static RoomModel getInstance(){
		if(instance == null){
			instance = new RoomModel();
		}
		return instance;
	}

	// Use this for initialization
	void Start () {
		instance = this;
		// Creates a dummy currentRoomData
		currentRoom = new RoomData ("0", "dummy", "none", 0, null, (int) RoomController.roomPresets.defaultRoom);
	}

	public RoomData getRoom (string name){
		foreach(RoomData room in allRooms){
			if(room.Name.Trim ('"') == name){
				return room;
			}
		}
		return null;
	}

	public RoomData CurrentRoom {
		get {
			return this.currentRoom;
		}
		set {
			currentRoom = value;
			RoomConfigMenu.ThisRoom = currentRoom;
			RoomConfigMenu.UserIsMember = userIsMember();
			ElevatorMenu.CurrentRoom = currentRoom;
		}
	}

	public RoomData[] AllRooms {
		get {
			return this.allRooms;
		}
		set {
			allRooms = value;
			ElevatorMenu.AllRooms = allRooms;
		}
	}

	public bool userIsMember ()
	{
		return Array.IndexOf (currentRoom.Members, LoginModel.UserId) >= 0;
	}
	
	public bool roomHasMembers ()
	{
		return currentRoom.Members.Length != 0;
	}
}
