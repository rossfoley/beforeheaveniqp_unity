using UnityEngine;
using System.Collections;

public class ElevatorMenu : MonoBehaviour {

	private BaseRoom[] allRooms;
	public GameObject currentRoom;
	public GameObject roomTemplate;
	public NetworkManager networkManager;
	private bool isChangingRoom;

	// Use this for initialization
	void Start () {
		isChangingRoom = false;
		Connect ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Display the buttons for the elevator
	void OnGUI() {
		// Make a background box
		GUI.Box(new Rect(10,10,100,90), "Loader Menu");

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(20,40,80,20), "Join Room")) {
			isChangingRoom = true;
			PhotonNetwork.LeaveRoom();

			Destroy (currentRoom);
			//Destroy (networkManager.myPlayerGO);

			currentRoom = (GameObject) Instantiate(roomTemplate);
		}
	}

	void Connect () {
		PhotonNetwork.ConnectUsingSettings ("BefoHev V001");
	}
	
	void OnJoinedLobby() {
		Debug.Log ("BLAHBLAHBLAHBLAHBLAHB");
		RoomOptions testRO = new RoomOptions ();
		
		// Join the room if it is already active on the server, otherwise create it
		if (isChangingRoom){
			PhotonNetwork.JoinOrCreateRoom ("name", testRO, PhotonNetwork.lobby);
			isChangingRoom = false;
		}
	}
}
