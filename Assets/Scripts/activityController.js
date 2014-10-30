//controls movement to next segment of activity
internal var continueActivity : boolean = false;
//controls movement to next segment of activity
internal var breakActivity : boolean = false;
//did the main activity succeed?
internal var success : boolean = false;
//dialog object of activity
var startDialogue : DialogController[];
//dialog object of activity if succeeded
var successDialogue : DialogController[];
//dialog object of activity if failed
var failDialogue : DialogController[];
//coins rewarded after successful activity
var goldReward : int[];
//rotating model after successful activity
var rewardModel : GameObject[];
//number of times activity completed
internal var completedCount : int = 0;
//points to current main activity script
var currentActivity : List.<String> = new List.<String>();
//moves camera to focus on character
var focusPoint : Transform;
//are activities random or linear?
var randomActivity : boolean = false;

function Start()
{
	if(randomActivity)
	{
		completedCount = Random.Range(0, currentActivity.Count);
	}
}

function Update ()
{
	if(completedCount<rewardModel.Length && rewardModel[completedCount] && rewardModel[completedCount].activeSelf && Input.GetButtonUp("actionButton1"))
	{
		EndActivity();
		rewardModel[completedCount].SetActive(false);
		if(randomActivity)
		{
			completedCount = Random.Range(0, currentActivity.Count);
		}
		else if(completedCount<rewardModel.Length-1)
		{
			completedCount++;
		}
	}
}

function muteAllSounds(toggler : boolean)
{
	var allAudioSources : AudioSource[] = FindObjectsOfType(AudioSource) as AudioSource[];
	for(var individualAudio : AudioSource in allAudioSources)
	{
		individualAudio.mute=toggler;
	}
}

function StartActivity()
{
	muteAllSounds(true);
	breakActivity=false;
	continueActivity=false;
	if(startDialogue[completedCount])
	{
		startDialogue[completedCount].Play();
		while(!continueActivity)
		{
			yield;
		}
		
		if(breakActivity)
		{
			audioObject = AudioController.Play("Close_Sound");			
			EndActivity();
			return;
		}
	continueActivity=false;
	}
	
	if(currentActivity[completedCount])
	{
		gameObject.GetComponent(currentActivity[completedCount]).runActivity();
		Debug.Log("Continued activity");
		while(!continueActivity)
		{
			yield;
		}
		
		if(success)
		{
			if(successDialogue[completedCount])
			{	
				audioObject = AudioController.Play("On_Success_Sound");
				successDialogue[completedCount].Play();
				continueActivity=false;
				while(!continueActivity)
				{
					yield;
				}	
			}
		
			GiveAward();	
		}
		else
		{
			audioObject = AudioController.Play("On_Fail_Sound");
			if(failDialogue[completedCount])
			{	
				failDialogue[completedCount].Play();
				continueActivity=false;
				while(!continueActivity)
				{
				yield;
				}	
			}
			EndActivity();	
		}
	}

}

function Continue()
{
	continueActivity=true;
}
function KillActivity()
{
	breakActivity=true;
	Continue();
}

function EndActivity()
{
	muteAllSounds(false);
	GameObject.FindGameObjectWithTag("FPS Controller").GetComponent(InteractivityController).EnableMovement();
}
function GiveAward()
{	
	if(goldReward[completedCount]>0)
	{
		audioObject = AudioController.Play("Coin_Sound");
		Debug.Log("Awarded " + goldReward[completedCount] + " coins!");
	}
	
	if(rewardModel[completedCount])
	{
		audioObject = AudioController.Play("Awarded_Sound");
		rewardModel[completedCount].SetActive(true);
	}
	else
	{
		EndActivity();
		if(randomActivity)
		{
			completedCount = Random.Range(0, currentActivity.Count);
		}
		else if(completedCount<startDialogue.Length-1)
		{
			completedCount++;
		}
	}	
}