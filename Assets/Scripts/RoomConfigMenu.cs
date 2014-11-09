using UnityEngine;
using System.Collections;
using System.Net;

public class RoomConfigMenu : MonoBehaviour {

	private RoomData thisRoom;
	private string authKey;
	private string userEmail;
	private string newMemberEmail = "";

	// Use this for initialization
	void Start () {
	
	}

	// ThisRoom getter and setter
	public RoomData ThisRoom {
		get {
			return this.thisRoom;
		}
		set {
			thisRoom = value;
		}
	}

	// AuthKey getter and setter
	public string AuthKey {
		get {
			return this.authKey;
		}
		set {
			authKey = value;
		}
	}

	// UserEmail getter and setter
	public string UserEmail {
		get {
			return this.userEmail;
		}
		set {
			userEmail = value;
		}
	} 

	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		// Sets up the GUI components of the window
		GUI.BeginGroup (new Rect(500, 300, 500, 300));
		GUI.Box (new Rect (0, 0, 500, 300), "Room Management");
		GUI.Label (new Rect (10, 10, 100, 20), thisRoom.Name);
		GUI.Label (new Rect (10, 30, 100, 20), thisRoom.Genre);
		GUI.Label (new Rect (10, 50, 100, 20), thisRoom.Visits.ToString());
		GUI.Label (new Rect (10, 70, 100, 20), "New member email");
		GUI.SetNextControlName ("email field");
		newMemberEmail = GUI.TextField (new Rect (100, 70, 100, 20), newMemberEmail);
		if (GUI.Button (new Rect (30, 90, 50, 20), "Submit") || 
		    (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "email field")){
			// If no email is entered, do not go through with the request
			if (newMemberEmail.Trim() == ""){
				//TODO Error
				Debug.Log("Email not entered");
			}
			// Put request for a new band member
			else {
				string roomId;

				// Get roomId from database
				roomId = thisRoom.RoomId;

				// Create the put request for adding the new band member
				var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/room/" + roomId + "/add_band_member/" + newMemberEmail) as System.Net.HttpWebRequest;

				request.KeepAlive = true;
				
				request.Method = "PUT";
				
				request.ContentType = "application/json";
				request.Headers.Add("x-user-email", userEmail);
				request.Headers.Add("x-user-token", authKey);
				request.ContentLength = 0;
				string responseContent=null;
				try {
					using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
						using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
							responseContent = reader.ReadToEnd();
						}
					}
				}
				// If a WebException is caught, display an error message
				catch(WebException e){
					//TODO Error message
					Debug.Log ("Invalid email entered");
				}
			}
		}
		GUI.EndGroup();
	}
}
