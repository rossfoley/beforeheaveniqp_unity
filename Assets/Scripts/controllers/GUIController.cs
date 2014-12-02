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

	private bool elMenuActive = false;
	private bool crWindowActive = false;
	private bool flWindowActive = false;

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

		if (!elWindowVisible && elMenuActive){
			elevatorMenu.SetActive(false);
			elMenuActive = false;
		}
		if (!crWindowVisible && crWindowActive){
			roomConfigMenu.SetActive(false);
			crWindowActive = false;
		}
		if (!flWindowVisible && flWindowActive){
			friendsListMenu.SetActive(true);
			flWindowActive = false;
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
			// If the elevator window is visible, create the GUI window
			if (elWindowVisible) {
				elevatorMenu.SetActive(true);
				elMenuActive = true;
				if (crWindowVisible || flWindowVisible){
					crWindowVisible = false;
					flWindowVisible = false;
					roomConfigMenu.SetActive(false);
					friendsListMenu.SetActive(false);
					crWindowActive = false;
					flWindowActive = false;
				}
				Debug.Log("Activating elevator menu");
			}
			else {
				elevatorMenu.SetActive(false);
				elMenuActive = false;
			}
		}

		// Room Edit / Room Create top-level GUI button
		// TODO: Make the button change on whether or not user owns room
		if (GUILayout.Button ("Create Room")) {
			crWindowVisible = !crWindowVisible;
			// If the create room window is visible, create + display all the GUI elements of the window
			if (crWindowVisible) {
				roomConfigMenu.SetActive(true);
				crWindowActive = true;
				if (elWindowVisible || flWindowVisible){
					elWindowVisible = false;
					flWindowVisible = false;
					elevatorMenu.SetActive(false);
					friendsListMenu.SetActive(false);
					elMenuActive = false;
					flWindowActive = false;
				}
			}
			else {
				roomConfigMenu.SetActive(false);
				crWindowActive = false;
			}
		}

		// Friend list top-level GUI button
		if (GUILayout.Button ("Friends List")) {
			flWindowVisible = !flWindowVisible;
			// If the friends list window is visible, create the GUI window
			if (flWindowVisible) {
				friendsListMenu.SetActive(true);
				flWindowActive = true;
				if (crWindowVisible || elWindowVisible){
					crWindowVisible = false;
					elWindowVisible = false;
					crWindowActive = false;
					elMenuActive = false;
					roomConfigMenu.SetActive(false);
					elevatorMenu.SetActive(false);
				}
				Debug.Log("Activating friends list");
			}
			else {
				friendsListMenu.SetActive(false);
				flWindowActive = false;
			}
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
