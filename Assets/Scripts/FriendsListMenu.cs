using UnityEngine;
using System.Collections;

public class FriendsListMenu : MonoBehaviour {

	private Rect friendWindowRect = new Rect(100, 100, Screen.width - 200, Screen.height - 200);
	
	private string friendEmail = "";
	private int updateCounter = 0;

	public int guiEdgeBorder = GUIController.GuiEdgeBorder;
	
	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;

	void OnGUI () {
		friendWindowRect = GUI.Window (5, friendWindowRect, 
		                                 FriendWindowFunction, "Welcome to the elevator!");
	}

	void FriendWindowFunction (int windowID) {
		createFriendList();
	}

	
	// Creates the friend's list and populates with the current user's friends
	void createFriendList() {
		if (updateCounter == 0) {
			LoginController.getFriends();
			updateCounter = 250;
		}
		else {
			updateCounter--;
		}
		GUILayout.BeginArea (new Rect (0, 0, 600, 500));
		GUI.Label (new Rect(200, 0, 200, 20), "Friends List");
		// Populates a scroll view with all of the rooms currently in the database
		GUI.skin.scrollView = style;
		if(LoginModel.FriendData.Length > 0) {
			scrollPosition = GUI.BeginScrollView (
				new Rect (200, 2 * guiEdgeBorder, 200, 200),
				scrollPosition, 
				new Rect(0, 0, 200, 20*LoginModel.FriendData.Length));
			for (int i = 0; i < LoginModel.FriendIds.Length; i++) {
				if(GUI.Button(new Rect(0, 20*i, 200, 20), LoginModel.FriendData[i].UserEmail)) {
					// TODO yield return?
					Debug.Log ("Button pressed");
					string roomName = LoginController.getCurrentRoomOfUser(LoginModel.FriendData[i].UserId);
					Debug.Log ("Friend room name " + roomName);
					RoomController.getInstance().getRooms("");
					RoomData friendRD = null;
					int j = 0;
					foreach (RoomData rd in RoomModel.getInstance().AllRooms){
						if (rd.Name.Trim ('"').Equals(roomName)){
							friendRD = rd;
							Debug.Log ("friendRD = " + friendRD.RoomId);
							break;
						}
						j++;
					}
					if (friendRD != null){
						NetworkManager.getInstance().changeRoom(friendRD);
					}
				}
			}
			GUI.EndScrollView();
		}
		friendEmail = GUI.TextField (new Rect (200, 220, 200, 20), friendEmail);
		if (GUI.Button (new Rect(400, 220, 100, 20), "Add Friend")) {
			LoginController.addFriend(friendEmail);
		}
		GUILayout.EndArea ();
	}


}