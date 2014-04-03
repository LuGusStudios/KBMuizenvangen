using UnityEngine;
using System.Collections;

public class CatchingMiceWorldObjectTrapGround : CatchingMiceWorldObject , ICatchingMiceWorldObjectTrap
{
    protected float _stacks = 3;
    protected float _cost = 1;
    protected float _damage = 1;

    public float Stacks
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

    public void OnHit( ICatchingMiceCharacter character)
    {
        character.Health -= _damage;
        Stacks--;
    }

    public void DestroySelf()
    {
        //remove the cheese from the list and tiletype
        CatchingMiceLevelManager.use.RemoveTrapFromTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y),tileType);
        gameObject.SetActive(false);
    }

    public override void SetTileType(CatchingMiceTile tile)
    {
        CatchingMiceTile levelTile = CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];
        //World objects are the furniture, ground traps cannot be set on furniture types
        if (tile.worldObject == null)
        {
            //Adds the furniture type to the tile with the or operator because a tile multiple types (ex. a tile can have a trap on a furniture)
            levelTile.tileType = levelTile.tileType | tileType;
            levelTile.trapObject = this;
            transform.position = transform.position.yAdd(gridOffset).zAdd(-0.25f);
        }
        else
        {
            Debug.LogError("Ground trap " + transform.name + " cannot be placed\non furniture tile " + tile.worldObject.name + " !");
        }
                    
    }    
}
