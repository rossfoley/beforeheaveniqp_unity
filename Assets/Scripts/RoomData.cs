using UnityEngine;
using System.Collections;

public class RoomData{
	private string name;
	private string genre;
	private int visits;
	private int[] members;

	public RoomData(string name, string genre, int visits, int[] members){
		this.name = name;
		this.genre = genre;
		this.visits = visits;
		this.members = members;
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

	public int[] Members {
		get {
			return this.members;
		}
		set {
			members = value;
		}
	}

}