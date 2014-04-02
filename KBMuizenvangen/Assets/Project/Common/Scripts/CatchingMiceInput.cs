using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceInput : LugusSingletonRuntime<CatchingMiceInput> 
{
    List<Waypoint> pathToWalk = new List<Waypoint>();
    ICatchingMiceCharacter character = null;
    CatchingMiceTile previousTile = null;
    CatchingMiceTile lastAddedWaypoint = null;
	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(LugusInput.use.down)
        {
            Debug.Log("Down");
            Transform hit = LugusInput.use.RayCastFromMouse();
            
            if (hit == null)
                return;
             
            pathToWalk.Clear();
            Debug.Log(hit.name);

            character = hit.GetComponent<ICatchingMiceCharacter>();
            if(character != null)
            {
                character.SetupLocal();
                Debug.Log("character added " + character.currentTile.waypoint);
                pathToWalk.Add(character.currentTile.waypoint);
                previousTile = character.currentTile;
            }
            
        }
	    if(LugusInput.use.dragging && character != null)
        {
            
            CatchingMiceTile currentTile = CatchingMiceLevelManager.use.GetTileByLocation(LugusInput.use.lastPoint.x,LugusInput.use.lastPoint.y);
            Debug.Log("drag " + currentTile + " :  " + LugusInput.use.lastPoint);
            if (currentTile != null)
            {
                if (currentTile == previousTile)
                    return;
                Debug.Log("Dragging"); 
                //previousTile = currentTile;

                if (pathToWalk.Contains(currentTile.waypoint))
                    return;

                //when you are on a new tile, and the new tile has not been found in the list,
                //check in the 4 directions to see if it can add it
                if (currentTile.gridIndices == lastAddedWaypoint.gridIndices.x(+1) ||
                   currentTile.gridIndices == lastAddedWaypoint.gridIndices.x(-1) ||
                   currentTile.gridIndices == lastAddedWaypoint.gridIndices.y(+1) ||
                   currentTile.gridIndices == lastAddedWaypoint.gridIndices.y(+1))
                {

                    pathToWalk.Add(currentTile.waypoint);
                    lastAddedWaypoint = currentTile;
                }
                else 
                {
                    //tile is not connected to the last tile
                }
                previousTile = currentTile;
            }
        }
        if(LugusInput.use.up && pathToWalk.Count > 0)
        {
            Debug.Log("UP");
            character.MoveToDestination(pathToWalk);
            
        }
	}
}

