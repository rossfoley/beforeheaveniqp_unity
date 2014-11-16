using UnityEngine;
using System.Collections;
using System;

public class RoomModel : MonoBehaviour {

	private static RoomModel instance = null;
	private RoomData currentRoom;
	private RoomData[] defaultRooms = new RoomData[4];
	private RoomData[] serverRooms = new RoomData[0];
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
		defaultRooms[0] = new RoomData("DefaultStart", "Starting Room", "N/A", 0, null, true);
		defaultRooms[1] = new RoomData("DefaultRock", "Rock", "Rock", 0, null, true);
		defaultRooms[2] = new RoomData("DefaultJazz", "Jazz", "Jazz", 0, null, true);
		defaultRooms[3] = new RoomData("DefaultPop", "Pop", "Pop", 0, null, true);
		// Creates a dummy currentRoomData
		currentRoom = defaultRooms[0];
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
			ElevatorMenu.AllRooms = AllRooms;
		}
	}

	public RoomData[] AllRooms{
		get {
			allRooms = new RoomData[defaultRooms.Length + serverRooms.Length];
			defaultRooms.CopyTo(allRooms, 0);
			serverRooms.CopyTo(allRooms, defaultRooms.Length);
			return allRooms;
		}
	}

	public bool userIsMember ()
	{
		if(currentRoom.Members != null){
			return Array.IndexOf (currentRoom.Members, LoginModel.UserId) >= 0;
		}
		else{
			return false;
		}
	}
	
	public bool roomHasMembers ()
	{
		return currentRoom.Members.Length != 0;
	}
}
