using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceInput : LugusSingletonRuntime<CatchingMiceInput> 
{
    public List<Waypoint> pathToWalk = new List<Waypoint>();
    protected ICatchingMiceCharacter _character = null;
    protected CatchingMiceTile _previousTile = null;
    protected CatchingMiceTile _lastAddedWaypoint = null;

    protected LineRenderer _lineRenderer = null;
	// Use this for initialization
	void Start () 
    {
        Transform lineRenderer = GameObject.Find("LineRenderer").transform;
        if (lineRenderer != null)
        {
            _lineRenderer = lineRenderer.GetComponent<LineRenderer>();
            if(_lineRenderer == null)
            {
                Debug.LogError("Line Renderer not found");
            }
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        //First downpress
        if(LugusInput.use.down)
        {
            Debug.Log("Down");
            Transform hit = LugusInput.use.RayCastFromMouse();
            
            if (hit == null)
                return;
             
            pathToWalk.Clear();
            Debug.Log(hit.name);

            _character = hit.parent.GetComponent<CatchingMiceCharacterPlayer>();
            if(_character != null)
            {
                _character.handle.StopRoutine();
                
                Debug.Log("character added " + _character.currentTile.waypoint);
                pathToWalk.Add(_character.currentTile.waypoint);
                _previousTile = _character.currentTile;
                _lastAddedWaypoint = _character.currentTile;
            }
            
        }

        //When dragging, try get the right swipe path
	    if(LugusInput.use.dragging && _character != null)
        {
            Vector3 dragPoint = LugusInput.use.ScreenTo3DPoint(_character.transform);
            //Debug.Log(dragPoint);
            CatchingMiceTile currentTile = CatchingMiceLevelManager.use.GetTileByLocation(dragPoint.x, dragPoint.y);
            if (currentTile == null)
                return;
           
            if (currentTile == _previousTile || !_character.IsWalkable(currentTile))
                return;

            //Debug.Log("Current Tile : " + currentTile);
            _previousTile = currentTile;

            //when the waypoints is already in the list, ignore it
            if (pathToWalk.Contains(currentTile.waypoint))
            {
                if (pathToWalk[pathToWalk.Count - 2 ] == currentTile.waypoint)
                {
                    pathToWalk.Remove(pathToWalk[pathToWalk.Count - 1]);
                    _lastAddedWaypoint = currentTile;
                    _lineRenderer.SetVertexCount(pathToWalk.Count);
                        
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
                
            
        }
        
        //when not dragging anymore get the path converted from the waypoints
        if (LugusInput.use.up && pathToWalk.Count > 0 && _character != null)
        {
            Debug.Log("UP");
            pathToWalk.Reverse();
            List<Waypoint> path = new List<Waypoint>( pathToWalk);
            _character.handle.StartRoutine(_character.MoveToDestination(path));
            _character = null;
            pathToWalk.Clear();
        }

        //visualisation of the motion the player makes
        if (pathToWalk.Count > 0)
        {
            _lineRenderer.SetVertexCount(pathToWalk.Count);
            for (int i = 0; i < pathToWalk.Count; i++)
            {
                _lineRenderer.SetPosition(i, pathToWalk[i].transform.position.z(-1));
            }
        }
        else
            _lineRenderer.SetVertexCount(0);
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

