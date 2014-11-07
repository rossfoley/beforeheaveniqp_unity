using UnityEngine;
using System.Collections;

public class RoomData{

	private string roomId;
	private string name;
	private string genre;
	private int visits;
	private string[] members;

	public RoomData(string roomId, string name, string genre, int visits, string[] members){
		this.roomId = roomId;
		this.name = name;
		this.genre = genre;
		this.visits = visits;
		this.members = members;
	}

	public string RoomId {
		get {
			return this.roomId;
		}
		set {
			roomId = value;
		}
	}

	public string Name {
		get {
			return this.name;
		}
		set {
			name = value;
		}
	}

	public string Genre {
		get {
			return this.genre;
		}
		set {
			genre = value;
		}
	}

	public int Visits {
		get {
			return this.visits;
		}
		set {
			visits = value;
		}
	}

	public string[] Members {
		get {
			return this.members;
		}
		set {
			members = value;
		}
	}

}