using UnityEngine;
using System.Collections;

public class CatchingMiceWorldObjectTrap : CatchingMiceWorldObject {
    public override void SetTileType()
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
                    CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].tileType =
                        CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].tileType | CatchingMiceTile.TileType.Trap;
                    Debug.Log(CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].tileType);
                    CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].trapObject = this;
                    //when it's a ground tile then it does not have a worldObject variable, so check is needed
                    if (tile.worldObject != null)
                        gridOffset = tile.worldObject.gridOffset;
                    transform.position = transform.position.yAdd(gridOffset).zAdd(-0.5f);
                }
            }
        }
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
