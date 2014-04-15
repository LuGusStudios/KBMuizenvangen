using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatchingMiceWorldObjectTrapFurniture : CatchingMiceWorldObject , ICatchingMiceWorldObjectTrap
{
    protected int _stacks = 3;
    protected float _cost = 1.0f;
    [SerializeField]
    protected float _damage = 1.0f;

    public int Stacks
    {
        get
        {
            return _stacks;
        }
        set
        {
            _stacks = value;
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

    public void OnHit(ICatchingMiceCharacter character)
    {
        character.Health -= _damage;
        _stacks--;
    }
    public void DestroySelf()
    {
        //remove the cheese from the list and tiletype
        CatchingMiceLevelManager.use.RemoveTrapFromTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y),tileType);
        gameObject.SetActive(false);
    }
    public override void SetTileType(List<CatchingMiceTile> tiles)
    {
        foreach (CatchingMiceTile tile in tiles)
        {
            if (tile.worldObject == null)
            {
                Debug.LogError("Furniture trap " + transform.name + " cannot be placed\non ground tile " + tile.gridIndices + " !");
                return;
            }

            if((tileType & CatchingMiceTile.TileType.Ground) == CatchingMiceTile.TileType.Ground)
            {
                Debug.LogError("Furniture trap " + transform.name + " cannot be of type ground(trap).");
                return;
            }
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

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
