#pragma strict
static var mainCharacter : GameObject;
static var mainCamera : GameObject;
function Start () {
mainCharacter = GameObject.FindGameObjectWithTag("FPS Controller");
mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
}

function Update () {

}

public static function switchCameras(cameraTarget : Transform)
{
	var t = 0.0f;
	var startingPos = mainCamera.transform.position;
	var startingRot = mainCamera.transform.rotation;
	while (t < 1.0f)
	{
	t += Time.deltaTime * (Time.timeScale/1.0f);
	 
	mainCamera.transform.position = Vector3.Lerp(startingPos, cameraTarget.position, t);
	mainCamera.transform.rotation = Quaternion.Lerp(startingRot, Quaternion.Euler(new Vector3(cameraTarget.transform.rotation.eulerAngles.x, cameraTarget.transform.rotation.eulerAngles.y, cameraTarget.transform.rotation.eulerAngles.z)), t);

	yield 0;
	}
}
public static function toggleMovement()
{
	mainCharacter.GetComponent(ThirdPersonController).enabled = !mainCharacter.GetComponent(ThirdPersonController).enabled;
	mainCharacter.GetComponent(ThirdPersonCamera).enabled= !mainCharacter.GetComponent(ThirdPersonCamera).enabled;
	Input.ResetInputAxes();
}