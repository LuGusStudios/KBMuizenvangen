using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceInput : LugusSingletonRuntime<CatchingMiceInput> 
{
    public List<Waypoint> pathToWalk = new List<Waypoint>();
    protected ICatchingMiceCharacter _character = null;
    protected CatchingMiceTile _previousTile = null;
    protected CatchingMiceTile _lastAddedWaypoint = null;
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

            _character = hit.GetComponent<CatchingMiceCharacterPlayer>();
            if(_character != null)
            {
                _character.handle.StopRoutine();
                
                Debug.Log("character added " + _character.currentTile.waypoint);
                pathToWalk.Add(_character.currentTile.waypoint);
                _previousTile = _character.currentTile;
                _lastAddedWaypoint = _character.currentTile;
            }
            
        }
	    if(LugusInput.use.dragging && _character != null)
        {
            Vector3 dragPoint = LugusInput.use.ScreenTo3DPoint(_character.transform);
            //Debug.Log(dragPoint);
            CatchingMiceTile currentTile = CatchingMiceLevelManager.use.GetTileByLocation(dragPoint.x, dragPoint.y);
            if (currentTile != null)
            {
                if (currentTile == _previousTile)
                    return;
                Debug.Log("Current Tile : " + currentTile);
                //Debug.Log("Dragging"); 
                _previousTile = currentTile;
                //when the waypoints is already in the list, ignore it
                if (pathToWalk.Contains(currentTile.waypoint))
                {
                    if (pathToWalk[pathToWalk.Count - 2 ] == currentTile.waypoint)
                    {
                        pathToWalk.Remove(pathToWalk[pathToWalk.Count - 1]);
                        _lastAddedWaypoint = currentTile;
                    }
                    return;
                }
                    

                //when you are on a new tile, and the new tile has not been found in the list,
                //check in the 4 directions to see if it can add it
                if (currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().xAdd(+1) ||
                   currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().xAdd(-1) ||
                   currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().yAdd(+1) ||
                   currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().yAdd(-1))
                {

                    pathToWalk.Add(currentTile.waypoint);
                    _lastAddedWaypoint = currentTile;
                }
                else 
                {
                    //tile is not connected to the last tile
                }
                
            }
        }
        
        if (LugusInput.use.up && pathToWalk.Count > 0 && _character != null)
        {
            Debug.Log("UP");
            pathToWalk.Reverse();
            List<Waypoint> path = new List<Waypoint>( pathToWalk);
            _character.handle.StartRoutine(_character.MoveToDestination(path));
            _character = null;
            pathToWalk.Clear();
        }
	}
    void OnDrawGizmos()
    {
        if(pathToWalk.Count>0)
        {
            foreach (Waypoint path in pathToWalk)
	        {  
                Gizmos.DrawCube(path.transform.position, new Vector3(0.4f, 0.4f, 0.4f));
	        }
        }
    }
}

