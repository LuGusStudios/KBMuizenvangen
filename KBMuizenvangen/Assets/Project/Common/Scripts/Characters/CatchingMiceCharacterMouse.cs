using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCharacterMouse : ICatchingMiceCharacter
{
    public float health = 100.0f;
    
    public override void SetupLocal()
    {
        base.SetupLocal();

        currentTile = CatchingMiceLevelManager.use.GetTileByLocation(transform.position.x, transform.position.y);
        //walkable = Waypoint.WaypointType.Ground;

        
    }
    public override void SetupGlobal()
    {
        
    }

    protected void Awake()
    {
        SetupLocal();
    }

	// Use this for initialization
	protected void Start () 
    {
        SetupGlobal();
	}
	
	// Update is called once per frame
    protected void Update() 
    {
        //CatchingMiceTile newTile = CatchingMiceLevelManager.use.GetTileByLocation(transform.position.x, transform.position.y);
        ////do behaviours when new tile is hit
        //if(currentTile != newTile && newTile != null)
        //{
        //    currentTile = newTile;
        //    DoCurrentTileBehaviour();
        //}
	}

    public override void GetTarget()
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

                targetWaypoint = tile.waypoint;
            }
        }

        if (targetWaypoint != null)
        {
            base.GetTarget();
        }
        else
        {
            Debug.LogError("No target found");
        }
    }

    public override void DoCurrentTileBehaviour()
    {
        Debug.Log("Doing current tile " + currentTile +" behaviour " + currentTile.tileType );
        if(currentTile.trapObject != null)
        {
            //get hit by trap object
            //process hit
            
        }
        //if the current tile is a cheese tile ( bitwise comparison, because tile can be ground and cheese tile)
        if((currentTile.tileType & CatchingMiceTile.TileType.Cheese) == CatchingMiceTile.TileType.Cheese)
        {
            //begin eating the cheese
            Debug.Log("eating cheeese");
        }
    }
}
