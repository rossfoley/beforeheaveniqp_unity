#pragma strict

var chirpingSound : AudioClip;
internal var inputString : String = "";
var birdView : Transform;
var birdObject : GameObject;
var birdDestination : Transform;

function Start()
{
	audio.clip=chirpingSound;
	audio.loop=true;
	audio.Play();
}

function OnTriggerEnter(other : Collider)
{
	if(other.CompareTag("FPS Controller"))
	{
		audio.loop=false;
		audio.Stop();
	}
}
function OnTriggerExit(other : Collider)
{
	if(other.CompareTag("FPS Controller"))
	{
		audio.loop=true;
		audio.Play();
	}
}


function Update()
{

		if(Input.GetKeyUp(KeyCode.Alpha1))
		{
			inputString+="1";
		}
		else if(Input.GetKeyUp(KeyCode.Alpha2))
		{
			inputString+="2";
		}
		else if(Input.GetKeyUp(KeyCode.Alpha3))
		{
			inputString+="3";
		}
		else if(Input.GetKeyUp(KeyCode.Alpha4))
		{
			inputString+="4";
		}		
		else if(Input.GetKeyUp(KeyCode.Alpha5))
		{
			inputString+="5";
		}		
		else if(Input.GetKeyUp(KeyCode.Alpha6))
		{
			inputString+="6";
		}		
		else if(Input.GetKeyUp(KeyCode.Alpha7))
		{
			inputString+="7";
		}
		else if(Input.GetKeyUp(KeyCode.Alpha8))
		{
			inputString+="8";
		}
}

function runActivity()
{
	audio.mute=false;
	audio.Play();
	playerStatus.readyToPlay=true;
	KeyInstruments.userCanPlay=true;
	
	inputString="";
	while(inputString.Length<3)
	{
		yield;
	}		
	playerStatus.readyToPlay=false;
	KeyInstruments.userCanPlay=false;
		
		


	
	if(inputString=="454")
	{
		activitySuccess();
	}
	else
	{
		activityFailure();
	}
	
}

function flyBird()
{
	focusCamera.switchCameras(birdView);
	yield WaitForSeconds(1);
	birdObject.animation.Play("Flight", PlayMode.StopAll);
	
	
	var t = 0.0f;
	while (t < 4.0f)
	{
	t += Time.deltaTime * 0.006;
	 
	birdObject.transform.position = Vector3.Lerp(birdObject.transform.position, birdDestination.position, t);

	yield 0;
	}
}

function activitySuccess()
{
	var audioObject = AudioController.Play("On_Success_Sound");
	flyBird();
	yield WaitForSeconds(5);
	gameObject.GetComponent(activityController).success=true;
	gameObject.GetComponent(activityController).continueActivity=true;
	playerStatus.readyToTalk = false;
	Destroy(gameObject);	
}
		
function activityFailure()
{
	gameObject.GetComponent(activityController).success=false;
	gameObject.GetComponent(activityController).continueActivity=true;		
}
