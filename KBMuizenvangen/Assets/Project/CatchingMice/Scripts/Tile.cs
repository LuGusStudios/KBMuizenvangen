using UnityEngine;
using System.Collections;

public class Tile
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
    public Vector2 location;
    public Vector2 gridIndices;

    public Tile()
	{
		tileType = TileType.Ground; 
		location = Vector2.zero;
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
