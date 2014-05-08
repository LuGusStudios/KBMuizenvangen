﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatchingMiceWorldObjectTrapFurniture : CatchingMiceTrap
{
	public override void SetTileType(List<CatchingMiceTile> tiles)
	{
		// Every tile can be placed
		foreach (CatchingMiceTile tile in tiles)
		{
			// TODO: Figure out why the tile is searched for again...?
			// !!!!! Warning: this could potentially lead to a neighboring tile being selected!
			// When a grid index is at i.e. 17, but the system might represent this as 16.9999999, rounding off here to 16!!!
			/*CatchingMiceTile levelTile = CatchingMiceLevelManager.use.tiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];

			if (levelTile != tile)
			{
				Debug.LogError("OMG: Wut is this sorcery?");
			}

			// Adds the trap type to the tile with the or operator because a tile multiple types (ex. a tile can have a trap on a furniture)
			levelTile.tileType = levelTile.tileType | tileType;
			
			// When it's a ground tile then it does not have a worldObject variable, so check is needed
			if (levelTile.trapObject == null)
			{
				levelTile.trapObject = this;
			}

			// Grid offset of the furniture. Used to position the trap correctly on the furniture
			gridOffset = tile.worldObject.gridOffset;

			transform.position = transform.position.yAdd(gridOffset).zAdd(-0.5f);*/

			tile.tileType = tile.tileType | tileType;
			tile.trapObject = this;
		}
		
		// A z-displacement of -0.5 is needed because the furniture is at -0.25f
		transform.position = transform.position.yAdd(tiles[0].furniture.gridOffset).zAdd(-0.5f);
	}

	public override bool ValidateTile(CatchingMiceTile tile)
	{
		if (!base.ValidateTile(tile))
		{
			return false;
		}

		if ((tile.furniture == null) || ((tile.tileType & CatchingMiceTile.TileType.Ground) == CatchingMiceTile.TileType.Ground))
		{
			Debug.LogError("Furniture trap " + transform.name + " cannot be placed on the ground.");
			return false;
		}
		else if ((tile.trapObject != null) || ((tile.tileType & CatchingMiceTile.TileType.Trap) == CatchingMiceTile.TileType.Trap))
		{
			Debug.LogError("Furniture trap " + transform.name + " cannot be placed because another trap is already present.");
			return false;
		}

		return true;
	}
}
