using UnityEngine;
using System.Collections;
using System.Linq;

public class NetworkManager : MonoBehaviour {

	//Unity GameObject Links
	public GameObject StandbyCamera;
	public GameObject myPlayerGO;
	public static GameObject currentRoomObject;
	public static GameObject roomTemplate;
	private SpawnSpot[] spawnSpots;

	private bool isStartup;
	private static string nextRoom;

	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString()); // show connectivity status
	}

	// Use this for initialization
	void Start () {
		PhotonNetwork.autoJoinLobby = true;
		isStartup = true;
		Connect (); // connect to photon stuff
	}

	void Connect () {
		PhotonNetwork.ConnectUsingSettings ("BefoHev V001");
	}

	// When the lobby is joined and it is in startUp, join the starting room, else join the room being changed to
	void OnJoinedLobby() {
		RoomOptions testRO = new RoomOptions();
		if (isStartup){
			PhotonNetwork.JoinOrCreateRoom ("starting room", testRO, PhotonNetwork.lobby);
			isStartup = false;
		}
		else{
			// Join the room if it is already active on the server, otherwise create it
			PhotonNetwork.JoinOrCreateRoom (nextRoom.Trim('"'), testRO, PhotonNetwork.lobby);
		}
	}

	// When a room is joined, spawn the player
	void OnJoinedRoom() {
		SpawnMyPlayer ();
	}

	// Spawns the player character in one of the spawn spots
	public void SpawnMyPlayer () {
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
		if (spawnSpots == null) {
			Debug.Log ("no spawn spots in level");
			return;
		}

		SpawnSpot mySpawnSpot = spawnSpots [Random.Range (0, spawnSpots.Length)];
		
		//StandbyCamera.SetActive(false);
		myPlayerGO = PhotonNetwork.Instantiate ("WC", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);

		((MonoBehaviour) myPlayerGO.GetComponent ("ThirdPersonController")).enabled = true;
		((MonoBehaviour) myPlayerGO.GetComponent ("ThirdPersonCamera")).enabled = true;
		
	}
	
	public static void changeRoom (int i)
	{
		PhotonNetwork.LeaveRoom();
		
		nextRoom = RoomModel.getInstance().getRoom(i).Name;
		
		Destroy (currentRoomObject);
		
		// Update currentRoomObject and Data
		currentRoomObject = (GameObject) Instantiate(roomTemplate);
		RoomModel.getInstance ().CurrentRoom = RoomModel.getInstance ().AllRooms [i];
	}
}
