using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatchingMiceWorldObjectTrapFurniture : CatchingMiceWorldObject, ICatchingMiceWorldObjectTrap
{
	[SerializeField]
	protected float _health = 100.0f;
	[SerializeField]
	protected int _stacks = 3;
	protected float _cost = 1.0f;
	[SerializeField]
	protected float _damage = 1.0f;

	public float Health
	{
		get
		{
			return _health;
		}
		set
		{
			_health = value;
			if (_health <= 0)
			{
				DestroySelf();
			}
		}
	}
	public int Stacks
	{
		get
		{
			return _stacks;
		}
		set
		{
			_stacks = value;
			if (_stacks <= 0)
			{
				DestroySelf();
			}
		}
	}

	public float Cost
	{
		get
		{
			return _cost;
		}
		set
		{
			_cost = value;
		}
	}

	public float Damage
	{
		get
		{
			return _damage;
		}
		set
		{
			_damage = value;
		}
	}
	public CatchingMiceWorldObject TrapObject
	{
		get
		{
			return this;
		}
	}
	public void OnHit(ICatchingMiceCharacter character)
	{
		character.Health -= _damage;
	}
	public void DestroySelf()
	{
		//remove the cheese from the list and tiletype
		CatchingMiceLevelManager.use.RemoveTrapFromTile(Mathf.RoundToInt(parentTile.gridIndices.x), Mathf.RoundToInt(parentTile.gridIndices.y), tileType);
		gameObject.SetActive(false);
	}
	public override void SetTileType(List<CatchingMiceTile> tiles)
	{
		//check if every tile can be placed first before applying the types to the level tiles
		foreach (CatchingMiceTile tile in tiles)
		{
			if (tile.worldObject == null)
			{
				Debug.LogError("Furniture trap " + transform.name + " cannot be placed\non ground tile " + tile.gridIndices + " !");
				return;
			}

			if ((tileType & CatchingMiceTile.TileType.Ground) == CatchingMiceTile.TileType.Ground)
			{
				Debug.LogError("Furniture trap " + transform.name + " cannot be of type ground(trap).");
				return;
			}
		}
		//every tile can be placed
		foreach (CatchingMiceTile tile in tiles)
		{
			CatchingMiceTile levelTile = CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];

			//Adds the furniture type to the tile with the or operator because a tile multiple types (ex. a tile can have a trap on a furniture)
			levelTile.tileType = levelTile.tileType | tileType;
			if (levelTile.trapObject == null)
			{
				levelTile.trapObject = this;
			}

			//when it's a ground tile then it does not have a worldObject variable, so check is needed

			gridOffset = tile.worldObject.gridOffset;
			transform.position = transform.position.yAdd(gridOffset).zAdd(-0.5f);
		}
	}
	public void DoBehaviour()
	{
		ICatchingMiceTrapType[] traptypes = GetComponentsInChildren<ICatchingMiceTrapType>();
		foreach (ICatchingMiceTrapType traptype in traptypes)
		{
			traptype.DoBehaviour();
		}
	}
}
