using UnityEngine;
using System.Collections;

public class BaseRoom : MonoBehaviour {

	private string name;
	private string genre;
	private int visits;
	private int[] members;

	// Use this for initialization
	void Start () {

		// Grab the room data from the database
		name = "TestRoom";
		genre = "TestGenre";
		int visits = 1;
		members = new int[10];
		members [0] = 5;

		// Increment visits and send to database
		visits++;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
