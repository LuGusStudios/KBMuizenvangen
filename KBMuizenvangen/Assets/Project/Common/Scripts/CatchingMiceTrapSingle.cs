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
        Collider2D[] colliders = Physics2D.OverlapAreaAll(_pointLeft, _pointRight);

        foreach (Collider2D collision in colliders)
        {
            CatchingMiceCharacterMouse enemy = collision.transform.parent.GetComponent<CatchingMiceCharacterMouse>();
            if (enemy != null)
            {
                _trap.OnHit(enemy);
                _trap.Stacks--;
                Debug.Log("Hitting enemy " + enemy.name);
                ResetTimer();
                break;
            }
        }
       
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position.yAdd(-_offset), new Vector3((tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, (tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, tileRange * CatchingMiceLevelManager.use.scale));
    }
}
