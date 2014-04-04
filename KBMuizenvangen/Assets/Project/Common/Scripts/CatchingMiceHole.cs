using UnityEngine;
using System.Collections;

public class CatchingMiceHole
{
    public string id = "";
    public ICatchingMiceCharacter.CharacterDirections spawnDirection = ICatchingMiceCharacter.CharacterDirections.Undefined;
    public Vector3 spawnPoint = Vector3.zero;
    public CatchingMiceTile parentTile = null;
    
    public CatchingMiceHole(ICatchingMiceCharacter.CharacterDirections direction, CatchingMiceTile tile)
    {
        spawnDirection = direction;
        parentTile = tile;
        float tileOffset = CatchingMiceLevelManager.use.scale;

        switch (spawnDirection)
        {
            case ICatchingMiceCharacter.CharacterDirections.Down:
                //you want to divide by 2 because you want your enemy to spawn right between 2 tiles, else the enemy appears to be floating 
                spawnPoint = parentTile.location.yAdd(tileOffset / 2);
                break;
            case ICatchingMiceCharacter.CharacterDirections.Left:
                spawnPoint = parentTile.location.xAdd(tileOffset);
                break;
            case ICatchingMiceCharacter.CharacterDirections.Right:
                spawnPoint = parentTile.location.xAdd(-tileOffset);
                break;
            case ICatchingMiceCharacter.CharacterDirections.Up:
                spawnPoint = parentTile.location.yAdd(-tileOffset);
                break;
            case ICatchingMiceCharacter.CharacterDirections.Undefined:
                Debug.LogError("Undefined direcion passed. Spawnpoint could not be made.");
                break;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
