using UnityEngine;
using System.Collections;

public class CatchingMiceWorldObject : MonoBehaviour
{

    public float gridOffset = 0.5f;
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

    public virtual void SetTileType()
    {
        BoxColliders2D = GetComponentsInChildren<BoxCollider2D>();
        //gets the indices of box colliders
        foreach (BoxCollider2D col2D in BoxColliders2D)
        {
            float xTiles = Mathf.Ceil(col2D.size.x / CatchingMiceLevelManager.use.scale);
            float yTiles = Mathf.Ceil(col2D.size.y / CatchingMiceLevelManager.use.scale);
            //TODO: needs to go over every other point, not every point
            for (int y = 1; y < (int)yTiles * 2; y++)
            {
                for (int x = 1; x < (int)xTiles * 2; x++)
                {
                    //gets most left position of the collider and add the wanted tile distance
                    float xTile = ((col2D.transform.position.x + col2D.center.x) - col2D.Bounds().extents.x) + xTiles / (xTiles * 2) * x;
                    //Shifts the tile gridOffset down first, then gets the lowest position and add the wanted tile distance
                    float yTile = ((col2D.transform.position.y + col2D.center.y) - col2D.Bounds().extents.y) - gridOffset + yTiles / (yTiles * 2) * y;
                    //Debug.Log("yTile : " + yTile); 
                    CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTile(Mathf.RoundToInt(xTile / CatchingMiceLevelManager.use.scale), Mathf.RoundToInt(yTile / CatchingMiceLevelManager.use.scale));

                    //Adds the furniture type to the tile with the or operator because a tile multiple types (ex. a tile can have a trap on a furniture)
                    CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].tileType =  CatchingMiceTile.TileType.Furniture;

                    CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int) tile.gridIndices.y]
                        .worldObject = this;
                    //the z axis will be the anchor point of the object. So the anchor point needs the be the lowest tile of the sprite
                    CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].location.z = transform.position.z;
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
