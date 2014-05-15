using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCharacterMouse : ICatchingMiceCharacter
{
    public delegate void OnGetHit();
    public event OnGetHit onGetHit;

    public int cheeseBites = 3;

	public override float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            
			if (onGetHit != null)
			{
                onGetHit();
			}

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
        

    }
    
	protected virtual void OnEnable() 
    {
		CatchingMiceLevelManager.use.CheeseRemoved += TargetRemoved;
    }
    
	protected virtual void OnDisable()
    {
        //CatchingMiceLevelManager.use.CheeseRemoved -= TargetRemoved;
    }
    
	public virtual void GetTarget()
    {
        if (CatchingMiceLevelManager.use.CheeseTiles.Count <= 0)
        {
            //Debug.LogWarning("No more cheese left!");
            return;
        }

        List<CatchingMiceTile> tiles = new List<CatchingMiceTile>(CatchingMiceLevelManager.use.CheeseTiles);
        targetWaypoint = GetTargetWaypoint(tiles);

        if (targetWaypoint != null)
        {
            CalculateTarget(targetWaypoint);
        }
        else
        {
            Debug.LogError("No target found");
        }
    }
   
    protected Waypoint GetTargetWaypoint(List<CatchingMiceTile> tileList)
    {
        Waypoint target = null;

        float smallestDistance = float.MaxValue;
        //Check which cheese tile is the closest
        foreach (CatchingMiceTile tile in tileList)
        {
            float distance = Vector2.Distance(transform.position.v2(), tile.location.v2());
            if (distance < smallestDistance)
            {
                smallestDistance = distance;

                target = tile.waypoint;
            }
        }

        if (target != null)
        {
            return target; 
        }
        else
        {
            Debug.LogError("No target found");
            return null;
        }
    }
    
	public void TargetRemoved(CatchingMiceTile tile)
    {
        // Current waypoint is null when pooling 
        if (targetWaypoint == null)
		{
            return;
		}

        StopCurrentBehaviour();
        //StopAllCoroutines(); 
        //handle.StopRoutine();
        GetTarget();
    }
    
	public override void DoCurrentTileBehaviour(int pathIndex)
    {
        //Debug.Log("Doing current tile " + currentTile +" behaviour " + currentTile.tileType );

        //if the current tile is a cheese tile ( bitwise comparison, because tile can be ground and cheese tile) and the last tile that it travelled
        if ((currentTile.tileType & CatchingMiceTile.TileType.Cheese) == CatchingMiceTile.TileType.Cheese && pathIndex==0)
        {
            //begin eating the cheese
            StartCoroutine(Attack());
        }
    }
    
	public override IEnumerator MoveToDestination(List<Waypoint> path)
    {
        yield return new WaitForSeconds(LugusRandom.use.Uniform.Next(0,0.5f));
        yield return StartCoroutine(base.MoveToDestination(path));
    }
    
	public override IEnumerator Attack()
    {
        attacking = true;
        OnHitEvent();
        CatchingMiceTile cheeseTile = currentTile;
        int attacked = 0;

        while((_health > 0)
			&& (cheeseTile.cheese != null)
			&& (cheeseTile.cheese.Stacks > 0)
			&& (attacked < cheeseBites))
        {
            cheeseTile.cheese.Stacks -= (int)damage;
            attacked++;

            yield return new WaitForSeconds(attackInterval);
        }

        attacking = false;

        if ((CatchingMiceLevelManager.use.CheeseTiles.Count > 0) && (cheeseTile.cheese.Stacks <= 0))
        {
            Debug.Log("getting new target");
            GetTarget();
        }
        else
        {

            DieRoutine();
        }
    }
    
	public virtual void DieRoutine()
    {
        //Drop cookie
        //TODO: Play Death animation (cloud particle)
        CatchingMiceLevelManager.use.CheeseRemoved -= TargetRemoved;
        CatchingMiceGameManager.use.ModifyAmountToKill(-1);
        //handle.StopRoutine();
        //Destroy(this.gameObject);
        gameObject.SetActive(false);
    }

    public void GetHit(float damage)
    {
        Health -= damage;
        if (onGetHit != null)
            onGetHit();
    }
}
