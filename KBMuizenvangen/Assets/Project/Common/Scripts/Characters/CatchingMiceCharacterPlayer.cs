using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCharacterPlayer : ICatchingMiceCharacter
{
    protected CatchingMiceCharacterMouse _enemy = null;
    protected bool _canAttack = true;
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
    }

    public IEnumerator CalculatePath(List<Waypoint> pathFromMouse)
    {
        //go to target
        List<Waypoint> graph = navigationGraph;
        bool fullPath = false;
        Waypoint currentWaypoint = null;
        float speed = 1.0f;

        for (int i = 1; i < pathFromMouse.Count ; i++)
        {
            
            currentWaypoint = pathFromMouse[i - 1];

            targetWaypoint = pathFromMouse[i];

            //Debug.LogError("current waypoint " + currentWaypoint + " targetWaypoint " + targetWaypoint);

            speed = Vector2.Distance(targetWaypoint.parentTile.gridIndices , currentWaypoint.parentTile.gridIndices) ;
            if (speed <= 0)
                speed = 1;
            List<Waypoint> path = AStarCalculate(graph, currentWaypoint, targetWaypoint, out fullPath, walkable);

            LugusCoroutines.use.StartRoutine(MoveToDestination(path));
            
            while (moving)
                yield return null;
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
            _enemy.Health -= damage;
            _canAttack = false;
            yield return new WaitForSeconds(attackInterval);
            _canAttack = true;
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
                    Debug.Log("Getting hits " + hit.transform);
                    //attack the mouse
                    LugusCoroutines.use.StartRoutine(Attack());
                    break;
                }
            }
        }
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
