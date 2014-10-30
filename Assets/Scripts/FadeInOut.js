
public var fadeOutTexture : Texture2D;
public var fadeSpeed = 0.3;
 
var drawDepth = -1000;
 
private var alpha = 1.0; 
 
private var fadeDir = -1;
function Start(){
	fadeIn();
}

function OnGUI(){
 
	alpha += fadeDir * fadeSpeed * Time.deltaTime;	
	alpha = Mathf.Clamp01(alpha);	
 
	GUI.color.a = alpha;
 
	GUI.depth = drawDepth;
 
	GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
}
 

 
function fadeIn(){
	alpha=1;
	fadeDir = -1;	
}
 
 
function fadeOut(){
	alpha=0;
	fadeDir = 1;	
}
 
