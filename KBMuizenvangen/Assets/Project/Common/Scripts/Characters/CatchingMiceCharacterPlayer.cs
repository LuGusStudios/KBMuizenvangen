using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCharacterPlayer : ICatchingMiceCharacter
{
    protected CatchingMiceCharacterMouse _enemy = null;
    protected bool _canAttack = true;

    protected ILugusCoroutineHandle walkHandle = null;
    public override float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }
    
    public override void SetupLocal()
    {
        base.SetupLocal();
        zOffset = 0.95f;
        walkHandle = LugusCoroutines.use.GetHandle();
    }

    public IEnumerator CalculatePath(List<Waypoint> pathFromMouse)
    {
        //go to target
        List<Waypoint> graph = navigationGraph;
        bool fullPath = false;
        Waypoint currentWaypoint = null;

        

        for (int i = 1; i < pathFromMouse.Count; i++)
        {
            if (interrupt)
                break;

            currentWaypoint = pathFromMouse[i - 1];

            targetWaypoint = pathFromMouse[i];

            //Debug.LogError("current waypoint " + currentWaypoint + " targetWaypoint " + targetWaypoint);

            List<Waypoint> path = AStarCalculate(graph, currentWaypoint, targetWaypoint, out fullPath, walkable);

            yield return walkHandle.StartRoutine(MoveToDestination(path));

        }

        
    }

    public override void DoCurrentTileBehaviour(int pathIndex)
    {

    }
    public override IEnumerator Attack()
    {
        //attack a mouse and wait until you can attack again
        if(_enemy != null)
        {
            attacking = true;
            _enemy.GetHit(damage);
            _canAttack = false;
            OnHitEvent();
            yield return new WaitForSeconds(attackInterval);
            _canAttack = true;
            attacking = false; 
        }
    }
	
    public void CheckForAttack()
    {
        // check all raycast hits
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + new Vector3(0, 0, 1000), this.transform.forward);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.parent != null)
            {
                _enemy = hit.transform.parent.GetComponent<CatchingMiceCharacterMouse>();
                //First mouse found, kill it and end
                if (_enemy != null)
                {
                    //attack the mouse
                    LugusCoroutines.use.StartRoutine(Attack());
                    break;
                }
            }
        }
    }
    public override void StopCurrentBehaviour()
    {
        base.StopCurrentBehaviour();
        
        if (walkHandle != null)
            walkHandle.StopRoutine(); 

        gameObject.StopTweens();
        interrupt = false;
    }
    public void MoveWithPath(List<Waypoint> path)
    {
        StopCurrentBehaviour();

        //post process before actually going through the path
        int count = 2;
        while (count < path.Count)
        {
            Vector2 wpStart = path[count - 2].parentTile.gridIndices;
            Vector2 wpMiddle = path[count - 1].parentTile.gridIndices;
            Vector2 wpEnd = path[count].parentTile.gridIndices;


            //check if the tile in between is on the same x or y axis
            if ((wpEnd.x == wpMiddle.x && wpMiddle.x == wpStart.x) ||
                (wpEnd.y == wpMiddle.y && wpMiddle.y == wpStart.y))
            {
                //check if the middle tile is inbetween the tiles
                if ((wpEnd.x > wpMiddle.x && wpMiddle.x > wpStart.x) ||
                     (wpEnd.y > wpMiddle.y && wpMiddle.y > wpStart.y))
                { 
                    path.RemoveAt(count - 1);
                    continue;
                }
            }

            count++;
            //when there are less the 3 waypoints to check 
            if (path.Count < 2)
                break;

        }

        //Debug.Log("path count : " + path.Count);
        StartCoroutine(CalculatePath(path));
        //handle.StartRoutine(CalculatePath(path));
    }
	// Update is called once per frame
	protected void Update () 
    {
	    if(!moving && _canAttack)
        {
            CheckForAttack();
        }
	}
}
