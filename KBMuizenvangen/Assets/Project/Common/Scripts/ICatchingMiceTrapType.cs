using UnityEngine;
using System.Collections;

public abstract class ICatchingMiceTrapType : MonoBehaviour 
{
    public float interval = 2.0f;
    protected float _startTime = -1.0f;
    protected ICatchingMiceWorldObjectTrap _trap = null;

    public int tileRange = 0;
    protected Vector2 _pointLeft = Vector2.zero;
    protected Vector2 _pointRight = Vector2.zero;
    protected float _offset = 0.0f; 

    public void Awake()
    {
        SetupLocal();
    }
    public virtual void SetupLocal()
    {
        _trap = (ICatchingMiceWorldObjectTrap)transform.GetComponent(typeof(ICatchingMiceWorldObjectTrap));
        if (_trap == null)
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
        furniture = _trap.TrapObject.parentTile.worldObject; // transform.GetComponent<CatchingMiceWorldObject>().parentTile.worldObject; 

        if (furniture != null)
        {
            //Debug.LogError(furniture.parentTile.worldObject);
            _offset = furniture.parentTile.worldObject.gridOffset;
        }

        //when traps are bigger than 1 tile, check the collider for its center
        BoxCollider2D trapCollider = null;
        trapCollider = transform.GetComponentInChildren<BoxCollider2D>();

        if(trapCollider == null)
        {
            //Debug.Log("No collider has been found. Using center of object");
            _pointLeft = transform.position.xAdd(-tileRange).yAdd(-tileRange - _offset).v2() * CatchingMiceLevelManager.use.scale;
            _pointRight = transform.position.xAdd(tileRange).yAdd(tileRange - _offset).v2() * CatchingMiceLevelManager.use.scale;
        }
        else
        {
            //get middle location from one tile
            Vector3 tile = Vector3.zero;
            
            //only do this when range is bigger then 1, if not just use collider bounds
            if(tileRange>1)
                tile = (CatchingMiceLevelManager.use.scale * Vector3.one / 2);

            //gets the bound from the first and last tile in world space
            Vector3 trapBoundLeft = trapCollider.transform.TransformPoint(trapCollider.Bounds().min) + tile;
            Vector3 trapBoundRight = trapCollider.transform.TransformPoint(trapCollider.Bounds().max) - tile;
           
            _pointLeft = trapBoundLeft.xAdd(-tileRange).yAdd(-tileRange - _offset).v2() * CatchingMiceLevelManager.use.scale;
            _pointRight = trapBoundRight.xAdd(tileRange).yAdd(tileRange - _offset).v2() * CatchingMiceLevelManager.use.scale;
        }
       
    }
    public void ResetTimer()
    {
        _startTime = CatchingMiceGameManager.use.Timer;
    }

    public virtual void CheckForHit() 
    {
        //override for custom behaviours
    }

    public virtual void DoBehaviour()
    {
        //override for custom behaviours
    }
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
