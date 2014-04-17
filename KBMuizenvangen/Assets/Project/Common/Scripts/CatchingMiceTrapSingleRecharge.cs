using UnityEngine;
using System.Collections;

public class CatchingMiceTrapSingleRecharge : ICatchingMiceTrapType 
{
    protected bool _recharged = true;

    void FixedUpdate() 
    {
        if (!CatchingMiceGameManager.use.GameRunning)
            return;

        if(_recharged)
            CheckForHit(); 
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
                //Debug.Log("Hitting enemy " + enemy.name);
                _recharged = false;
                ResetTimer();
                break;
            }
        }

    }

    public override void DoBehaviour()
    {
        //after interval time you can recharge the trap
        if (CatchingMiceGameManager.use.Timer - _startTime > interval)
        {
            _recharged = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position.yAdd(-_offset), new Vector3((tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, (tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, tileRange * CatchingMiceLevelManager.use.scale));
    }
}
