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
		photonView.RPC("removeFromRoom", PhotonTargets.All, !visibleInRoom);
	}

	[RPC] void removeFromRoom(bool inRoom) {
		MeshRenderer graphicsMesh = GetComponentInChildren<MeshRenderer>();
		visibleInRoom = inRoom;
		graphicsMesh.enabled = visibleInRoom;
	}
}
