using UnityEngine;
using System.Collections;

public class isUpdated : MonoBehaviour {

	bool isInCurrentRoom;

	// Use this for initialization
	void Start () {
		isInCurrentRoom = true;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void updateObjectStatus( bool inRoom) {
		isInCurrentRoom = inRoom;
		if (isInCurrentRoom) {

		} else {
			removeFromRoom ();
		}
	}

	[RPC]
	void removeFromRoom() {
		Destroy ( gameObject);
	}
}
