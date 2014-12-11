using UnityEngine;
using System.Collections;

public class BaseRoom : MonoBehaviour {

	private RoomData roomData;

	// Use this for initialization
	void Start () {
	
		roomData = new RoomData ("0", "TestRoom", "TestGenre", 1, null, (int) RoomController.roomPresets.defaultRoom);

		// Increment visits and send to database
		roomData.Visits++;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
