#pragma strict

internal var inputString : String = "";


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
	flute.scale_C();
	playerStatus.readyToPlay=true;
	KeyInstruments.userCanPlay=true;
	
	inputString="";
	while(inputString.Length<3)
	{
		yield;
	}		
	playerStatus.readyToPlay=false;
	KeyInstruments.userCanPlay=false;
		
		


	
	if(inputString=="135")
	{
		activitySuccess();
	}
	else
	{
		activityFailure();
	}
	
}

function activitySuccess()
{
	gameObject.GetComponent(activityController).success=true;
	gameObject.GetComponent(activityController).continueActivity=true;		
}		
function activityFailure()
{
	gameObject.GetComponent(activityController).success=false;
	gameObject.GetComponent(activityController).continueActivity=true;		
}	