using UnityEngine;
using System.Collections;

public class RoomConfigMenu : MonoBehaviour {

	private RoomData thisRoom;

	// Use this for initialization
	void Start () {
		thisRoom = new RoomData ("Starting Room", "N/A", 0, null);
	}

	public RoomData ThisRoom {
		get {
			return this.thisRoom;
		}
		set {
			thisRoom = value;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		GUI.BeginGroup (new Rect(500, 300, 500, 300));
		GUI.Box (new Rect (0, 0, 500, 300), "Room Management");
		GUI.Label (new Rect (10, 10, 100, 20), thisRoom.Name);
		GUI.Label (new Rect (10, 30, 100, 20), thisRoom.Genre);
		GUI.Label (new Rect (10, 50, 100, 20), thisRoom.Visits.ToString());
		string email = "";
		email = GUI.TextField (new Rect (10, 70, 100, 20), email);
		GUI.Button (new Rect (120, 70, 50, 20), "Submit");
		GUI.EndGroup();
	}
}
