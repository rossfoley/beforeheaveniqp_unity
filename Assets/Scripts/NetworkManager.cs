using UnityEngine;
using System.Collections;
using System.Linq;

public class NetworkManager : MonoBehaviour {

	public GameObject StandbyCamera;
	SpawnSpot[] spawnSpots;

	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString()); // show connectivity status
	}

	// Use this for initialization
	void Start () {
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();

		Connect (); // connect to photon stuff
	}

	void Connect () {
		PhotonNetwork.ConnectUsingSettings ("BefoHev V001");
	}

	void OnJoinedLobby() {
		PhotonNetwork.JoinRandomRoom ();
	}

	void OnPhotonRandomJoinFailed() {
		Debug.Log ("OnPhotonRandomJoinFailed");
		PhotonNetwork.CreateRoom (null);
	}

	void OnJoinedRoom() {
		Debug.Log ("joined Room");

		SpawnMyPlayer ();
	}

	void SpawnMyPlayer () {
		if (spawnSpots == null) {
			Debug.Log ("no spawn spots in level");
			return;
		}

		SpawnSpot mySpawnSpot = spawnSpots [Random.Range (0, spawnSpots.Length)];

		//StandbyCamera.SetActive(false);
		GameObject myPlayerGO = PhotonNetwork.Instantiate ("WC", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);


		((MonoBehaviour) myPlayerGO.GetComponent ("ThirdPersonController")).enabled = true;
		((MonoBehaviour) myPlayerGO.GetComponent ("ThirdPersonCamera")).enabled = true;

		//((MonoBehaviour) myPlayerGO.GetComponent ("FPSInputController")).enabled = true;
		//((MonoBehaviour) myPlayerGO.GetComponent ("MouseLook")).enabled = true;
		//((MonoBehaviour) myPlayerGO.GetComponent ("CharacterMotor")).enabled = true;

		Debug.Log ("stuff happened");

		//myPlayerGO.transform.FindChild ("Main Camera").gameObject.SetActive (true);

	}

	// Update is called once per frame
	void Update () {
	
	}
}
