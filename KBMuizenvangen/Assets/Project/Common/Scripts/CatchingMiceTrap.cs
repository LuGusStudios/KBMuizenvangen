using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceTrap : CatchingMiceWorldObject, ICatchingMiceWorldObjectTrap
{
	[SerializeField]
	protected float health = 100.0f;
	[SerializeField]
	protected int stacks = 3;
	protected float cost = 1.0f;
	[SerializeField]
	protected float damage = 1.0f;

	public float Health
	{
		get
		{
			return health;
		}
		set
		{
			health = value;
			if (health <= 0)
			{
				DestroySelf();
			}
		}
	}
	public int Stacks
	{
		get
		{
			return stacks;
		}
		set
		{
			stacks = value;
			if (stacks <= 0)
			{
				DestroySelf();
			}
		}
	}
	public float Cost
	{
		get
		{
			return cost;
		}
		set
		{
			cost = value;
		}
	}
	public float Damage
	{
		get
		{
			return damage;
		}
		set
		{
			damage = value;
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
		character.Health -= damage;
	}

	public void DestroySelf()
	{
		// Remove the trap from the list and tiletype
		CatchingMiceLevelManager.use.RemoveTrapFromTile(Mathf.RoundToInt(parentTile.gridIndices.x), Mathf.RoundToInt(parentTile.gridIndices.y), tileType);
		gameObject.SetActive(false);
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
