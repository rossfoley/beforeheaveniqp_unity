using UnityEngine;
using System.Collections;

public class RoomData{

	private string roomId;
	private string name;
	private string genre;
	private int visits;
	private string[] members;
	private bool defaultRoom;

	public RoomData(string roomId, string name, string genre, int visits, string[] members, bool defaultRoom){
		this.roomId = roomId;
		this.name = name;
		this.genre = genre;
		this.visits = visits;
		this.members = members;
		this.defaultRoom = defaultRoom;
	}

	// RoomId getter and setter
	public string RoomId {
		get {
			return this.roomId;
		}
		set {
			roomId = value;
		}
	}

	// Name getter and setter
	public string Name {
		get {
			return this.name;
		}
		set {
			name = value;
		}
	}

	// Genre getter and setter
	public string Genre {
		get {
			return this.genre;
		}
		set {
			genre = value;
		}
	}

	// Visits getter and setter
	public int Visits {
		get {
			return this.visits;
		}
		set {
			visits = value;
		}
	}

	// Members getter and setter
	public string[] Members {
		get {
			return this.members;
		}
		set {
			members = value;
		}
	}

	public bool DefaultRoom {
		get {
			return this.defaultRoom;
		}
		set {
			defaultRoom = value;
		}
	}
}