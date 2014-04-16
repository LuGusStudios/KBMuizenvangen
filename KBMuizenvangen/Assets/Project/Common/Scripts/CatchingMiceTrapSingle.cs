using UnityEngine;
using System.Collections;

public class CatchingMiceTrapSingle : ICatchingMiceTrapType 
{

    public override void SetupLocal()
    {
        base.SetupLocal();
        
        _startTime = CatchingMiceGameManager.use.Timer - interval;
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!CatchingMiceGameManager.use.GameRunning)
            return;


        if (CatchingMiceGameManager.use.Timer - _startTime > interval)
        {
            CheckForHit();
           
        }
	}

    public override void CheckForHit()
    {
        Collider2D hit = Physics2D.OverlapArea(_pointUpperLeft, _pointLowerRight);
        if (hit != null) 
        {
            ICatchingMiceCharacter enemy = hit.transform.parent.GetComponent<CatchingMiceCharacterMouse>();
            if (enemy != null)
            {
                Debug.Log("Hitting enemy " + enemy.name);
                trap.OnHit(enemy);
                trap.Stacks--;

                ResetTimer(); 
            }
            
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position.yAdd(-_offset), new Vector3((tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, (tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, tileRange * CatchingMiceLevelManager.use.scale));
    }
}
