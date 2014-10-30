#pragma strict


function runActivity()
{

Debug.Log("Pinned ya ageen");
gameObject.GetComponent(activityController).continueActivity=true;
gameObject.GetComponent(activityController).success=true;

}