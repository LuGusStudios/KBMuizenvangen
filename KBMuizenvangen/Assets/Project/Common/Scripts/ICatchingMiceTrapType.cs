using UnityEngine;
using System.Collections;

public abstract class ICatchingMiceTrapType : MonoBehaviour 
{
    public float interval = 2.0f;
    protected float _startTime = -1.0f;
    protected ICatchingMiceWorldObjectTrap trap = null;

    public int tileRange = 0;
    protected Vector2 _pointUpperLeft = Vector2.zero;
    protected Vector2 _pointLowerRight = Vector2.zero;
    protected float _offset = 0.0f; 

    public void Awake()
    {
        SetupLocal();
    }
    public virtual void SetupLocal()
    {
        trap = (ICatchingMiceWorldObjectTrap)transform.GetComponent(typeof(ICatchingMiceWorldObjectTrap));
        if (trap == null)
        {
            Debug.LogError("No trap has been found on this object, although trap type script is attached");
            return;
        }

        ResetTimer();
    }
    public virtual void SetupGlobal()
    {
        //get y offset
        CatchingMiceWorldObject furniture = null;
        furniture = transform.GetComponent<CatchingMiceWorldObject>().parentTile.worldObject; 

        if (furniture != null)
        {
            Debug.LogError(furniture.parentTile.worldObject);
            _offset = furniture.parentTile.worldObject.gridOffset;
        }

        _pointUpperLeft = transform.position.xAdd(-tileRange).yAdd(-tileRange - _offset).v2() * CatchingMiceLevelManager.use.scale;
        _pointLowerRight = transform.position.xAdd(tileRange).yAdd(tileRange - _offset).v2() * CatchingMiceLevelManager.use.scale;
    }
    public void ResetTimer()
    {
        _startTime = CatchingMiceGameManager.use.Timer;
    }

    public abstract void CheckForHit();
	// Use this for initialization
	void Start () 
    {
        SetupGlobal();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
