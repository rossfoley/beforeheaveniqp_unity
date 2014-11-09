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
		newMemberEmail = GUI.TextField (new Rect (100, 70, 100, 20), newMemberEmail);
		if (GUI.Button (new Rect (30, 90, 50, 20), "Submit")){
			// If no email is entered, do not go through with the request
			if (newMemberEmail.Trim() == ""){
				Debug.Log("Email not entered");
			}
			// Put request for a new band member
			else {
				string roomId;

				// TODO get roomId from database
				roomId = thisRoom.RoomId;

				var request = System.Net.WebRequest.Create("http://beforeheaveniqp.herokuapp.com/api/rooms/" + roomId + "/add_band_member/") as System.Net.HttpWebRequest;

				Debug.Log ("http://beforeheaveniqp.herokuapp.com/api/rooms/" + roomId + "/add_band_member/");

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
				Debug.Log (newMemberEmail);
				try {
					using (var response = request.GetResponse() as System.Net.HttpWebResponse) {
						using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
							responseContent = reader.ReadToEnd();
						}
					}
				}
				catch(WebException e){
					Debug.Log ("Invalid email entered");
				}
			}
		}
		GUI.EndGroup();
	}
}
