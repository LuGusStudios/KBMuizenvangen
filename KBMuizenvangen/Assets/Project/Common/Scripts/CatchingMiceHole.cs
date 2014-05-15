using UnityEngine;
using System.Collections;

public class CatchingMiceHole : CatchingMiceWorldObject
{
    public enum CharacterDirections
    {
        Up = 1,			// 0001
        Right = 2,		// 0010
        Down = 4,		// 0100
        Left = 8,		// 1000

        Undefined = -1
    }

    public string id = "";
    public CharacterDirections spawnDirection = CharacterDirections.Undefined;
    public Vector3 spawnPoint = Vector3.zero;
    
    public void SetHoleSpawnPoint(CharacterDirections direction, CatchingMiceTile tile)
    {
        spawnDirection = direction;
        float tileOffset = CatchingMiceLevelManager.use.scale;

		// Relocate and rotate the mice hole based on its direction
        switch (spawnDirection)
        {
            case CharacterDirections.Down:
                //you want to divide by 2 because you want your enemy to spawn right between 2 tiles, else the enemy appears to be floating 
                spawnPoint = parentTile.location.yAdd(tileOffset / 2);
                break;
            case CharacterDirections.Left:
                spawnPoint = parentTile.location.xAdd(tileOffset);
				transform.Rotate(new Vector3(0, 0, -90));
                break;
            case CharacterDirections.Right:
                spawnPoint = parentTile.location.xAdd(-tileOffset);
				transform.Rotate(new Vector3(0, 0, 90));
                break;
            case CharacterDirections.Up:
                spawnPoint = parentTile.location.yAdd(-tileOffset);
				transform.localScale = transform.localScale.y(-1);
                break;
            case CharacterDirections.Undefined:
                Debug.LogError("Undefined direction passed. Mice hole could not be made.");
                break;
        }
    }

	public override void SetTileType(System.Collections.Generic.List<CatchingMiceTile> tiles)
	{
		foreach (CatchingMiceTile tile in tiles)
		{
			tile.tileType = tile.tileType | tileType;
			tile.hole = this;
		}

		// Here we don't apply a vertical grid offset, because the sprite placement
		// already complements that
		transform.position = transform.position.zAdd(-0.1f);
	}

	public override bool ValidateTile(CatchingMiceTile tile)
	{
		// A mice hole should only be placed on the ground

		if (!base.ValidateTile(tile))
		{
			return false;
		}

		if ((tile.tileType & CatchingMiceTile.TileType.Furniture) == CatchingMiceTile.TileType.Furniture)
		{
			Debug.LogError("Mice hole " + transform.name + " cannot be placed on furniture.");
			return false;
		}

		return true;
	}
}
