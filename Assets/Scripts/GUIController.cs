using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour {
	
	public GameObject elevatorMenu;
	public GameObject roomConfigMenu;

	private static bool elWindowVisible = false;
	private static bool crWindowVisible = false;
	private Rect elevatorWindowRect = new Rect(50, 50, Screen.width - 100, Screen.height - 100);
	private static int guiEdgeBorder = 20;

	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;

	public static bool ElWindowVisible {
		get { return elWindowVisible; }
		set { elWindowVisible = value; }
	}

	public static bool CrWindowVisible {
		get { return crWindowVisible; }
		set { crWindowVisible = value; }
	}

	public static int GuiEdgeBorder {
		get { return guiEdgeBorder; }
		set { guiEdgeBorder = value; }
	}

	// Creates the top-level GUI
	void OnGUI() {
		
		GUILayout.BeginArea (new Rect (guiEdgeBorder, guiEdgeBorder, 400, 60));
		
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
		
		GUILayout.BeginHorizontal();
		
		// When the elevator button is clicked, switch the visibility of the elevator menu
		if (GUILayout.Button ("Elevator")) {
			// If the elevator window is going to appear, update allRooms by getting all the rooms with no search string
			if (!elWindowVisible) {
				StartCoroutine(RoomController.getInstance().getRooms(""));
			}
			elWindowVisible = !elWindowVisible;
		}
		// Create Room button
		if (GUILayout.Button ("Create Room")) {
			crWindowVisible = !crWindowVisible;
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
