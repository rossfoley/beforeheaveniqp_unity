#pragma strict
public var targetScript : GameObject;
var lookAtMe : boolean = false;
var startingPosition : Quaternion;

function Start () {
startingPosition = transform.rotation;
targetScript = GameObject.FindGameObjectWithTag("FPS Controller");
}

function Update()
{
	if(lookAtMe)
	{
	var rotation = Quaternion.LookRotation(Vector3(targetScript.transform.position.x, transform.position.y, targetScript.transform.position.z) - transform.position);
	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 6.0);
	}
	else
	{
	transform.rotation = Quaternion.Lerp(transform.rotation, startingPosition, Time.deltaTime * 4.0);
	}
}

function OnTriggerEnter(other : Collider)
{
	if(other.CompareTag("FPS Controller"))
	{
	lookAtMe=true;
	}
}
function OnTriggerExit(other : Collider)
{
	if(other.CompareTag("FPS Controller"))
	{
	lookAtMe=false;
	}
}