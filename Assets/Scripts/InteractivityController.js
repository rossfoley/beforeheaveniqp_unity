// Author: Dustin Lombardi

#pragma strict

/*********************************
 *          VARIABLES            *
 *********************************/
 
// All NPCs on map
var npcs : GameObject[];

// Current NPC
var curNpc : GameObject;

// The player
var player : GameObject;

// Set to true if the player is within 3 distance units of an NPC, false otherwise
// Set to true if the player is currently interacting with an NPC, false otherwise
var interacting : boolean;





/*********************************
 *             Start             *
 *********************************/

function Start () {
	//initialize variables
	player = this.gameObject;
	
	npcs = GameObject.FindGameObjectsWithTag("NPC");
	
}





/*********************************
 *            Update             *
 *********************************/
 
 function OnTriggerEnter(other : Collider)
 {
			 if(other.gameObject.CompareTag("NPC"))
			 {
 				curNpc = other.gameObject;
				playerStatus.readyToTalk = true;
			 }
}
function OnTriggerExit(other : Collider)
{
 
 			 if(other.gameObject.CompareTag("NPC"))
			 {				
				playerStatus.readyToTalk = false;
			 }
 }
 
 
function Update () {
	
	// Get the distance from each NPC to the player
	// If an NPC is within 3 of the player, make that NPC the curNpc and set readyToInteract to true

	//Debug.Log (curNpc);
	
	
	// If within 3 distance units of an NPC, get input

		// 'E' is the interact button
		if (playerStatus.readyToTalk && playerStatus.talkingPlayer && !interacting)
		{
			 interacting=true; 
	         curNpc.GetComponent(activityController).StartActivity();
	         DisableMovement();
	         
	         
	    }// If already interacting, use 'E' to progress the interaction
	    else if (Input.GetButtonUp("actionButton1") && playerStatus.readyToTalk && playerStatus.talkingPlayer && interacting)
	    {
	    	//curNpc.GetComponent(Activity).readyForNext = true;
	    }

	// Else if not within 3 distance units, interacting and guiText should be false
	
}





/*********************************
 *       Other Functions         *
 *********************************/
 
// Disable the player's movement and set interacting to true
function DisableMovement ()
{
	focusCamera.toggleMovement();
	focusCamera.switchCameras(curNpc.GetComponent(activityController).focusPoint);
	//player.GetComponent(CharacterMotor).enabled = false;
}

// Enable the player's movement and set interacting to false
function EnableMovement () 
{
	playerStatus.talkingPlayer = false;
	interacting=false;
	focusCamera.toggleMovement();
	//player.GetComponent(CharacterMotor).enabled = true;
}
