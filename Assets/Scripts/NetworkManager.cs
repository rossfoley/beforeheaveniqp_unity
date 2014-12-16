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
	public GameObject defaultRoomTemplate;
	public GameObject jazzRoomTemplate;
	public GameObject coolRoomTemplate;
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
			RoomController.getInstance().getRooms("");
			RoomData startingRoom = null;
			foreach (RoomData rd in RoomModel.getInstance().AllRooms){
				if (rd.Name.Trim('"') == "Starting Room"){
					startingRoom = rd;
					break;
				}
			}
			PhotonNetwork.JoinOrCreateRoom ("Starting Room", testRO, PhotonNetwork.lobby);

			LoginModel.CurrentRoomId = startingRoom.RoomId;
			
			RoomModel.getInstance ().CurrentRoom = startingRoom;
			
			LoginController.updateCurrentRoom ();

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
		Debug.Log("Changing room from " + LoginModel.CurrentRoomId + " to " + toRoom.RoomId);

		LoginModel.CurrentRoomId = toRoom.RoomId;
		
		RoomModel.getInstance ().CurrentRoom = toRoom;

		PhotonNetwork.LeaveRoom();

		nextRoom = toRoom.Name;
		
		Destroy (currentRoomObject);

		// Update currentRoomObject and Data
		switch (toRoom.RoomPreset) {
		case (int) RoomController.roomPresets.defaultRoom:
			currentRoomObject = (GameObject) Instantiate(defaultRoomTemplate);
			break;
		case (int) RoomController.roomPresets.jazzRoom:
			currentRoomObject = (GameObject) Instantiate(jazzRoomTemplate);
			break;
		case (int) RoomController.roomPresets.coolRoom:
			currentRoomObject = (GameObject) Instantiate(coolRoomTemplate);
			break;
		default:
			currentRoomObject = (GameObject) Instantiate(defaultRoomTemplate);
			break;
		}

		Debug.Log ("Changing room to " + toRoom.RoomId);

		LoginController.updateCurrentRoom ();
	}

	public void kickAll(){
		photonView.RPC ("kickToStart", PhotonTargets.All, "");
	}

	[RPC] void kickToStart(string username){
		if(username == "" || username == LoginModel.Username.Trim('"')){
			changeRoom(RoomModel.getInstance().getRoom("Starting Room"));
		}
	}

	public void updateAll(string name, string genre){
		object[] parameters = new object[2];
		parameters[0] = name;
		parameters[1] = genre;
		photonView.RPC ("updateCurrentRoom", PhotonTargets.All, parameters);
	}

	[RPC] void updateCurrentRoom(string name, string genre){
		RoomModel.getInstance().CurrentRoom.Name = name;
		RoomModel.getInstance().CurrentRoom.Genre = genre;
	}

	void OnApplicationQuit() { 
		Debug.Log ("Quitting the app");
		LoginController.updateOnlineStatus(false);
	}
}
