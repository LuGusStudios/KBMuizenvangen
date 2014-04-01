using UnityEngine;
using System.Collections;

public class CatchingMiceWorldObjectTrapFurniture : CatchingMiceWorldObject 
{
    public override void SetTileType(CatchingMiceTile tile)
    {
        CatchingMiceTile levelTile = CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];
        if(tile.worldObject != null)
        {
            //Adds the furniture type to the tile with the or operator because a tile multiple types (ex. a tile can have a trap on a furniture)
            levelTile.tileType = levelTile.tileType | tileType;
            if(levelTile.trapObject == null)
            {
                levelTile.trapObject = this;
            }
            
            //when it's a ground tile then it does not have a worldObject variable, so check is needed

            gridOffset = tile.worldObject.gridOffset;
            transform.position = transform.position.yAdd(gridOffset).zAdd(-0.5f);         
        }
        else
            Debug.LogError("Furniture trap " + transform.name + " cannot be placed\non ground tile " + tile.gridIndices + " !");
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
