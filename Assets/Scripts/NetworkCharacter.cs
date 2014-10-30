using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
						//do nothing
		} else {
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
		}
	}

	void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			// this is OUR player. we need to send our actual position to the network

			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		} else {
			// this is someone else's player. we need to recieve their position (as of a few
			// millisecondsd ago, and update our version of that player

			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
		}
	}
}
