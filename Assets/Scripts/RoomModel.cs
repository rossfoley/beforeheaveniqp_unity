using UnityEngine;
using System.Collections;
using System;

public class RoomModel : MonoBehaviour {

	private static RoomModel instance = null;
	private RoomData currentRoom;
	private RoomData[] defaultRooms = new RoomData[1];
	private RoomData[] serverRooms = new RoomData[0];
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
		defaultRooms[0] = new RoomData("DefaultStart", "starting room", "N/A", 0, null);
		// Creates a dummy currentRoomData
		currentRoom = new RoomData ("0", "dummy", "none", 0, null);
	}

	public RoomData getRoom (string name){
		foreach(RoomData room in serverRooms){
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

	public RoomData[] ServerRooms {
		get {
			return this.serverRooms;
		}
		set {
			serverRooms = value;
			ElevatorMenu.AllRooms = defaultRooms + serverRooms;
		}
	}

	public RoomData[] AllRooms{
		get {
			return this.defaultRooms + this.serverRooms;
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
