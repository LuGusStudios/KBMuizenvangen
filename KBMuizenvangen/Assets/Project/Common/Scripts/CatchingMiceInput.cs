using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceInput : LugusSingletonRuntime<CatchingMiceInput> 
{
    public List<Waypoint> pathToWalk = new List<Waypoint>();
    protected CatchingMiceCharacterPlayer _character = null;
    protected CatchingMiceTile _previousTile = null;
    protected CatchingMiceTile _lastAddedWaypoint = null;

    protected LineRenderer _lineRenderer = null;

    protected float _timer = 0.0f;
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
                _character.StopCurrentBehaviour();
                
                Debug.Log("character added " + _character.currentTile.waypoint);
                //pathToWalk.Add(_character.currentTile.waypoint);
                //_previousTile = _character.currentTile;
                //_lastAddedWaypoint = _character.currentTile;
                CatchingMiceTile tile =  CatchingMiceLevelManager.use.GetTileByLocation(hit.position.x, hit.position.y);
                pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(tile.gridIndices));
                _lastAddedWaypoint = tile;
            }
            
        }

        //When dragging, try get the right swipe path
	    else if(LugusInput.use.dragging && _character != null)
        {
            //CheckDraggingPoints();   
            CheckDraggingPointsOffGrid();
        }
        
        //when not dragging anymore get the path converted from the waypoints
        else if (LugusInput.use.up && pathToWalk.Count > 0 && _character != null)
        {

            //Debug.Log("UP");
            //pathToWalk.Reverse();
            List<Waypoint> path = new List<Waypoint>(pathToWalk);
            _character.MoveWithPath(path);
            _character = null; 
            //pathToWalk.Clear();
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
    protected void CheckDraggingPoints()
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
            if (pathToWalk[pathToWalk.Count - 2] == currentTile.waypoint)
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
    protected void CheckDraggingPointsOffGrid()
    {
        Vector3 dragPoint = LugusInput.use.ScreenTo3DPoint(_character.transform);
        CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(dragPoint.x, dragPoint.y);
        //only add a tile when its new tile is more than half a grid away
        if (Vector2.Distance(tile.gridIndices, _lastAddedWaypoint.gridIndices) < CatchingMiceLevelManager.use.scale / 2)
            return; 

        float distance = Vector2.Distance(tile.gridIndices,_lastAddedWaypoint.gridIndices);
        float maxDistance = CatchingMiceLevelManager.use.scale * 2; 
        //if distance is more then x grids away interpolate
        if (distance > maxDistance) 
        {
            while (distance > maxDistance)
            {
                // interpolated vector: value * (endpoint - beginpoint) + beginpoint --> value between begin and end point
                Vector3 interpolated = (maxDistance) * Vector3.Normalize(tile.gridIndices.v3() - _lastAddedWaypoint.gridIndices.v3()) + _lastAddedWaypoint.gridIndices.v3();
                
                CatchingMiceTile interpolatedTile = CatchingMiceLevelManager.use.GetTile(interpolated.v2());

                pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(interpolatedTile.gridIndices));
                _lastAddedWaypoint = interpolatedTile;
                //Debug.Log("added interpolated tile " + interpolated);
                distance -= maxDistance;
            }
        }


        pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(tile.gridIndices));

        _lastAddedWaypoint = tile;
        
        
    }
    protected void OnDrawGizmos()
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

