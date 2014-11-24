using UnityEngine;
using System.Collections;

public class FriendsListMenu : MonoBehaviour {

	private Rect friendWindowRect = new Rect(100, 100, Screen.width - 200, Screen.height - 200);
	
	private string friendEmail = "";
	private int updateCounter = 0;

	public int guiEdgeBorder = GUIController.GuiEdgeBorder;
	
	public Vector2 scrollPos = Vector2.zero;
	public GUIStyle style;

	void OnGUI () {
		friendWindowRect = GUI.Window (5, friendWindowRect, 
		                                 FriendWindowFunction, "Friends List");
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
		GUILayout.BeginArea (new Rect(0, guiEdgeBorder, 
		                              friendWindowRect.width / 3, friendWindowRect.height));
		
		friendEmail = GUILayout.TextField (friendEmail);
		if (GUILayout.Button ("Add Friend")) {
			LoginController.addFriend(friendEmail);
		}
		
		GUILayout.EndArea();

		// Populates a scroll view with all of the rooms currently in the database
		GUI.skin.scrollView = style;
		if(LoginModel.FriendData.Length > 0) {
			GUILayout.BeginArea (new Rect(friendWindowRect.width / 3, guiEdgeBorder, 
			                              friendWindowRect.width * 2/3, friendWindowRect.height-guiEdgeBorder));

			scrollPos = GUILayout.BeginScrollView (scrollPos);

			for (int i = 0; i < LoginModel.FriendIds.Length; i++) {
				if(GUILayout.Button(LoginModel.FriendData[i].UserEmail)) {
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
			GUILayout.EndScrollView();
			GUILayout.EndArea ();
		}
	}
}