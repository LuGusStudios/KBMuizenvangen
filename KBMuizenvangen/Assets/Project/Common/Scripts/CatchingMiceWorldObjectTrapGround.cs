using UnityEngine;
using System.Collections;

public class CatchingMiceWorldObjectTrapGround : CatchingMiceWorldObject 
{
    public override void SetTileType(CatchingMiceTile tile)
    {
        CatchingMiceTile levelTile = CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];
        //World objects are the furniture, ground traps cannot be set on furniture types
        if (tile.worldObject == null)
        {
            //Adds the furniture type to the tile with the or operator because a tile multiple types (ex. a tile can have a trap on a furniture)
            levelTile.tileType = levelTile.tileType | tileType;
            levelTile.trapObject = this;
            transform.position = transform.position.yAdd(gridOffset).zAdd(-0.5f);
        }
        else
            Debug.LogError("Ground trap " + transform.name + " cannot be placed\non furniture tile " + tile.worldObject.name + " !");
        
    }
}
