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
        BoxColliders2D = GetComponentsInChildren<BoxCollider2D>();
        Debug.Log(transform.position.x);
        //count indices of box colliders
        foreach (BoxCollider2D col2D in BoxColliders2D)
        {
            float xTiles = (int)col2D.size.x / CatchingMiceLevelManager.use.scale;
            float yTiles = (int)col2D.size.y / CatchingMiceLevelManager.use.scale;
            for (int y = (int)yTiles - 1; y >= 0; y--)
            {
                for (int x = 0; x < (int)xTiles; x++)
                {
                    //gets the anchor point of the parent, add it with the collider position
                    float xTile = transform.position.x + col2D.transform.localPosition.x + xTiles * (x / xTiles);
                    Debug.Log("xtile : " + xTile);
                    float yTile = transform.position.y + (col2D.transform.localPosition.y - gridOffset) + yTiles * (y / yTiles);
                    Debug.Log("yTile : " + yTile);
                    CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTile((int)xTile,(int) yTile);
                    Debug.Log(tile);
                    CatchingMiceLevelManager.use.levelTiles[(int)xTile,(int)yTile].tileType = CatchingMiceTile.TileType.Furniture;
                    
                }
            }
            
            //float x = col2D.center.x / col2D.size.x;
            //Debug.Log(x);
            //Debug.Log(col2D.transform.position.y);
            //float y = col2D.transform.position.y / col2D.size.y - (gridOffset * CatchingMiceLevelManager.use.scale);
            //Debug.Log(y);
            //CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(x, y);
            //Debug.Log(tile);
        }
        CatchingMiceLevelManager.use.CreateGrid();
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
