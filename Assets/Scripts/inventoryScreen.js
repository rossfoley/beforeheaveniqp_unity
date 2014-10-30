#pragma strict
import System.Collections.Generic;

var instrumentList : List.<String> = new List.<String>();
var clothingList : List.<String> = new List.<String>();
var itemList : List.<String> = new List.<String>();
var currentMenu : int = 0;
var toolTipLabelHorizontal : int = 0;
var toolTipLabelVertical : int = 0;
var BHSkin : GUISkin;

var iconHeight : int = 70;
var iconWidth : int = 70;
var iconHeightSpacing : int = 15;
var iconWidthSpacing : int = 15;
var iconRowCount : int = 5;
var iconColumnCount : int = 7;
var panelMarginHorizontal : int = -250;
var panelMarginVertical : int = 200;

var iconHeightInstrument : int = 70;
var iconWidthInstrument : int = 70;
var iconHeightSpacingInstrument : int = 15;
var iconWidthSpacingInstrument : int = 15;
var iconRowCountInstrument : int = 1;
var iconColumnCountInstrument : int = 6;
var panelMarginHorizontalInstrument : int = -250;
var panelMarginVerticalInstrument : int = 200;

var instrumentIcons : List.<Texture2D> = new List.<Texture2D>();

var iconHeightClothing : int = 70;
var iconWidthClothing : int = 70;
var iconHeightSpacingClothing : int = 15;
var iconWidthSpacingClothing : int = 15;
var iconRowCountClothing : int = 2;
var iconColumnCountClothing : int = 4;
var panelMarginHorizontalClothing : int = -250;
var panelMarginVerticalClothing : int = 200;

var clothingIcons : List.<Texture2D> = new List.<Texture2D>();

var iconHeightItem : int = 70;
var iconWidthItem : int = 70;
var iconHeightSpacingItem : int = 15;
var iconWidthSpacingItem : int = 15;
var iconRowCountItem : int = 4;
var iconColumnCountItem : int = 6;
var panelMarginHorizontalItem : int = -250;
var panelMarginVerticalItem : int = 200;

var itemIcons : List.<Texture2D> = new List.<Texture2D>();


function Start ()
{
instrumentList.Add("Harp");
instrumentList.Add("Flute");
instrumentList.Add("Piano");
instrumentList.Add("Drums");
instrumentList.Add("Synth");
instrumentList.Add("Flute & Harpsichord");
clothingList.Add("Hover Boots");
clothingList.Add("Iron Boots");
clothingList.Add("Goron Tunic");
clothingList.Add("Golden Gauntlets");
clothingList.Add("Zora Mask");
clothingList.Add("Deku Shield");
clothingList.Add("Goron Bracelet");
clothingList.Add("Goron Mask");
itemList.Add("Deku Nut");
itemList.Add("Stick");
itemList.Add("Slingshot");
itemList.Add("Ocarina");
itemList.Add("Bow");
itemList.Add("Din's Fire");
itemList.Add("Nayru's Love");
itemList.Add("Farore's Wind");
itemList.Add("Empty Bottle");
itemList.Add("Lens of Truth");
itemList.Add("Magic Beans");
itemList.Add("Triforce");
itemList.Add("Master Sword");
itemList.Add("Biggoron Sword");
itemList.Add("Mirror Shield");
itemList.Add("Fire Arrow");
itemList.Add("Ice Arrow");
itemList.Add("Light Arrow");
itemList.Add("Ice Medallion");
itemList.Add("Fire Medallion");
itemList.Add("Forest Medallion");
itemList.Add("Spirit Medallion");
itemList.Add("Water Medallion");
itemList.Add("Shadow Medallion");
itemList.Add("Shadow Medallion");
itemList.Add("Shadow Medallion");
itemList.Add("Shadow Medallion");
changeMenu(0);
}

var selectedInstrument : int = 0;
var selectedClothing : int = 0;
var selectedItem : int = 0;

 

 

// Function to scroll through possible menu items array, looping back to start/end depending on direction of movement.

