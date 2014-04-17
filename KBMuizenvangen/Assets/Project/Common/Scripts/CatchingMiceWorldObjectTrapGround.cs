using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceWorldObjectTrapGround : CatchingMiceWorldObject , ICatchingMiceWorldObjectTrap
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
            if(_stacks <= 0 )
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
    public void OnHit( ICatchingMiceCharacter character)
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
            //World objects are the furniture, ground traps cannot be set on furniture types
            if (tile.worldObject != null) 
            {
                Debug.LogError("Ground trap " + transform.name + " cannot be placed\non furniture tile " + tile.worldObject.name + " !");
                return;
            }

            if ((tileType & CatchingMiceTile.TileType.Furniture) == CatchingMiceTile.TileType.Furniture)
            {
                Debug.LogError("Ground trap " + transform.name + " cannot be of type Furniture(trap).");
                return;
            }           
        }
        //every tile can be placed
        foreach (CatchingMiceTile tile in tiles)
        {
            CatchingMiceTile levelTile = CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];

            //Adds the furniture type to the tile with the or operator because a tile multiple types (ex. a tile can have a trap on a furniture)
            levelTile.tileType = levelTile.tileType | tileType;
            levelTile.trapObject = this;
            transform.position = transform.position.yAdd(gridOffset).zAdd(-0.25f);
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
