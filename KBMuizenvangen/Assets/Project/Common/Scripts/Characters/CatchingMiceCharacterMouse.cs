using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCharacterMouse : ICatchingMiceCharacter
{
    
    public override void SetupLocal()
    {
        base.SetupLocal();

        
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

	}
    protected void OnEnable()
    {
        CatchingMiceLevelManager.use.CheeseRemoved += CheeseRemoved;
    }
    protected void OnDisable()
    {
        //
    }
    public override void GetTarget()
    {
        float smallestDistance = float.MaxValue;
        //Check which cheese tile is the closest
        foreach (CatchingMiceTile tile in CatchingMiceLevelManager.use.cheeseTiles)
        {
            float distance = Vector2.Distance(transform.position.v2(), tile.location.v2());
            if (distance < smallestDistance)
            {
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
    public void CheeseRemoved(CatchingMiceTile tile)
    {
        //only get new target when your target waypoint has been removed
        if (tile.waypoint != targetWaypoint)
            return;

        handle.StopRoutine();
        GetTarget();
    }
    public override void DoCurrentTileBehaviour()
    {
        //Debug.Log("Doing current tile " + currentTile +" behaviour " + currentTile.tileType );
        if ((currentTile.tileType & CatchingMiceTile.TileType.Trap) == CatchingMiceTile.TileType.Trap && currentTile.trapObject != null)
        {
            //get hit by trap object
            //process hit
            currentTile.trapObject.OnHit(this);
            if(health<=0)
            {
                Die();
            }
        }
        //if the current tile is a cheese tile ( bitwise comparison, because tile can be ground and cheese tile)
        if((currentTile.tileType & CatchingMiceTile.TileType.Cheese) == CatchingMiceTile.TileType.Cheese)
        {
            //begin eating the cheese
            Debug.Log("eating cheeese");
            handle.StartRoutine(Eat());
        }
    }
    public IEnumerator Eat()
    {
        CatchingMiceTile cheeseTile = currentTile;
        while(health > 0 && cheeseTile.trapObject != null && cheeseTile.trapObject.Stacks > 0)
        {
            Debug.Log(currentTile.trapObject.Stacks);
            cheeseTile.trapObject.Stacks -= damage;

            yield return new WaitForSeconds(0.5f);
        }
        if (CatchingMiceLevelManager.use.cheeseTiles.Count > 0)
        {
            Debug.Log("getting new target");
            //CatchingMiceLevelManager.use.RemoveTrapFromTile((int)currentTile.gridIndices.x, (int)currentTile.gridIndices.y);

            GetTarget();
        }
    }
    public void Die()
    {
        //Drop cookie
        //Play Death animation (cloud particle)
        CatchingMiceLevelManager.use.CheeseRemoved -= CheeseRemoved;
        handle.StopRoutine();
        gameObject.SetActive(false);
    }
}
