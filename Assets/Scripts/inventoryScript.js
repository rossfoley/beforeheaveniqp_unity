#pragma strict
import System.Collections.Generic;
var menuLength : int = 0;
var menuStart : int = 0;
var currentMenu : int = 0;
var menuLengths : List.<int> = new List.<int>();
var menuStarts : List.<int> = new List.<int>();
var menuNames : List.<String> = new List.<String>();
var titleText : String ="Main Menu";
function Start ()
{
menuNames.Add("Resume Game");
menuNames.Add("Instruments");
menuNames.Add("Items");
menuNames.Add("Clothing");
menuNames.Add("Log Out");
}

function Update () 
{


}
function OnGUI()
{
	GUI.depth=0;
	GUI.Label(Rect(Screen.width/2, 100, 100, 100), titleText);

	for(var i=menuStart;i<menuStart+menuLength; i++)
	{
		if(Rect(50, 50+(i*30), 200, 25).Contains(Event.current.mousePosition))
		{
			if(Event.current.type == EventType.MouseDown)
			{
			Debug.Log(menuNames[i] + " was pushed down...");
			}
			if(Event.current.type == EventType.MouseUp)
			{
			Debug.Log(menuNames[i] + " was released...");
			titleText=menuNames[i];
			}
		}	
	
	
		GUI.Button(Rect(50, 50+(i*30), 200, 25), menuNames[i]);
	}
	
	//generate rectangles for menu items
	
		
}