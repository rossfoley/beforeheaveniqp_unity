var timeOut = 3.0;
var detachChildren = false;

function Awake ()
{
	Invoke ("DestroyNow", 6);
}

function DestroyNow ()
{
	if (detachChildren) {
		transform.DetachChildren ();
	}
	DestroyObject (gameObject);
}