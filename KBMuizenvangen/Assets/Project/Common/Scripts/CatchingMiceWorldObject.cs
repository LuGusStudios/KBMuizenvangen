using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatchingMiceWorldObject : MonoBehaviour
{
	public float gridOffset = 0.5f;
	public CatchingMiceTile.TileType tileType = CatchingMiceTile.TileType.None;
	protected BoxCollider2D[] _BoxColliders2D;
	public CatchingMiceTile parentTile = null;

	public static bool CheckColliders(CatchingMiceWorldObject obj, Vector3 position)
	{
		// Checks whether the object's colliders overlaps with a tile that
		// is not valid (a null tile)
		
		obj.transform.position = position;
		BoxCollider2D[] boxColliders2D = obj.GetComponentsInChildren<BoxCollider2D>();

		if (boxColliders2D.Length == 0)
		{
			CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(obj.transform.position.x, obj.transform.position.y);
			
			if (tile == null)
			{
				return false;
			}
		}
		else
		{
			foreach (BoxCollider2D col2D in boxColliders2D)
			{
				float xTiles = Mathf.Ceil(col2D.size.x / CatchingMiceLevelManager.use.scale);
				float yTiles = Mathf.Ceil(col2D.size.y / CatchingMiceLevelManager.use.scale);

				for (int y = 1; y < (int)yTiles * 2; y += 2)
				{
					for (int x = 1; x < (int)xTiles * 2; x += 2)
					{
						//gets most left position of the collider and add the wanted tile distance
						float xTile = ((col2D.transform.position.x + col2D.center.x) - col2D.Bounds().extents.x) + xTiles / (xTiles * 2) * x;
						//Shifts the tile gridOffset down first, then gets the lowest position and add the wanted tile distance
						float yTile = ((col2D.transform.position.y + col2D.center.y) - col2D.Bounds().extents.y) - obj.gridOffset + yTiles / (yTiles * 2) * y;

						//Debug.Log("yTile : " + yTile); 
						CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTile(Mathf.RoundToInt(xTile / CatchingMiceLevelManager.use.scale), Mathf.RoundToInt(yTile / CatchingMiceLevelManager.use.scale));

						if (tile == null)
						{
							return false;
						}
					}
				}
			}
		}

		return true;
	}

	protected void Awake()
	{
		SetupLocal();
	}

	public virtual void SetupLocal()
	{

	}

	public virtual void SetupGlobal()
	{

	}

	public virtual void CalculateColliders()
	{
		_BoxColliders2D = GetComponentsInChildren<BoxCollider2D>();
		List<CatchingMiceTile> tileList = new List<CatchingMiceTile>();

		//gets the indices of box colliders
		foreach (BoxCollider2D col2D in _BoxColliders2D)
		{
			float xTiles = Mathf.Ceil(col2D.size.x / CatchingMiceLevelManager.use.scale);
			float yTiles = Mathf.Ceil(col2D.size.y / CatchingMiceLevelManager.use.scale);

			//needs to go over every other point
			for (int y = 1; y < (int)yTiles * 2; y += 2)
			{
				for (int x = 1; x < (int)xTiles * 2; x += 2)
				{
					//gets most left position of the collider and add the wanted tile distance
					float xTile = ((col2D.transform.position.x + col2D.center.x) - col2D.Bounds().extents.x) + xTiles / (xTiles * 2) * x;
					//Shifts the tile gridOffset down first, then gets the lowest position and add the wanted tile distance
					float yTile = ((col2D.transform.position.y + col2D.center.y) - col2D.Bounds().extents.y) - gridOffset + yTiles / (yTiles * 2) * y;
					//Debug.Log("yTile : " + yTile); 
					CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTile(Mathf.RoundToInt(xTile / CatchingMiceLevelManager.use.scale), Mathf.RoundToInt(yTile / CatchingMiceLevelManager.use.scale));

					tileList.Add(tile);
				}
			}
		}

		//when there is no boxcollider, then it only takes up 1 tile
		if (_BoxColliders2D.Length <= 0)
		{
			Debug.Log("no collider has been found. will be using 1 tile");
			CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(transform.position.x, transform.position.y);
			tileList.Add(tile);
		}

		Debug.Log(transform.name + tileList.Count);
		SetTileType(tileList);
	}

	public virtual void SetTileType(List<CatchingMiceTile> tiles)
	{
		foreach (CatchingMiceTile tile in tiles)
		{
			CatchingMiceTile levelTile = CatchingMiceLevelManager.use.levelTiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];
			levelTile.tileType = tileType;
			levelTile.worldObject = this;

			//the z axis will be the anchor point of the object. So the anchor point needs the be the lowest tile of the sprite
			levelTile.location.z = transform.position.z;
		}

	}
	// Use this for initialization
	void Start()
	{
		SetupGlobal();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
