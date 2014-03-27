using UnityEngine;
using System.Collections;

public class CatchingMiceWorldObjectTrap : CatchingMiceWorldObject 
{
    public override void SetTileType(CatchingMiceTile tile)
    {
        //Adds the furniture type to the tile with the or operator because a tile multiple types (ex. a tile can have a trap on a furniture)
        CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].tileType =
            CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].tileType | tileType;
        Debug.Log(CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].tileType);
        CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y].trapObject = this;
        //when it's a ground tile then it does not have a worldObject variable, so check is needed
        if (tile.worldObject != null)
            gridOffset = tile.worldObject.gridOffset;
        transform.position = transform.position.yAdd(gridOffset).zAdd(-0.5f);         
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
