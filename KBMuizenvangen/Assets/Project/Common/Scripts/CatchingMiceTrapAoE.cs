using UnityEngine;
using System.Collections;

public class CatchingMiceTrapAoE : ICatchingMiceTrapType
{
    public void Update()
    {
        if (!CatchingMiceGameManager.use.GameRunning)
            return;


        if( CatchingMiceGameManager.use.Timer - _startTime > interval )
        {
            CheckForHit();
            ResetTimer();
        }

    }

    public override void CheckForHit()
    {
        Collider2D[] colliders = Physics2D.OverlapAreaAll(_pointUpperLeft, _pointLowerRight);

        foreach (Collider2D collision in colliders)
        {
            CatchingMiceCharacterMouse enemy = collision.transform.parent.GetComponent<CatchingMiceCharacterMouse>();
            if(enemy != null)
            {
                trap.OnHit(enemy);
                //trap.Stacks--;
                Debug.Log("Hitting enemy " + enemy.name);
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position.yAdd(-_offset), new Vector3((tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, (tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, tileRange * CatchingMiceLevelManager.use.scale));
    }
}
