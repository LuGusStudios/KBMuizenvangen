using UnityEngine;
using System.Collections;

public class CatchingMiceHole : MonoBehaviour
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
    public CatchingMiceTile parentTile = null;
    
    public void SetHoleSpawnPoint(CharacterDirections direction, CatchingMiceTile tile)
    {
        spawnDirection = direction;
        parentTile = tile;
        float tileOffset = CatchingMiceLevelManager.use.scale;

        switch (spawnDirection)
        {
            case CharacterDirections.Down:
                //you want to divide by 2 because you want your enemy to spawn right between 2 tiles, else the enemy appears to be floating 
                spawnPoint = parentTile.location.yAdd(tileOffset / 2);
                break;
            case CharacterDirections.Left:
                spawnPoint = parentTile.location.xAdd(tileOffset);
                break;
            case CharacterDirections.Right:
                spawnPoint = parentTile.location.xAdd(-tileOffset);
                break;
            case CharacterDirections.Up:
                spawnPoint = parentTile.location.yAdd(-tileOffset);
                break;
            case CharacterDirections.Undefined:
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
