using System;
using UnityEngine;
using System.Collections;

public class CatchingMiceTile
{
    [Flags]
    public enum TileType
    {
        Ground = 1,
        Furniture = 2,
        Collide = 4,
        Trap  = 8,
        Hole  = 16,
        Cheese = 32,

        Both = 3,
        GroundTrap = 9,
        FurnitureTrap = 10,
        None = -1 // place at the bottom for nicer auto-complete in IDE
    }

    public CatchingMiceWorldObject worldObject = null;
    public ICatchingMiceWorldObjectTrap trapObject = null;
    public Waypoint waypoint = null;
    public TileType tileType = TileType.None;
    public Vector3 location;
    public Vector2 gridIndices;

    public CatchingMiceTile()
	{
		tileType = TileType.Ground;
        location = Vector3.zero;
		gridIndices = Vector2.zero;
	}
    public override string ToString ()
	{
		return "GameTile: " + gridIndices;
	}
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
