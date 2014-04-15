using UnityEngine;
using System.Collections;

public class CatchingMiceTrapAoE : MonoBehaviour
{
    public int tileRange = 1;
    public float interval = 2.0f;
    protected float _startTime = -1.0f;
    protected ICatchingMiceWorldObjectTrap trap;

    protected Vector2 _pointUpperLeft = Vector2.zero;
    protected Vector2 _pointLowerRight = Vector2.zero;
    public void Awake()
    {
        SetupLocal();

        
    }
    public void SetupLocal()
    {
        trap = (ICatchingMiceWorldObjectTrap)transform.GetComponent(typeof(ICatchingMiceWorldObjectTrap));
        if(trap == null)
        {
            Debug.LogError("No trap has been found on this object, although AoE script is attached");
            return;
        }

        _pointUpperLeft = transform.position.xAdd(-tileRange).yAdd(-tileRange).v2() * CatchingMiceLevelManager.use.scale;
        _pointLowerRight = transform.position.xAdd(tileRange).yAdd(tileRange).v2() * CatchingMiceLevelManager.use.scale;
        StartTimer();
    }
    public void StartTimer()
    {
        _startTime = CatchingMiceGameManager.use.Timer;
    }
    public void Update()
    {
        if (!CatchingMiceGameManager.use.GameRunning)
            return;


        if( CatchingMiceGameManager.use.Timer - _startTime > interval )
        {
            CheckCollision();
            StartTimer();
        }

    }

    public void CheckCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapAreaAll(_pointUpperLeft, _pointLowerRight);

        foreach (Collider2D collision in colliders)
        {
            CatchingMiceCharacterMouse enemy = collision.transform.parent.GetComponent<CatchingMiceCharacterMouse>();
            if(enemy != null)
            {
                enemy.GetHit(trap.Damage);
                Debug.Log(enemy.name);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3((tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, (tileRange * 2 + 1) * CatchingMiceLevelManager.use.scale, tileRange * CatchingMiceLevelManager.use.scale));
    }
}
