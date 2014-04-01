﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public abstract class ICatchingMiceCharacter : MonoBehaviour
{
    protected List<Waypoint> navigationGraph = null;
    public float timeToReachTile = 0.5f;

    public CatchingMiceTile currentTile = null;
    public Waypoint targetWaypoint = null;
    public Waypoint.WaypointType walkable = Waypoint.WaypointType.None;

    public virtual void GetTarget()
    {
        //go to target
        List<Waypoint> graph = navigationGraph;

        Waypoint currentWaypoint = currentTile.waypoint;

        bool fullPath = false;

        if(targetWaypoint != null)
        {
            List<Waypoint> path = AStarCalculate(graph, currentWaypoint, targetWaypoint, out fullPath, walkable);

            LugusCoroutines.use.StartRoutine(MoveToDestination(path)); 
        }
    }
    public virtual void DoCurrentTileBehaviour()
    {

    }
    protected void Awake()
    {
        SetupLocal();
    }
    protected void Start()
    {
        SetupGlobal();
    }
    public virtual void SetupLocal()
    {
        //navigationGraph = new List<Waypoint>((Waypoint[])GameObject.FindObjectsOfType(typeof(Waypoint)));
        navigationGraph = new List<Waypoint>(CatchingMiceLevelManager.use.WaypointList);
        if (navigationGraph.Count == 0)
            Debug.LogError(transform.Path() + " : no navigationGraph found for this level!!");
    }
    public virtual void SetupGlobal()
    {
        
    }
    // TODO: move this to Util?
    protected List<Waypoint> AStarCalculate(List<Waypoint> waypoints, Waypoint start, Waypoint stop, out bool wasFullPath, Waypoint.WaypointType waypointType)
    {
        // https://code.google.com/p/csharpgameprogramming/source/browse/trunk/Examples/AdventureGames/PathFinding/AStar.cs

        List<Waypoint> path = new List<Waypoint>();

        // 1. Setup : clear cost and parents of waypoints
        foreach (Waypoint waypoint in waypoints)
        {
            waypoint.AStarCost = 0.0f;
            waypoint.AStarParent = null;
        }

        List<Waypoint> openList = new List<Waypoint>();
        List<Waypoint> closedList = new List<Waypoint>();

        openList.Add(start);
        wasFullPath = true;


        // 2. 
        bool running = true;
        int count = 0;
        Waypoint current = null;
        while (running)
        {

            if (openList.Count == 0)
            {
                //Debug.LogWarning("OpenList is empty. Current is " + current.name );

                // TODO: ga closedList af, neem degene met laagste Cost als current : TEST of dat juiste resultaten geeft

                current = closedList[0];
                foreach (Waypoint candidate in closedList)
                {
                    if (candidate.AStarCost < current.AStarCost)
                        current = candidate;
                }

                // we have reached our final destination
                // backtrace to build the path
                Waypoint pointer = current;
                while (pointer != null)
                {
                    //Debug.LogWarning("ADDING PATH item " + pointer.name + " from " + pointer.AStarParent);
                    path.Add(pointer);
                    pointer = pointer.AStarParent;
                }

                wasFullPath = false;
                running = false;

                continue; // skip the rest of the calculations
            }


            current = openList[0]; // initialize so it's not null

            // current should be the Best waypoint in the Open list (the one with the lowest cost at this time)
            foreach (Waypoint waypoint in openList)
            {
                if (waypoint.AStarCost < current.AStarCost)
                {
                    current = waypoint;
                }
            }

            //Debug.Log ("ASTar iteration " + count + " : new current == " + current.name);


            if (current == stop)
            {
                // we have reached our final destination
                // backtrace to build the path
                Waypoint pointer = stop;
                while (pointer != null)
                {
                    path.Add(pointer);
                    pointer = pointer.AStarParent;
                }

                wasFullPath = true;
                running = false;

                continue; // skip the rest of the calculations
            }

            // not sure if the current will still be a good option after these calculations
            // so remove it from the openList for now and only add if needed later
            openList.Remove(current);
            closedList.Add(current);

            //shifts the waypoint gridoffset back because the shift is only for the animationpath
            float gridOffsetCurrent = 0.0f;
            //worldobjects has gridoffsets, so only apply when there is an object
            if (current.parentTile.worldObject != null)
            {
                gridOffsetCurrent = current.parentTile.worldObject.gridOffset;
            }

            foreach (Waypoint neighbour in current.neighbours)
            {
                // http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html

                // NOTE: This is not actually the best implementation of AStar heuristics, as that uses the distance to the goal as well
                // however, I find this one gives a bit more variation and more interesting paths in the current setup, so keep it for now

                //shifts the waypoint gridoffset back because the shift is only for the animationpath
                float gridOffset = 0.0f;
                if (neighbour.parentTile.worldObject != null)
                {
                    gridOffset = neighbour.parentTile.worldObject.gridOffset;
                    //Debug.Log(neighbour.transform.name + " " + neighbour.transform.position.yAdd(-gridOffset).v2());
                }
                // use the distance to the neighbour as a heuristic here 
                float cost = current.AStarCost + Vector3.Distance(neighbour.transform.position.yAdd(-gridOffset).v2(), current.transform.position.yAdd(-gridOffsetCurrent).v2());//Vector3.Distance( neighbour.transform.position, stop.transform.position ); 

                // if the neighbour's cost is already higher than the cost for this node
                // the neighbour is never going to be the best path, so delete it from our calculations 
                if (openList.Contains(neighbour) && cost < neighbour.AStarCost)
                {
                    openList.Remove(neighbour);
                }

                if (closedList.Contains(neighbour) && cost < neighbour.AStarCost)
                {
                    closedList.Remove(neighbour);
                }

                // if neighbour has not yet been examined : put it up for examination
                // Waypoint type bitwise : groundType 01
                //                         bothType   11
                //                      &  -------------
                //                         groundType 01 == neighbour.Waypointype 
                if (!openList.Contains(neighbour) && !closedList.Contains(neighbour) && (neighbour.waypointType & waypointType) == neighbour.waypointType)
                {
                    //Debug.Log ("ASTAR : adding " + neighbour.name + " to open with cost " + cost);

                    neighbour.AStarCost = cost;
                    neighbour.AStarParent = current;
                    openList.Add(neighbour);
                }
            }

            // if we haven't found the shortest path after 50 iterations in this game, we probably won't find it ever
            if (count == 500)
            {
                Debug.LogError("TownNavigationDefault:AStarCalculate : reached iteration limit of 50. Returning path of only the Stop waypoint");
                path.Add(stop);
                wasFullPath = true;
                return path;
            }

            count++;
        }

        return path;
    }
    public virtual IEnumerator MoveToDestination(List<Waypoint> path)
    {
        float depth = transform.position.z;
        int pathIndex = path.Count - 1;
        while (pathIndex > -1)
        {
            gameObject.StopTweens();

            Vector3 movePosition = path[pathIndex].transform.position;

            //check which zdepth the object must be
            if (transform.position.z < path[pathIndex].transform.position.z)
            {
                movePosition.z = transform.position.z;
            }

            gameObject.MoveTo(movePosition).Time(timeToReachTile).Execute();

            //movementDirection = Vector3.Normalize(path[pathIndex].transform.position.z(transform.position.z) - transform.position);

            //float maxDistance = 0.4f; // units (in this setup = pixels)
            bool reachedTarget = false;
            while (!reachedTarget)
            {
                yield return null;

                reachedTarget = (Vector2.Distance(transform.position.v2(), path[pathIndex].transform.position.v2()) <= 0);// maxDistance);
            }

            //Mover.renderer.sortingOrder = path[pathIndex].layerOrder;

            //z needs to be the next tile because else the object will be behind the next tile while on its way to the next tile
            if (pathIndex > 0)
            {
                if (path[pathIndex - 1].transform.position.z <= path[pathIndex].transform.position.z)
                {
                    transform.position = transform.position.z(path[pathIndex - 1].transform.position.z);
                }
                else
                    transform.position = transform.position.z(path[pathIndex].transform.position.z);
            }
            else
                transform.position = transform.position.z(path[pathIndex].transform.position.z);

            currentTile = path[pathIndex].parentTile;
            DoCurrentTileBehaviour();

            pathIndex--;
        }

        // we have reached the final target now (or should have...)
        gameObject.StopTweens();

        //moving = false;
    }
}