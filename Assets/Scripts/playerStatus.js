#pragma strict
public static var playerID : int;
public static var playerName : String;
public static var playerGold : int;

var inventoryScreen : GameObject;
var pauseClip : AudioClip;
public var successClip : AudioClip;
public static var pausedGame : boolean = false;
public static var playerCanPlay : boolean = false;
public static var playerCanMove : boolean = false;


public static var listeningSong : boolean = false;
public static var playingSong : boolean = false;
public static var recordingSong : boolean = false;
public static var talkingPlayer : boolean = false;
public static var shoppingPlayer : boolean = false;
public static var composingPlayer : boolean = false;
public static var watching : boolean = false;


public static var readyToListen : boolean = false;
public static var readyToPlay : boolean = false;
public static var readyToRecord : boolean = false;
public static var readyToTalk : boolean = false;
public static var readyToWatch : boolean = false;
public static var readyToShop : boolean = false;
public static var readyToCompose : boolean = false;

function Start () 
{

}

function Update () 
{
	if(Input.GetButtonUp("pauseButton"))
	{
		pauseGame();
	}
	if(Input.GetButtonUp("actionButton1") && !isAnythingRunning() && !pausedGame)
	{
		if(readyToTalk)
		{
		talkingPlayer=true;
		}
		else if(readyToShop)
		{
		shoppingPlayer=true;
		}
		else if(readyToListen)
		{
		listeningSong=true;
		}
		else if(readyToPlay)
		{
		playingSong=true;
		}
		else if(readyToWatch)
		{
		watching=true;
		}
		else if(readyToRecord)
		{
		recordingSong=true;
		}
		else if(readyToCompose)
		{
		composingPlayer=true;
		}
	}
	//Debug.Log("Listening: " + listeningSong + " Playing: " + playingSong + " Recording: " + recordingSong + " Talking: " + talkingPlayer + " Shopping: " + shoppingPlayer);
}
function isAnythingRunning()
{
	if(listeningSong || playingSong || watching || recordingSong || talkingPlayer || shoppingPlayer || composingPlayer)
	return true;
	else
	return false;
}
function playSuccess()
{
	audio.PlayOneShot(successClip);
}
function pauseGame()
{
		if(pausedGame)
		{
		pausedGame=!pausedGame;
		//focusCamera.toggleMovement();
		Time.timeScale=1.0;
		inventoryScreen.SetActive(false);
		}
		else
		{
		pausedGame=!pausedGame;
		inventoryScreen.SetActive(true);
		audio.PlayOneShot(pauseClip);
		Time.timeScale=0.0;
		//focusCamera.toggleMovement();
		}
}
	