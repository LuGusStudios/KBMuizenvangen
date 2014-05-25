using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Obstacles are world objects that can only be placed on furniture
// and are activated on a specific wave(s)
public abstract class CatchingMiceObstacle : CatchingMiceWorldObject
{
	public Sprite activeSprite = null;
	public Sprite inactiveSprite = null;
	public SpriteRenderer spriteRenderer = null;

	public virtual void SetupLocal()
	{

	}

	public virtual void SetupGlobal()
	{
		// Find the sprite renderer
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		if (spriteRenderer != null)
		{
			spriteRenderer.sprite = inactiveSprite;
		}
		else
		{
			Debug.LogError("Could not find the sprite renderer for the obstacle.");
		}
	}

	public void Awake()
	{
		SetupLocal();
	}

	protected void Start()
	{
		SetupGlobal();
	}

	public override void SetTileType(List<CatchingMiceTile> tiles)
	{
		foreach (CatchingMiceTile tile in tiles)
		{
			tile.tileType = tile.tileType | tileType;
			tile.obstacle = this;
		}

		transform.position = transform.position.yAdd(tiles[0].furniture.yOffset + yOffset).zAdd(-tiles[0].furniture.zOffset - zOffset);
	}

	public override bool ValidateTile(CatchingMiceTile tile)
	{
		if (!base.ValidateTile(tile))
		{
			return false;
		}

		if ((tile.furniture == null) || ((tile.tileType & CatchingMiceTile.TileType.Ground) == CatchingMiceTile.TileType.Ground))
		{
			Debug.LogError("Obstacle " + transform.name + " cannot be placed on the ground.");
			return false;
		} else if ((tile.obstacle != null) || ((tile.tileType & CatchingMiceTile.TileType.Obstacle) == CatchingMiceTile.TileType.Obstacle))
		{
			Debug.LogError("Obstacle " + transform.name + " cannot be placed because another obstacle is already present.");
			return false;
		}

		return true;
	}

	public abstract void FromXMLObstacleDefinition(string configuration);
}
