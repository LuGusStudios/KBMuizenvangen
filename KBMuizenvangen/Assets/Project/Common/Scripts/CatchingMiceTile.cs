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
    
	public TileType tileType = TileType.None;

    public CatchingMiceFurniture furniture = null;
    public CatchingMiceTrap trapObject = null;
	public CatchingMiceCheese cheese = null;
	public CatchingMiceHole hole = null;
    public Waypoint waypoint = null;
    
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
}
