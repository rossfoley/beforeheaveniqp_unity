﻿using UnityEngine;
using System.Collections;

public class FriendsListMenu : MonoBehaviour {

	private Rect friendWindowRect = new Rect(Screen.width/3, 100, Screen.width/3, Screen.height - 200);
	
	private string friendUsername = "";
	private int updateCounter = 0;
	private int statusCounter = 0;

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
		GUILayout.BeginArea (new Rect(5, guiEdgeBorder, 
		                              (friendWindowRect.width / 3)-5, friendWindowRect.height));
		
		friendUsername = GUILayout.TextField (friendUsername);
		if (GUILayout.Button ("Add Friend")) {
			LoginController.addFriend(friendUsername);
			friendUsername = "";
		}
		
		GUILayout.EndArea();

		// Populates a scroll view with all of the rooms currently in the database
		GUI.skin.scrollView = style;
		if(LoginModel.FriendData.Length > 0) {
			GUILayout.BeginArea (new Rect(friendWindowRect.width / 3, guiEdgeBorder, 
			                              friendWindowRect.width * 2/3, friendWindowRect.height-guiEdgeBorder));

			scrollPos = GUILayout.BeginScrollView (scrollPos);

			for (int i = 0; i < LoginModel.FriendData.Length; i++) {
				GUILayout.BeginHorizontal();
				bool isFriendOnline = true;

				if (statusCounter == 0){
					bool isUserOnline = LoginController.isUserOnline(LoginModel.FriendData[i].UserId);
					if (isUserOnline){
						isFriendOnline = isUserOnline;
					}
					else {
						isFriendOnline = isUserOnline;
					}
					statusCounter = 150;
					if (LoginModel.FriendData[i].IsOnline != isFriendOnline){
						LoginModel.FriendData[i].IsOnline = isFriendOnline;
					}
				}
				else {
					statusCounter--;
				}

				if (LoginModel.FriendData[i].IsOnline){
					GUI.color = Color.green;
				}
				else {
					GUI.color = Color.red;
				}

				if(GUILayout.Button(LoginModel.FriendData[i].Username)) {
					// TODO yield return?
					if (LoginModel.FriendData[i].IsOnline){
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

				GUI.color = Color.gray;
				if (GUILayout.Button("x", GUILayout.Width(20))){
					LoginController.removeFriend(LoginModel.FriendData[i].Username); 
				}
				GUILayout.EndHorizontal();

			}

			GUILayout.EndScrollView();
			GUILayout.EndArea ();
		}
	}
}