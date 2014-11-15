using UnityEngine;
using System.Collections;
using System.Linq;

public class NetworkManager : Photon.MonoBehaviour {

	public static NetworkManager instance;

	public static NetworkManager getInstance(){
		if(instance == null){
			instance = new NetworkManager();
		}
		return instance;
	}

	//Unity GameObject Links
	public GameObject StandbyCamera;
	public GameObject currentRoomObject;
	public GameObject roomTemplate;
	private SpawnSpot[] spawnSpots;
	private GameObject myPlayerGO;

	private bool isStartup;
	private string nextRoom;

	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString()); // show connectivity status
	}

	// Use this for initialization
	void Start () {
		instance = this;
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
	
	public void changeRoom (RoomData toRoom)
	{
		PhotonNetwork.LeaveRoom();
		
		nextRoom = toRoom.Name;
		
		Destroy (currentRoomObject);
		
		// Update currentRoomObject and Data
		currentRoomObject = (GameObject) Instantiate(roomTemplate);
		RoomModel.getInstance ().CurrentRoom = toRoom;
	}

	public void kickAll(){
		photonView.RPC ("kickToStart", PhotonTargets.All, "");
	}

	[RPC] void kickToStart(string email){
		if(email == "" || email == LoginModel.UserEmail.Trim('"')){
			changeRoom(RoomModel.getInstance().getRoom("starting room"));
		}
	}
}
