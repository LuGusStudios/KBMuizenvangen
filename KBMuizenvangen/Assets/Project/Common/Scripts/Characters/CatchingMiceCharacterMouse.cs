using UnityEngine;
using System.Collections;

public class CatchingMiceCharacterMouse : ICatchingMiceCharacter
{
    public float health = 100.0f;
    public CatchingMiceTile currentTile = null;
    public Waypoint targetWaypoint = null;
    public Waypoint.WaypointType walkable = Waypoint.WaypointType.None;
    public void SetupLocal()
    {
        currentTile = CatchingMiceLevelManager.use.GetTileByLocation(transform.position.x, transform.position.y);
        walkable = Waypoint.WaypointType.Ground;
    }
    protected void OnAwake()
    {
        SetupLocal();
    }
	// Use this for initialization
	protected void Start () 
    {
	
	}
	
	// Update is called once per frame
    protected void Update() 
    {
        CatchingMiceTile newTile = CatchingMiceLevelManager.use.GetTileByLocation(transform.position.x, transform.position.y);
	    //do behaviours when new tile is hit
        if(currentTile != newTile)
        {
            currentTile = newTile;
            DoCurrentTileBehaviour();
        }
	}

    public void MoveToDestination()
    {
        CatchingMiceTile targetTile = null;
        float smallestDistance = float.MaxValue;
        //Check which cheese tile is the closest
        foreach (CatchingMiceTile tile in CatchingMiceLevelManager.use.CheeseTiles)
        {
            float distance = Vector2.Distance(transform.position.v2(), tile.location.v2());
            if (distance < smallestDistance)
            {
                targetTile = tile;
                smallestDistance = distance;
            }
        }

        //After we found the closest tile, get the waypoint of the tile
        if (targetTile != null)
        {
            foreach (Waypoint wp in CatchingMiceLevelManager.use.WaypointList)
            {
                if (wp.transform.position == targetTile.location)
                {
                    targetWaypoint = wp;
                    break;
                }
            }
        }

        if (targetWaypoint != null)
        {
            //go to target
        }
        else
        {
            Debug.LogError("No target found");
        }
    }

    public void DoCurrentTileBehaviour()
    {
        if(currentTile.trapObject != null)
        {
            //get hit by trap object
        }
        //if the current tile is a cheese tile ( bitwise comparison, because tile can be ground and cheese tile)
        if((currentTile.tileType & CatchingMiceTile.TileType.Cheese) == CatchingMiceTile.TileType.Cheese)
        {
            //begin eating the cheese
        }
    }
}
