using UnityEngine;
using System.Collections;
using System.Linq;

public class NetworkManager : MonoBehaviour {

	public GameObject StandbyCamera;
	private SpawnSpot[] spawnSpots;
	public GameObject myPlayerGO;
	private bool isStartup;
	public bool	inLobby;

	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString()); // show connectivity status
	}

	// Use this for initialization
	void Start () {
		PhotonNetwork.autoJoinLobby = true;
		inLobby = false;
		isStartup = true;
		Connect (); // connect to photon stuff
	}

	void Connect () {
		PhotonNetwork.ConnectUsingSettings ("BefoHev V001");
	}

	// When the lobby is joined and it is in startUp, join the starting room
	void OnJoinedLobby() {
		Debug.Log ("OnJoinedLobby Called");
		inLobby = true;
		if (isStartup){
			RoomOptions testRO = new RoomOptions();
			PhotonNetwork.JoinOrCreateRoom ("starting room", testRO, PhotonNetwork.lobby);
			isStartup = false;
		}
	}

	// When a room is joined, spawn the player
	void OnJoinedRoom() {
		Debug.Log ("joined Room");
		inLobby = false;
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

	// Update is called once per frame
	void Update () {
		
	}

}
