using UnityEngine;
using System.Collections;

public class CatchingMiceWorldObjects : MonoBehaviour
{

    public float gridOffset = 0.5f;
    protected Vector2[] Gridindices;
    protected BoxCollider2D[] BoxColliders2D;
    
    protected void Awake()
    {
        SetupLocal();
    }
    
    public virtual void SetupLocal()
    {
       
    }

    public virtual void SetupGlobal()
    {
        
    }

    public void SetTileType()
    {
        BoxColliders2D = GetComponentsInChildren<BoxCollider2D>();
        //gets the indices of box colliders
        foreach (BoxCollider2D col2D in BoxColliders2D)
        {
            float xTiles = Mathf.Ceil(col2D.size.x / CatchingMiceLevelManager.use.scale);
            float yTiles = Mathf.Ceil(col2D.size.y / CatchingMiceLevelManager.use.scale);
            //TODO: needs to go over every other point not every point
            for (int y = 1; y < (int)yTiles * 2; y++)
            {
                for (int x = 1; x < (int)xTiles * 2; x++)
                {
                    //gets most left position of the collider and add the wanted tile distance
                    float xTile = ((col2D.transform.position.x + col2D.center.x) - col2D.Bounds().extents.x) + xTiles / (xTiles * 2) * x;
                    float yTile = ((col2D.transform.position.y + col2D.center.y) - col2D.Bounds().extents.y) - gridOffset + yTiles / (yTiles * 2) * y;
                    //Debug.Log("yTile : " + yTile); 
                    CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTile(Mathf.RoundToInt(xTile / CatchingMiceLevelManager.use.scale), Mathf.RoundToInt(yTile / CatchingMiceLevelManager.use.scale));
                    Debug.Log(tile + " " + gameObject.name);
                    CatchingMiceLevelManager.use.levelTiles[Mathf.RoundToInt(xTile / CatchingMiceLevelManager.use.scale), Mathf.RoundToInt(yTile / CatchingMiceLevelManager.use.scale)].tileType = CatchingMiceTile.TileType.Furniture;

                }
            }
        }
    }

    // Use this for initialization
	void Start ()
	{
	    SetupGlobal();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
