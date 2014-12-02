using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour {

	// Game objects controlled by top-level buttons
	public GameObject elevatorMenu;
	public GameObject roomConfigMenu;
	public GameObject friendsListMenu;

	// Visibility of elements changes when selecting their buttons
	private static bool elWindowVisible = false; // Elevator menu
	private static bool crWindowVisible = false; // Room config menu
	private static bool flWindowVisible = false; // Friends list menu

	private Rect topBarRect = new Rect (guiEdgeBorder, guiEdgeBorder, 600, 60);

	// Super awesome variable for distance between edges of things
	private static int guiEdgeBorder = 20;

	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;

	// Getters and setters
	public static bool ElWindowVisible {
		get { return elWindowVisible; }
		set { elWindowVisible = value; }
	}

	public static bool CrWindowVisible {
		get { return crWindowVisible; }
		set { crWindowVisible = value; }
	}

	public static bool FlWindowVisible {
		get { return flWindowVisible; }
		set { flWindowVisible = value; }
	}

	public static int GuiEdgeBorder {
		get { return guiEdgeBorder; }
		set { guiEdgeBorder = value; }
	}

	// Creates the top-level GUI
	void OnGUI() {

		// If the elevator window is visible, create the GUI window
		if (elWindowVisible) {
			elevatorMenu.SetActive(true);
		}
		else {
			elevatorMenu.SetActive(false);
		}
		
		// If the create room window is visible, create + display all the GUI elements of the window
		if (crWindowVisible) {
			roomConfigMenu.SetActive(true);
		}
		else {
			roomConfigMenu.SetActive(false);
		}

		// If the friends list window is visible, create the GUI window
		if (flWindowVisible) {
			friendsListMenu.SetActive(true);
		}
		else {
			friendsListMenu.SetActive(false);
		}
		
		GUILayout.BeginArea (topBarRect);
		GUILayout.BeginHorizontal();
		
		// Elevator top-level GUI button
		if (GUILayout.Button ("Elevator")) {
			// If the elevator window is going to appear, update allRooms by getting all the rooms with no search string
			if (!elWindowVisible) {
				StartCoroutine(RoomController.getInstance().getRooms(""));
			}
			elWindowVisible = !elWindowVisible;
		}

		// Room Edit / Room Create top-level GUI button
		// TODO: Make the button change on whether or not user owns room
		if (GUILayout.Button ("Create Room")) {
			crWindowVisible = !crWindowVisible;
		}

		// Friend list top-level GUI button
		if (GUILayout.Button ("Friends List")) {
			flWindowVisible = !flWindowVisible;
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
