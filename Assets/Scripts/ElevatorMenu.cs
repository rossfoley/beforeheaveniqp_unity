using UnityEngine;
using System.Collections;
using SimpleJSON;

public class ElevatorMenu : MonoBehaviour {
	
	public GameObject currentRoom;
	public GameObject roomTemplate;
	public NetworkManager networkManager;
	private bool isChangingRoom;
	private RoomData[] allRooms = new RoomData[0];

	private const string loginURL = "http://beforeheaveniqp.herokuapp.com/api/user/login";
	private const string roomsURL = "http://beforeheaveniqp.herokuapp.com/api/rooms";

	private string nextRoom;

	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;

	// Use this for initialization
	IEnumerator Start () {
		isChangingRoom = false;

		WWWForm loginForm = new WWWForm ();
		loginForm.AddField ("email", "awhan@wpi.edu");
		loginForm.AddField ("password", "hiandy257");
		WWW login = new WWW(loginURL, loginForm);
		yield return login;
		var parsed = JSON.Parse (login.text);

		var headers = new Hashtable();
		headers.Add ("Content-Type", "application/json");
		headers.Add("X-User-Email", (parsed ["data"] ["email"]).ToString().Trim('"'));
		headers.Add("X-User-Token", (parsed ["data"] ["authentication_token"]).ToString().Trim('"'));
		WWW rooms = new WWW (roomsURL, null, headers);
		yield return rooms;
		Debug.Log (rooms.text);
		var roomsParsed = JSON.Parse (rooms.text);
		allRooms = new RoomData[roomsParsed["data"].AsArray.Count];
		int roomCount = 0;
		foreach(JSONNode data in roomsParsed["data"].AsArray){
			RoomData roomData = new RoomData(data["name"].ToString(), data["genre"].ToString(), data["visits"].AsInt, null);
			allRooms[roomCount] = roomData;
			roomCount++;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Display the buttons for the elevator
	void OnGUI() {
		// Make a background box
		GUI.Box(new Rect(10,10,100,90), "Loader Menu");

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(20,40,80,20), "Join Room")) {
			isChangingRoom = true;
			PhotonNetwork.LeaveRoom();

			Destroy (currentRoom);
			//Destroy (networkManager.myPlayerGO);

			currentRoom = (GameObject) Instantiate(roomTemplate);
		}

		GUI.skin.scrollView = style;
		if(allRooms.Length > 0){
			scrollPosition = GUI.BeginScrollView(new Rect(200, 200, 220, 100), scrollPosition, new Rect(0, 0, 220, 20*allRooms.Length));
			for (int i = 0; i < allRooms.Length; i++){
				if(GUI.Button(new Rect(0, 20*i, 220, 20), allRooms[i].Name)){
					isChangingRoom = true;
					PhotonNetwork.LeaveRoom();

					nextRoom = allRooms[i].Name;

					Destroy (currentRoom);
					//Destroy (networkManager.myPlayerGO);
					
					currentRoom = (GameObject) Instantiate(roomTemplate);
				}
			}
			GUI.EndScrollView();
		}

	}

	void Connect () {
		PhotonNetwork.ConnectUsingSettings ("BefoHev V001");
	}
	
	void OnJoinedLobby() {
		RoomOptions testRO = new RoomOptions ();
		
		// Join the room if it is already active on the server, otherwise create it
		if (isChangingRoom){
			PhotonNetwork.JoinOrCreateRoom (nextRoom, testRO, PhotonNetwork.lobby);
			isChangingRoom = false;
		}
	}
}