function menuSelection (menuList : List.<String>, selectedItem : int, direction)
{
	

    if (direction == "up")
    {
        if (selectedItem == 0) 
        {
            selectedItem = menuList.Count - 1;
        }
        else 
        {
            selectedItem -= 1;
        }
    }

    if (direction == "down")
    {
        if (selectedItem == menuList.Count - 1)
        {
            selectedItem = 0;
        }
        else
        {
            selectedItem += 1;
        }
    }

    return selectedItem;
}
function changeMenu(newMenu : int)
{
	currentMenu=newMenu;
 	if(newMenu==1)
 	{
 	iconHeight = iconHeightClothing;
    iconWidth = iconHeightClothing;
	iconWidthSpacing = iconHeightSpacingClothing;
	iconRowCount = iconRowCountClothing;
	iconColumnCount = iconColumnCountClothing;
	panelMarginHorizontal = panelMarginHorizontalClothing;
	panelMarginVertical = panelMarginVerticalClothing;
 	}
 	else if(newMenu==2)
 	{
 	iconHeight = iconHeightItem;
    iconWidth = iconHeightItem;
	iconWidthSpacing = iconHeightSpacingItem;
	iconRowCount = iconRowCountItem;
	iconColumnCount = iconColumnCountItem;
	panelMarginHorizontal = panelMarginHorizontalItem;
	panelMarginVertical = panelMarginVerticalItem;
 	}
 	else
 	{
 	iconHeight = iconHeightInstrument;
    iconWidth = iconHeightInstrument;
	iconWidthSpacing = iconHeightSpacingInstrument;
	iconRowCount = iconRowCountInstrument;
	iconColumnCount = iconColumnCountInstrument;
	panelMarginHorizontal = panelMarginHorizontalInstrument;
	panelMarginVertical = panelMarginVerticalInstrument;
 	}
}
function Update ()
{

    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
    	if(currentMenu==0)
        selectedInstrument = menuSelection(instrumentList, selectedInstrument, "down");
		else if(currentMenu==1)
        selectedClothing = menuSelection(clothingList, selectedClothing, "down");
		else if(currentMenu==2)
        selectedItem = menuSelection(itemList, selectedItem, "down");
    }

    if (Input.GetKeyUp(KeyCode.UpArrow))
    {
    	if(currentMenu==0)
        selectedInstrument = menuSelection(instrumentList, selectedInstrument, "up");
		else if(currentMenu==1)
        selectedClothing = menuSelection(clothingList, selectedClothing, "up");
		else if(currentMenu==2)
        selectedItem = menuSelection(itemList, selectedItem, "up");
    }

}
function changeInstrument(instrumentNumber : int)
{
	if(instrumentNumber==0)
	{
			globals.INSTRUMENT = "nylon"; 
			nylon.standard_C(); 	
	}
	else if(instrumentNumber==1)
	{
			globals.INSTRUMENT = "flute"; 
			flute.standard_C(); 
	}
	else if(instrumentNumber==2)
	{
			globals.INSTRUMENT = "piano"; 
			piano.standard_C(); 
	}
	else if(instrumentNumber==3)
	{
			globals.INSTRUMENT = "drums_rock"; 
			drums_rock.pattern_C(); 
	}
	else if(instrumentNumber==4)
	{
			globals.INSTRUMENT = "synth_strings";
			synth_strings.standard_C();
	}
	else if(instrumentNumber==5)
	{
			globals.INSTRUMENT = "flute_harpsichord"; 
			flute_harpsichord.standard_C(); 
	}
}
function GUIKeyDown(key : KeyCode)
    {
    if (Event.current.type == EventType.KeyDown)
   		return (Event.current.keyCode == key);
    return false;
}
function OnGUI()
{
	GUI.depth=2;
	GUI.skin=BHSkin;
	if(currentMenu==0)
	GUI.tooltip=instrumentList[selectedInstrument];
	else if(currentMenu==1)
	GUI.tooltip=clothingList[selectedClothing];	
	else if(currentMenu==2)
	GUI.tooltip=itemList[selectedItem];
	for(var i=0; i<iconRowCount; i++)
	{
		for(var j=0; j<iconColumnCount; j++)
		{
			if(Rect((Screen.width/2+panelMarginHorizontal)+iconWidthSpacing+((iconWidth+iconWidthSpacing)*j), panelMarginVertical+((iconHeight+iconHeightSpacing)*i), iconWidth, iconHeight).Contains(Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y)))
			{
				if(Event.current.type == EventType.MouseDown)
				{
				
				}
				if(Event.current.type == EventType.MouseUp)
				{
					if(currentMenu==0)
					{
					changeInstrument(i*iconColumnCount+j);
					selectedInstrument=i*iconColumnCount+j;
					}
					else if(currentMenu==1)
					{
					Debug.Log("You equipped " + clothingList[i*iconColumnCount+j]);					
					selectedClothing=i*iconColumnCount+j;
					}
					else if(currentMenu==2)
					{
					Debug.Log("You used " + itemList[i*iconColumnCount+j]);
					selectedItem=i*iconColumnCount+j;
					}
				}
				if(GUIKeyDown(KeyCode.Return))
				{
					if(currentMenu==0)
					{
					changeInstrument(i*iconColumnCount+j);
					selectedInstrument=i*iconColumnCount+j;
					}
					else if(currentMenu==1)
					{
					Debug.Log("You equipped " + clothingList[i*iconColumnCount+j]);					
					selectedClothing=i*iconColumnCount+j;
					}
					else if(currentMenu==2)
					{
					Debug.Log("You used " + itemList[i*iconColumnCount+j]);
					selectedItem=i*iconColumnCount+j;
					}
				}								
			}
			
			if(currentMenu==0)
			{
			GUI.SetNextControlName (instrumentList[(i*iconColumnCount+j)]);		
			GUI.Button(Rect((Screen.width/2+panelMarginHorizontal)+iconWidthSpacing+((iconWidth+iconWidthSpacing)*j), panelMarginVertical+((iconHeight+iconHeightSpacing)*i), iconWidth, iconHeight), GUIContent (instrumentIcons[i*iconColumnCount+j], instrumentList[(i*iconColumnCount+j)]));
			}
			else if(currentMenu==1)
			{
			GUI.SetNextControlName (clothingList[(i*iconColumnCount+j)]);		
			GUI.Button(Rect((Screen.width/2+panelMarginHorizontal)+iconWidthSpacing+((iconWidth+iconWidthSpacing)*j), panelMarginVertical+((iconHeight+iconHeightSpacing)*i), iconWidth, iconHeight), GUIContent ("C #" + "[" + i + "] [" + j + "]", clothingList[(i*iconColumnCount+j)]));
			}
			else if(currentMenu==2)
			{
			GUI.SetNextControlName (itemList[(i*iconColumnCount+j)]);		
			GUI.Button(Rect((Screen.width/2+panelMarginHorizontal)+iconWidthSpacing+((iconWidth+iconWidthSpacing)*j), panelMarginVertical+((iconHeight+iconHeightSpacing)*i), iconWidth, iconHeight), GUIContent ("I #" + "[" + i + "] [" + j + "]", itemList[(i*iconColumnCount+j)]));
			}
		}
	}
	
	
	if(GUI.Button(Rect(Screen.width/2-180, 150, 150, 25), "Instruments"))
	{
		changeMenu(0);
	}
	if(GUI.Button(Rect(Screen.width/2, 150, 150, 25), "Clothing"))
	{
		changeMenu(1);
	}
	if(GUI.Button(Rect(Screen.width/2+180, 150, 150, 25), "Items"))
	{
		changeMenu(2);
	}
	
		// Display the tooltip from the element that has mouseover or keyboard focus
	GUI.Label (Rect (Screen.width/2+toolTipLabelHorizontal,toolTipLabelVertical,250,40), GUI.tooltip);
	if(currentMenu==0)
	GUI.FocusControl (instrumentList[selectedInstrument]);
	else if(currentMenu==1)
	GUI.FocusControl (clothingList[selectedClothing]);	
	else if(currentMenu==2)
	GUI.FocusControl (itemList[selectedItem]);

}