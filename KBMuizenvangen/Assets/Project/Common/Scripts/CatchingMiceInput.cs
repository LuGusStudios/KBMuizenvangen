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
        CatMovementInput();

        VisualizePath();

        DoTrapBehaviourInput();
	}
    //protected void CheckDraggingPoints()
    //{
    //    Vector3 dragPoint = LugusInput.use.ScreenTo3DPoint(_character.transform);
    //    //Debug.Log(dragPoint);
    //    CatchingMiceTile currentTile = CatchingMiceLevelManager.use.GetTileByLocation(dragPoint.x, dragPoint.y);
    //    if (currentTile == null)
    //        return;

    //    if (currentTile == _previousTile || !_character.IsWalkable(currentTile))
    //        return;

    //    //Debug.Log("Current Tile : " + currentTile);
    //    _previousTile = currentTile;

    //    //when the waypoints is already in the list, ignore it
    //    if (pathToWalk.Contains(currentTile.waypoint))
    //    {
    //        if (pathToWalk[pathToWalk.Count - 2] == currentTile.waypoint)
    //        {
    //            pathToWalk.Remove(pathToWalk[pathToWalk.Count - 1]);
    //            _lastAddedWaypoint = currentTile;
    //            _lineRenderer.SetVertexCount(pathToWalk.Count);

    //        }
    //        return;
    //    }


    //    //when you are on a new tile, and the new tile has not been found in the list,
    //    //check in the 4 directions to see if it can add it
    //    if (currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().xAdd(+1) ||
    //        currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().xAdd(-1) ||
    //        currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().yAdd(+1) ||
    //        currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().yAdd(-1))
    //    {

    //        pathToWalk.Add(currentTile.waypoint);
    //        _lastAddedWaypoint = currentTile;
    //    }
    //}
   
    public void CatMovementInput()
    {
        //First downpress
        if (LugusInput.use.down)
        {
            Transform hit = LugusInput.use.RayCastFromMouse();

            if (hit == null)
                return;

            //pathToWalk.Clear();
            //Debug.Log(hit.name);

            _character = hit.parent.GetComponent<CatchingMiceCharacterPlayer>();
            if (_character != null)
            {
                //when it is still jumping, let the jump finish before stopping
                if (_character.jumping)
                {
                    _character.interrupt = true;
                    return;
                }

                _character.StopCurrentBehaviour();

                CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(_character.transform.position.x, _character.transform.position.y);
                pathToWalk.Add(_character.currentTile.waypoint);

                //CatchingMiceTile tile =  CatchingMiceLevelManager.use.GetTileByLocation(hit.position.x, hit.position.y);
                //pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(tile.gridIndices));
                _lastAddedWaypoint = tile;
            }

        }

        //When dragging, try get the right swipe path
        else if (LugusInput.use.dragging && _character != null && pathToWalk.Count > 0)
        {
            //CheckDraggingPoints();   
            CheckDraggingPointsOffGrid();
        }

        //when not dragging anymore get the path converted from the waypoints
        else if (LugusInput.use.up && pathToWalk.Count > 1 && _character != null)
        {

            //Debug.Log("UP");
            //pathToWalk.Reverse();
            List<Waypoint> path = new List<Waypoint>(pathToWalk);
            _character.MoveWithPath(path);
            _character = null;
            pathToWalk.Clear();
        }
    }

    protected void CheckDraggingPointsOffGrid()
    {
        Transform hit = LugusInput.use.RayCastFromMouse();
        float yOffset = 0.0f;
        //on need to check this for the first count only
        //because the player is taller than 1 tile, when you click on the tile that is above your tile, you don't want that added
        //if you're still in the cat collider with your mouse, don't add
        if (hit != null)
        {
            if (pathToWalk.Count == 1)
            {
                CatchingMiceCharacterPlayer character = null;
                character = hit.parent.GetComponent<CatchingMiceCharacterPlayer>();
                if (character != null && character == _character)
                    return;
            }

            //check furniture tiles for shifts
            CatchingMiceWorldObject FurnitureObject = null;
            FurnitureObject = hit.parent.GetComponent<CatchingMiceWorldObject>();
            if (FurnitureObject != null)
            {
                if (FurnitureObject.parentTile != null)
                {
                    //make sure we have the furniture and not a trap
                    //if it is null than you hit a ground trap, so no furniture object
                    if (FurnitureObject.parentTile.worldObject != null)
                        yOffset = FurnitureObject.parentTile.worldObject.gridOffset * CatchingMiceLevelManager.use.scale;
                }

            }

        }

        Vector3 dragPoint = LugusInput.use.ScreenTo3DPoint(_character.transform);
        CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(dragPoint.x, dragPoint.y - yOffset);
        //only add a tile when its new tile is more than half a grid away
        if (Vector2.Distance(tile.gridIndices, _lastAddedWaypoint.gridIndices) < CatchingMiceLevelManager.use.scale / 2)
            return;

        float distance = Vector2.Distance(tile.gridIndices, _lastAddedWaypoint.gridIndices);
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

    public void DoTrapBehaviourInput()
    {
         //First downpress
        if (!LugusInput.use.down)
            return;

        Transform hit = LugusInput.use.RayCastFromMouse();

        if (hit == null)
            return;

        //pathToWalk.Clear();
        //Debug.Log(hit.name);

        ICatchingMiceWorldObjectTrap trap = hit.parent.GetComponent(typeof(ICatchingMiceWorldObjectTrap)) as ICatchingMiceWorldObjectTrap;
        
        if (trap == null)
            return;

        //go over the 2 characters and check if the trap is in range
        List<ICatchingMiceCharacter> characters = new List<ICatchingMiceCharacter>(CatchingMiceLevelManager.use.playerList);
        foreach (ICatchingMiceCharacter character in characters)
        {
            CatchingMiceTile[] tilesAround = CatchingMiceLevelManager.use.GetTileAround(character.currentTile);
            foreach (CatchingMiceTile tile in tilesAround)
            {
                if (tile == null)
                    continue;
                if (tile.trapObject == null)
                    continue;
                if(tile.trapObject == trap)
                {
                    trap.DoBehaviour();
                    return;
                }
            }
        }
        
        
    }

    public void VisualizePath()
    {
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

