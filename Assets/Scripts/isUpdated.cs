using UnityEngine;
using System.Collections;

public class isUpdated : Photon.MonoBehaviour {

	bool visibleInRoom = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("jumpButton")) {
			updateObjectStatus(false);
		}
	}

	public void updateObjectStatus (bool isInCurrentRoom)  {
		if (isInCurrentRoom) {

		} else {
			photonView.RPC("removeFromRoom", PhotonTargets.All, (byte)1);
		}
	}

	[RPC] void removeFromRoom(byte num) {
		MeshRenderer graphicsMesh = GetComponentInChildren<MeshRenderer>();

		visibleInRoom = !visibleInRoom;
		graphicsMesh.enabled = visibleInRoom;
	}
}
