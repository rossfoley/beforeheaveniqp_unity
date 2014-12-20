using UnityEngine;
using System.Collections;

public class RoomData{

	private string roomId;
	private string name;
	private string genre;
	private string playlistId;
	private int visits;
	private string[] members;
	private int roomPreset;
	
	public RoomData(string roomId, string name, string genre, string playlistId, int visits, string[] members, int roomPreset){
		this.roomId = roomId;
		this.name = name;
		this.genre = genre;
		this.visits = visits;
		this.members = members;
		this.playlistId = playlistId;
		this.roomPreset = roomPreset;
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
	public int RoomPreset {
		get {
			return this.roomPreset;
		}
		set {
			roomPreset = value;
		}
	}

	public string PlaylistId {
		get {
			return this.playlistId;
		}
		set {
			playlistId = value;
		}
	}

}