using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCharacterMouse : ICatchingMiceCharacter
{
    public int timesToAttack = 3;
    public override float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                DieRoutine();
            }
        }
    }
    public override void SetupLocal()
    {
        base.SetupLocal();

        zOffset = 0.75f;
        //walkable = Waypoint.WaypointType.Ground;

        
    }
    public override void SetupGlobal()
    {
        
    }

    protected void OnEnable()
    {
        CatchingMiceLevelManager.use.CheeseRemoved += CheeseRemoved;
    }
    protected void OnDisable()
    {
        //CatchingMiceLevelManager.use.CheeseRemoved -= CheeseRemoved;
    }
    public override void GetTarget()
    {
        if (CatchingMiceLevelManager.use.cheeseTiles.Count <= 0)
            return;

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
    public override void DoCurrentTileBehaviour(int pathIndex)
    {
        //Debug.Log("Doing current tile " + currentTile +" behaviour " + currentTile.tileType );
        if ((currentTile.tileType & CatchingMiceTile.TileType.Trap) == CatchingMiceTile.TileType.Trap && currentTile.trapObject != null)
        {
            //get hit by trap object
            //process hit
            currentTile.trapObject.OnHit(this);
            
        }
        //if the current tile is a cheese tile ( bitwise comparison, because tile can be ground and cheese tile) and the last tile that it travelled
        if ((currentTile.tileType & CatchingMiceTile.TileType.Cheese) == CatchingMiceTile.TileType.Cheese && pathIndex==0)
        {
            handle.StopRoutine();
            //begin eating the cheese
            //Debug.Log("eating cheeese");
            handle.StartRoutine(Attack());
        }
    }
    public override IEnumerator Attack()
    {
        CatchingMiceTile cheeseTile = currentTile;
        int attacked = 0;
        while(_health > 0 && cheeseTile.trapObject != null && cheeseTile.trapObject.Stacks > 0 && attacked < timesToAttack)
        {
            //Debug.Log(currentTile.trapObject.Stacks);
            cheeseTile.trapObject.Stacks -= damage;

            attacked++;
            yield return new WaitForSeconds(attackInterval);
        }
        if (CatchingMiceLevelManager.use.cheeseTiles.Count > 0 && cheeseTile.trapObject.Stacks <= 0)
        {
            Debug.Log("getting new target");

            GetTarget();
        }
        else
        {
            //mouse ate the cheese x timesToAttack, ate too much, now die
            DieRoutine();
        }
    }
    public void DieRoutine()
    {
        //Drop cookie
        //Play Death animation (cloud particle)
        CatchingMiceLevelManager.use.CheeseRemoved -= CheeseRemoved;
        CatchingMiceGameManager.use.ModifyAmountToKill(-1);
        handle.StopRoutine();
        //Destroy(this.gameObject);
        gameObject.SetActive(false);
    }
}
