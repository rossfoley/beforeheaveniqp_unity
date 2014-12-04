using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class InRoomChatGUI : Photon.MonoBehaviour 
{
	public Rect GuiRect;
	public Rect chatButtonRect;
	public bool IsVisible = true;
	public bool chatMinimized = false;
	public List<string> messages = new List<string>();
	
	private int MAXLINES = 1000;
	private int BUFFER = 20;
	private int minimizeButtonWidth = 20;
	private string inputLine = "";
	private Vector2 scrollPos = Vector2.zero;
	
	public static readonly string ChatRPC = "Chat";
	
	public void Start()
	{
		this.GuiRect = new Rect(Screen.width / 3*2, Screen.height / 3*2, 
		                        Screen.width/3, Screen.height/3);
		this.chatButtonRect = new Rect(0, 0, 200, 20);
		this.chatButtonRect.x = Screen.width - chatButtonRect.width - BUFFER;
		this.chatButtonRect.y = Screen.height - chatButtonRect.height - BUFFER;
	}
	
	public void OnGUI()
	{
		if (!chatMinimized) {
			GUI.Window(2, this.GuiRect, ChatWindowFunction, "Chat");
		}
		else {
			GUILayout.BeginArea(chatButtonRect);
			if (GUILayout.Button("Chat")) {
				chatMinimized = false;
			}
			GUILayout.EndArea();
		}
	}
	
	public void ChatWindowFunction(int windowID) {
		if (!this.IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined)
		{
			return;
		}
		
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
		{
			if (!string.IsNullOrEmpty(this.inputLine))
			{
				this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
				this.inputLine = "";
				GUI.FocusControl("ChatInput");
				return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
			}
			else
			{
				GUI.FocusControl("ChatInput");
			}
		}
		
		GUI.SetNextControlName("");
		GUILayout.BeginArea(new Rect(0, 0, GuiRect.width, GuiRect.height));
		
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		
		GUILayout.Space(GuiRect.width - minimizeButtonWidth);
		
		if (GUILayout.Button("-", GUILayout.Width(minimizeButtonWidth))) {
			chatMinimized = true;
		}
		
		GUILayout.EndHorizontal();
		
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.FlexibleSpace();
		for (int i = 0; i < messages.Count; i++)
		{
			GUILayout.Label(messages[i]);
		}
		GUILayout.EndScrollView();
		
		GUILayout.EndVertical();
		
		GUILayout.BeginHorizontal();
		GUI.SetNextControlName("ChatInput");
		inputLine = GUILayout.TextField(inputLine);
		if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
		{
			if (!string.IsNullOrEmpty(this.inputLine)) {
				this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
				this.inputLine = "";
				GUI.FocusControl("ChatInput");
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	[RPC]
	public void Chat(string newLine, PhotonMessageInfo mi)
	{
		string senderName = LoginModel.UserEmail;
		
		if (mi != null && mi.sender != null)
		{
			if (!string.IsNullOrEmpty(mi.sender.name))
			{
				senderName = mi.sender.name;
			}
			else
			{
				senderName = LoginModel.UserEmail;
			}
		}
		
		this.messages.Add(senderName +": " + newLine);
		scrollPos.y = 1000000;
		
		// If there are more than MAXLINES messages, remove the oldest message
		while (messages.Count > MAXLINES) {
			messages.RemoveAt (0);
		}
	}
	
	public void AddLine(string newLine)
	{
		this.messages.Add(newLine);
	}

	private void OnJoinedRoom(){
		messages = new List<string>();
	}
}
