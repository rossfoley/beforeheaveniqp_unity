using UnityEngine;
using System.Collections;
using System.Net;

public class RoomConfigMenu : MonoBehaviour {

	private RoomData thisRoom;
	private string authKey;
	private string userEmail;
	private string newMemberEmail = "";

	private int addMemberStatus;

	// Use this for initialization
	void Start () {
	
	}

	public RoomData ThisRoom {
		get {
			return this.thisRoom;
		}
		set {
			thisRoom = value;
		}
	}

	public string AuthKey {
		get {
			return this.authKey;
		}
		set {
			authKey = value;
		}
	}

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
		authKey = LoginScript.AuthKey;
		userEmail = LoginScript.UserEmail;
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
				addMemberStatus = -1;
				//TODO Error
				Debug.Log("Email not entered");
			}
			// Put request for a new band member
			else {
				addMemberStatus = 1;
				string roomId;

				// TODO get roomId from database
				roomId = thisRoom.RoomId;

				var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/rooms/" + roomId + "/add_band_member/") as System.Net.HttpWebRequest;

				request.KeepAlive = true;
				
				request.Method = "PUT";
				
				request.ContentType = "application/json";
				request.Headers.Add("x-user-email", userEmail);
				request.Headers.Add("x-user-token", authKey);
				byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("{\"new_member_email\": \"" + newMemberEmail + "\"}");
				request.ContentLength = byteArray.Length;
				using (var writer = request.GetRequestStream()){
					writer.Write(byteArray, 0, byteArray.Length);
				}
				string responseContent=null;
				try {
					using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
						using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
							responseContent = reader.ReadToEnd();
							addMemberStatus = 2;
						}
					}
				}
				catch(WebException e){
					//TODO Error
					Debug.Log ("Invalid email entered");
					addMemberStatus = -2;
				}
			}
		}
		string status;
		switch(addMemberStatus){
			case 1: 
				status = "Adding...";
				break;
			case 2:
				status = "Added";
				break;
			case -1:
				status = "Email not entered";
				break;
			case -2:
				status = "Invalid email entered";
				break;
			default:
				status = "";
				break;
		}
		GUI.Label (new Rect (10, 110, 200, 20), status);
		GUI.EndGroup();
	}
}
