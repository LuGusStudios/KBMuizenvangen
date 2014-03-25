﻿using UnityEngine;
using System.Collections;

public class CatchingMiceTile
{
    public enum TileType
    {
        None,
        Ground,
        Furniture,
        Collide
    }

    public GameObject rendered;
    public TileType tileType;
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
