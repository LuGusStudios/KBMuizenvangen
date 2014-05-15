using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatchingMiceLevelManager : LugusSingletonExisting<CatchingMiceLevelManagerDefault>
{

}

public class CatchingMiceLevelManagerDefault : MonoBehaviour
{
	// Accessors
	#region Accessors
	public Transform ObjectParent
	{
		get
		{
			return objectParent;
		}
	}
	public CatchingMiceLevelDefinition CurrentLevel
	{
		get
		{
			return currentLevel;
		}
		set
		{
			currentLevel = value;
		}
	}
	public int Width
	{
		get
		{
			return width;
		}
	}
	public int Height
	{
		get
		{
			return height;
		}
	}
	public CatchingMiceTile[,] Tiles
	{
		get
		{
			return tiles;
		}
	}
	public List<ICatchingMiceCharacter> Players
	{
		get
		{
			return playerList;
		}
	}
	public List<Waypoint> Waypoints
	{
		get
		{
			return waypointList;
		}
	}
	public List<CatchingMiceWaveDefinition> Waves
	{
		get
		{
			return wavesList;
		}
	}
	public List<CatchingMiceHole> MiceHoles
	{
		get
		{
			return miceHoles;
		}
	}
	public List<CatchingMiceTile> TrapTiles
	{
		get
		{
			return trapTiles;
		}
	}
	public List<CatchingMiceTile> CheeseTiles
	{
		get
		{
			return cheeseTiles;
		}
	}
	#endregion

	// Events
	public delegate void TrapRemovedEventHandler(CatchingMiceTile trapTile);
	public event TrapRemovedEventHandler TrapRemoved;

	public delegate void CheeseRemovedEventHandler(CatchingMiceTile cheeseTile);
	public event CheeseRemovedEventHandler CheeseRemoved;
	
	// Inspector
	public float scale = 1;

	// Prefabs
	#region Prefabs
	public ICatchingMiceCharacter[] characterPrefabs = null;
	public GameObject[] enemyPrefabs = null;
	public CatchingMiceFurniture[] furniturePrefabs = null;
	public CatchingMiceCheese[] cheesePrefabs = null;
	public CatchingMiceTrap[] trapPrefabs = null;
	public CatchingMiceHole[] holePrefabs = null;
	public GameObject[] cookiePrefabs = null;
	#endregion
	
	// Protected
	#region Protected
	protected Transform objectParent = null;
	protected Transform levelRoot = null;
	protected Transform levelParent = null;
	protected Transform navigationParent = null;
	protected Transform characterParent = null;
	protected Transform spawnParent = null;
	protected Transform enemyParent = null;

	protected CatchingMiceLevelDefinition currentLevel = null;
	protected int width = 13;
	protected int height = 13;

	protected CatchingMiceTile[,] tiles = null;

	protected List<ICatchingMiceCharacter> playerList = new List<ICatchingMiceCharacter>();
	protected List<Waypoint> waypointList = new List<Waypoint>();
	protected List<CatchingMiceWaveDefinition> wavesList = new List<CatchingMiceWaveDefinition>();
	protected List<GameObject> enemyParentList = new List<GameObject>();
	protected List<CatchingMiceTile> holeTiles = new List<CatchingMiceTile>();
	protected List<CatchingMiceTile> trapTiles = new List<CatchingMiceTile>();
	protected List<CatchingMiceTile> cheeseTiles = new List<CatchingMiceTile>();
	protected List<CatchingMiceHole> miceHoles = new List<CatchingMiceHole>();

	protected ILugusCoroutineHandle spawnRoutine = null;
	#endregion

	void Awake()
	{
		FindReferences();
	}
	void FindReferences()
	{
		// only do this once
		if (levelRoot != null)
		{
			return;
		}

		if (levelRoot == null)
		{
			levelRoot = GameObject.Find("LevelRoot").transform;

			if (levelRoot == null)
			{
				Debug.Log("LevelManager: Could not find level root.");
			}
		}


		levelParent = levelRoot.FindChild("LevelParent");
		objectParent = levelRoot.FindChild("ObjectParent");
		navigationParent = levelRoot.FindChild("NavigationParent");
		characterParent = levelRoot.FindChild("CharacterParent");
		enemyParent = levelRoot.FindChild("EnemyParent");
		spawnParent = levelRoot.FindChild("SpawnParent");

		spawnRoutine = LugusCoroutines.use.GetHandle();
	}

	public void ClearLevel()
	{

		Debug.Log("Clearing level (playing in editor).");
		for (int i = levelParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(levelParent.GetChild(i).gameObject);
		}
		for (int i = objectParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(objectParent.GetChild(i).gameObject);
		}
		for (int i = navigationParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(navigationParent.GetChild(i).gameObject);
		}
		for (int i = characterParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(characterParent.GetChild(i).gameObject);
		}
		for (int i = enemyParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(enemyParent.GetChild(i).gameObject);
		}
		for (int i = spawnParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(spawnParent.GetChild(i).gameObject);
		}

		tiles = null;
		playerList.Clear();
		waypointList.Clear();
		wavesList.Clear();
		enemyParentList.Clear();
		trapTiles.Clear();
		cheeseTiles.Clear();
		holeTiles.Clear();
		miceHoles.Clear();

		if (spawnRoutine != null)
		{
			spawnRoutine.StopRoutine();
		}
	}

	// Only used for testing and for quickly building a level
	// Level is build without characters and enemies
	// because the generate errors when starting a game and the level
	// gets cleared
	public void BuildLevelEditor()
	{
		if (currentLevel == null)
		{
			Debug.LogError("The level definition was null.");
			return;
		}

		FindReferences();

		ClearLevel();

		width = currentLevel.width;
		height = currentLevel.height;

		ParseTiles(width, height, currentLevel.layout);

		PlaceFurniture(currentLevel.tileItems);

		CreateGrid();

		PlaceWaypoints();

		PlaceTraps(currentLevel.traps);

		PlaceCheeses(currentLevel.cheeses);

		PlaceCharacters(currentLevel.characters);

		PlaceMiceHoles(currentLevel.holeItems);
	}

	public void BuildLevel()
	{
		if (currentLevel == null)
		{
			Debug.LogError("The level definition was null.");
			return;
		}

		FindReferences();

		ClearLevel();

		width = currentLevel.width;
		height = currentLevel.height;

		ParseTiles(width, height, currentLevel.layout);

		PlaceFurniture(currentLevel.tileItems);

		CreateGrid();

		PlaceWaypoints();

		PlaceTraps(currentLevel.traps);

		PlaceCheeses(currentLevel.cheeses);
		
		PlaceCharacters(currentLevel.characters);

		PlaceMiceHoles(currentLevel.holeItems);

		wavesList = new List<CatchingMiceWaveDefinition>(currentLevel.waves);
	}

	public void CreateGrid()
	{
		// iterate over entire grid
		for (int y = height - 1; y >= 0; y--)
		{
			for (int x = 0; x < width; x++)
			{
				if (tiles[x, y] == null)
				{
					continue;
				}

				GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				quad.gameObject.name = tiles[x, y].gridIndices.ToString();
				quad.transform.localScale = Vector3.one * 0.98f * scale;
				quad.transform.position = tiles[x, y].location;
				quad.transform.parent = levelParent;
				quad.GetComponent<MeshCollider>().enabled = false;


				////if (levelTiles[x, y].tileType != CatchingMiceTile.TileType.Ground)
				////{
				////    Material tempMaterial = new Material(quad.renderer.sharedMaterial);
				////    tempMaterial.color = Color.red;
				////    //quad.transform.position = levelTile.location.v3().z(-0.5f);
				////    quad.transform.localScale = Vector3.one * 1.1f * scale;
				////    quad.renderer.sharedMaterial = tempMaterial;
				////}

			}
		}

	}

	public void PlaceWaypoints()
	{
		// Iterate over entire grid
		for (int y = height - 1; y >= 0; --y)
		{
			for (int x = 0; x < width; ++x)
			{
				if (tiles[x, y] == null)
				{
					waypointList.Add(null);
					continue;
				}

				// Make child object with waypoint script
				GameObject wayPoint = new GameObject();
				wayPoint.gameObject.name = "Waypoint " + tiles[x, y].gridIndices;


				wayPoint.transform.parent = navigationParent.transform;
				wayPoint.transform.position = tiles[x, y].location;

				Waypoint wp = wayPoint.AddComponent<Waypoint>();

				if ((tiles[x, y].tileType & CatchingMiceTile.TileType.Furniture) == CatchingMiceTile.TileType.Furniture)
				{
					wp.waypointType = CatchingMiceTile.TileType.Furniture;
					CatchingMiceWorldObject catchingMiceObject = tiles[x, y].furniture;
					if (catchingMiceObject != null)
					{
						wayPoint.transform.position = wayPoint.transform.position.yAdd(catchingMiceObject.gridOffset);
					}
					else
					{
						Debug.LogError("Tile " + tiles[x, y].gridIndices + " is of type furniture, but misses script CatchingMiceWorldObject");
					}
				}

				wp.parentTile = tiles[x, y];
				tiles[x, y].waypoint = wp;
				waypointList.Add(wp);
			}
		}

		AssignNeighbours();
	}

	public void ParseTiles(int _width, int _height, string layout)
	{
		// Clear grid
		tiles = new CatchingMiceTile[_width, _height];

		// Iterate over entire grid
		for (int y = height - 1; y >= 0; --y)
		{
			for (int x = 0; x < width; ++x)
			{
				switch (layout[y * _width + x])
				{
					case 'x':
						tiles[x, y] = null;
						break;
					case 'o':
						CatchingMiceTile currentTile = new CatchingMiceTile();
						tiles[x, y] = currentTile;

						// Register this tile's grid indices and its true location, which is its index * scale
						currentTile.gridIndices = new Vector2(x, y);

						// We want to use the y position for the z axis because the tiles that are the highest are also the ones
						// that are the furthers away from us
						currentTile.location = currentTile.gridIndices.v3().z(y) * scale;
						break;
				}
			}
		}
	}

	protected void PlaceFurniture(CatchingMiceTileItemDefinition[] tileItemDefinitions)
	{
		foreach (CatchingMiceTileItemDefinition definition in tileItemDefinitions)
		{
			// Find the furniture prefab
			GameObject furniturePrefab = null;
			foreach (CatchingMiceFurniture go in furniturePrefabs)
			{
				if (go.name == definition.prefabName)
				{
					furniturePrefab = go.gameObject;
					break;
				}
			}

			if (furniturePrefab == null)
			{
				Debug.LogError("Did not find furniture ID: " + definition.prefabName);
				continue;
			}

			// Get the tile on which the furniture is to be placed
			CatchingMiceTile targetTile = GetTile(definition.position, false);
			if (targetTile == null)
			{
				Debug.LogError("Did not find tile with coordinates:" + definition.position + ". Skipping placing tile item: " + definition.prefabName);
				continue;
			}

			// Create the furniture item
			GameObject tileItem = (GameObject)Instantiate(furniturePrefab);
			tileItem.transform.parent = objectParent;
			tileItem.transform.name += " " + targetTile.gridIndices;
			tileItem.transform.localPosition = targetTile.location;

			// When placing the furniture, check if the it has the object script, so it can set the right tiles to furniture type
			CatchingMiceFurniture furniture = tileItem.GetComponent<CatchingMiceFurniture>();
			if (furniture != null)
			{
				if (furniture.CalculateColliders())
				{
					furniture.parentTile = targetTile;

					if ((targetTile.tileType & CatchingMiceTile.TileType.Furniture) != CatchingMiceTile.TileType.Furniture)
					{
						Debug.LogWarning("The tile type of the tile has no furniture flag set!");
					}
				}
				else
				{
					Debug.LogError("The furniture " + furniture.name + " could not be placed on the grid.");
					DestroyGameObject(furniture.gameObject);
				}
			}
			else
			{
				Debug.LogError("The furniture prefab " + furniturePrefab.name + " does not have a WorldObject component attached to it.");
				DestroyGameObject(tileItem);
			}
		}
	}
	
	protected void PlaceCheeses(CatchingMiceCheeseDefinition[] cheeseDefinitions)
	{
		foreach (CatchingMiceCheeseDefinition definition in cheeseDefinitions)
		{
			// Find the cheese prefab
			GameObject cheesePrefab = null;
			foreach (CatchingMiceCheese go in cheesePrefabs)
			{
				if (go.name == definition.prefabName)
				{
					cheesePrefab = go.gameObject;
					break;
				}
			}

			if (cheesePrefab == null)
			{
				Debug.LogError("Did not find tile item ID: " + definition.prefabName);
				return;
			}

			// Get the tile where the cheese is supposed to be placed
			CatchingMiceTile targetTile = GetTile(definition.position, false);
			if (targetTile == null)
			{
				Debug.LogError("Did not find tile with coordinates:" + definition.position + ". Skipping placing cheese: " + definition.prefabName);
				return;
			}

			// Create the cheese item
			GameObject tileItem = (GameObject)Instantiate(cheesePrefab);
			tileItem.transform.parent = objectParent;
			tileItem.transform.name += " " + targetTile.gridIndices;
			tileItem.transform.localPosition = targetTile.location;

			CatchingMiceCheese cheese = tileItem.GetComponent<CatchingMiceCheese>();
			if (cheese != null)
			{
				if (cheese.CalculateColliders())
				{
					cheese.parentTile = targetTile;

					if ((targetTile.tileType & CatchingMiceTile.TileType.Cheese) != CatchingMiceTile.TileType.Cheese)
					{
						Debug.LogWarning("The tile type of the tile has no cheese flag set!");
					}

					cheeseTiles.Add(targetTile);
					cheese.Stacks = definition.stacks;
				}
				else
				{
					Debug.LogError("The cheese " + cheese.name + " could not be placed on the grid.");
					DestroyGameObject(cheese.gameObject);
				}
			}
			else
			{
				Debug.LogError("The cheese prefab " + cheesePrefab.name + " does not have a Cheese component attached.");
				DestroyGameObject(tileItem);
			}
		}
	}

	protected void PlaceTraps(CatchingMiceTrapDefinition[] trapdefinitions)
	{
		foreach (CatchingMiceTrapDefinition definition in trapdefinitions)
		{
			if (string.IsNullOrEmpty(definition.prefabName))
			{
				Debug.LogError("The trap prefab name is null or empty!");
				continue;
			}

			// Find the trap prefab
			GameObject trapPrefab = null;
			foreach (CatchingMiceTrap go in trapPrefabs)
			{
				if (go.name == definition.prefabName)
				{
					trapPrefab = go.gameObject;
					break;
				}
			}

			if (trapPrefab == null)
			{
				Debug.LogError("Did not find trap prefab name: " + definition.prefabName);
				continue;
			}

			// Get the tile where the trap supposed to be placed
			CatchingMiceTile targetTile = GetTile(definition.position, false);
			if (targetTile == null)
			{
				Debug.LogError("Did not find tile with coordinates:" + definition.position + ". Skipping placing trap: " + definition.prefabName);
				continue;
			}

			// Create the trap item
			GameObject tileItem = (GameObject)Instantiate(trapPrefab);
			tileItem.transform.parent = objectParent;
			tileItem.transform.name += " " + targetTile.gridIndices;
			tileItem.transform.localPosition = targetTile.location;

			CatchingMiceTrap trap = tileItem.GetComponent<CatchingMiceTrap>();
			if (trap != null)
			{
				if (trap.CalculateColliders())
				{
					trap.parentTile = targetTile;

					if ((targetTile.tileType & CatchingMiceTile.TileType.Trap) != CatchingMiceTile.TileType.Trap)
					{
						Debug.LogWarning("The tile type of the tile has no trap flag set!");
					}

					trapTiles.Add(targetTile);
					trap.Stacks = definition.stacks;
				}
				else
				{
					Debug.LogError("The trap " + trap.name + " could not be placed on the grid.");
					DestroyGameObject(trap.gameObject);
				}
			}
			else
			{
				Debug.LogError("The trap prefab " + trapPrefab.name + " does not have a Trap component attached.");
				DestroyGameObject(tileItem);
			}
		}
	}
	
	public void PlaceMiceHoles(CatchingMiceHoleDefinition[] holeDefinitions)
	{
		if (holeDefinitions == null || holeDefinitions.Length < 1)
		{
			Debug.LogError("This level has no mice holes!");
			return;
		}

		foreach (CatchingMiceHoleDefinition definition in holeDefinitions)
		{
			if (string.IsNullOrEmpty(definition.prefabName))
			{
				Debug.LogError("The mice hole prefab name is null or empty!");
				continue;
			}

			// Find the hole prefab
			GameObject holePrefab = null;
			foreach (CatchingMiceHole go in holePrefabs)
			{
				if (go.name == definition.prefabName)
				{
					holePrefab = go.gameObject;
					break;
				}
			}

			if (holePrefab == null)
			{
				Debug.LogError("Did not find mice hole prefab name: " + definition.prefabName);
				return;
			}

			// Get the tile where the mice hole supposed to be placed
			CatchingMiceTile targetTile = GetTile(definition.position, false);
			if (targetTile == null)
			{
				Debug.LogError("Did not find tile with coordinates:" + definition.position + ". Skipping placing mice hole: " + definition.prefabName);
				continue;
			}

			// Create the trap item
			GameObject tileItem = (GameObject)Instantiate(holePrefab);
			tileItem.transform.parent = objectParent;
			tileItem.transform.name += " " + targetTile.gridIndices;
			tileItem.transform.localPosition = targetTile.location;

			CatchingMiceHole hole = tileItem.GetComponent<CatchingMiceHole>();
			if (hole != null)
			{
				if (hole.CalculateColliders())
				{
					hole.parentTile = targetTile;

					if ((targetTile.tileType & CatchingMiceTile.TileType.Hole) != CatchingMiceTile.TileType.Hole)
					{
						Debug.LogWarning("The tile type of the tile has no hole flag set!");
					}

					miceHoles.Add(hole);
					holeTiles.Add(targetTile);

					hole.id = definition.holeId;
					hole.SetHoleSpawnPoint(definition.startDirection, targetTile);
				}
				else
				{
					Debug.LogError("The mice hole " + hole.name + " could not be placed on the grid.");
					DestroyGameObject(hole.gameObject);
				}
			}
			else
			{
				Debug.LogError("The hole prefab " + holePrefab.name + " does not have a MiceHole component attached.");
				DestroyGameObject(tileItem);
			}
		}
	}

	protected void PlaceCharacters(CatchingMiceCharacterDefinition[] characterDefinitions)
	{
		if (characterDefinitions == null || characterDefinitions.Length == 0)
		{
			Debug.LogError("This level has no characters!");
			return;
		}

		foreach (CatchingMiceCharacterDefinition characterDefinition in characterDefinitions)
		{
			if (string.IsNullOrEmpty(characterDefinition.prefabName))
			{
				Debug.LogError("Character ID is null or empty!");
				continue;
			}

			ICatchingMiceCharacter characterPrefabFound = null;
			foreach (ICatchingMiceCharacter characterPrefab in characterPrefabs)
			{
				if (characterDefinition.prefabName == characterPrefab.gameObject.name)
				{
					characterPrefabFound = characterPrefab;
					break;
				}
			}

			if (characterPrefabFound == null)
			{
				Debug.LogError("Character prefab could not be found: " + characterDefinition.prefabName);
				continue;
			}

			// Get the tile where the trap supposed to be placed
			CatchingMiceTile targetTile = GetTile(characterDefinition.position, false);
			if (targetTile == null)
			{
				Debug.LogError("Did not find tile with coordinates:" + characterDefinition.position + ". Skipping placing character: " + characterDefinition.prefabName);
				continue;
			}

			ICatchingMiceCharacter characterSpawned = (ICatchingMiceCharacter)Instantiate(characterPrefabFound);

			characterSpawned.transform.parent = characterParent;
			characterSpawned.transform.localPosition = targetTile.waypoint.transform.position.zAdd(-characterSpawned.zOffset);
			characterSpawned.currentTile = targetTile;

			// set speed
			if (characterDefinition.speed < 0)
			{
				Debug.LogError("Speed is negative for character " + characterDefinition.prefabName + ". Setting speed to 1.");
				characterSpawned.speed = 1;
			}
			else
			{
				characterSpawned.speed = characterDefinition.speed;
			}

			playerList.Add(characterSpawned);
		}
	}

	public void InstantiateWave(int waveIndex)
	{
		if (waveIndex < 0)
		{
			Debug.LogError("Wave index can't be negative");
			return;
		}

		if (waveIndex >= wavesList.Count)
		{
			Debug.LogError("Wave exceeds the waves list. Using last wave count.");
			waveIndex = wavesList.Count - 1;
		}

		// Now it's not pooling so delete every child in enemy parent
		for (int i = enemyParent.childCount - 1; i >= 0; i--)
		{
			Destroy(enemyParent.GetChild(i).gameObject);
		}
		enemyParentList.Clear();

		CatchingMiceWaveDefinition wave = wavesList[waveIndex];
		int amountToKill = 0;

		for (int i = 0; i < wave.enemies.Length; i++)
		{
			// Get the prefab from string name
			if (string.IsNullOrEmpty(wave.enemies[i].prefabName))
			{
				Debug.LogError("Character ID is null or empty!");
				continue;
			}

			GameObject enemyPrefab = null;
			foreach (GameObject go in enemyPrefabs)
			{
				if (go.name == wave.enemies[i].prefabName)
				{
					enemyPrefab = go;
					break;
				}
			}

			if (enemyPrefab == null)
			{
				Debug.LogError("Did not find enemy prefab with ID: " + wave.enemies[i].prefabName);
				return;
			}

			//everything has been found, start new coroutine with spawnwave
			//LugusCoroutines.use.StartRoutine(SpawnSubWave(enemyPrefab, spawnTile, enemyDefinition.amount, enemyDefinition.spawnTimeInterval, enemyDefinition.spawnDelay));

			InstantiateSubWave(i, enemyPrefab, wave.enemies[i].amount);

			amountToKill += wave.enemies[i].amount;
		}
		//foreach (CatchingMiceEnemyDefinition enemyDefinition in wave.enemies)
		//{
		//    //get the prefab from string name
		//    if (string.IsNullOrEmpty(enemyDefinition.prefabName))
		//    {
		//        Debug.LogError("Character ID is null or empty!");
		//        continue;
		//    }
		//    GameObject enemyPrefab = null;
		//    foreach (GameObject go in enemyPrefabs)
		//    {
		//        if (go.name == enemyDefinition.prefabName)
		//        {
		//            enemyPrefab = go;
		//            break;
		//        }
		//    }
		//    if (enemyPrefab == null)
		//    {
		//        Debug.LogError("Did not find hole item ID: " + enemyDefinition.prefabName);
		//        return;
		//    }

		//    //get spawnlocation from hole id
		//    CatchingMiceHole spawnTile = null;
		//    foreach (CatchingMiceHole hole in holeTiles)
		//    {
		//        if(hole.id == enemyDefinition.holeId)
		//        {
		//            spawnTile = hole;
		//        }
		//    }
		//    if (spawnTile == null)
		//    {
		//        Debug.LogError("hole id has not been found for " + enemyDefinition.prefabName);
		//        return;
		//    }
		//    //everything has been found, start new coroutine with spawnwave
		//    LugusCoroutines.use.StartRoutine(SpawnSubWave(enemyPrefab,spawnTile, enemyDefinition.amount, enemyDefinition.spawnTimeInterval, enemyDefinition.spawnDelay));

		//    amountToKill += enemyDefinition.amount;
		//}

		CatchingMiceGameManager.use.SetAmountToKill(amountToKill);
	}

	protected void InstantiateSubWave(int index, GameObject spawnGO, int amount)
	{
		//if the object already exist use it, else make new one
		GameObject waveParent = null;

		foreach (GameObject enemyParent in enemyParentList)
		{
			if (enemyParent.name == "subwave" + index)
			{
				waveParent = enemyParent;
				break;
			}
		}

		if (waveParent == null)
		{
			waveParent = new GameObject("subwave" + index);
			//Debug.Log("Making new wave Parent " + waveParent);
		}

		waveParent.transform.position = new Vector3(-1000, -100, 0);
		waveParent.transform.parent = spawnParent;

		for (int i = 0; i < amount; i++)
		{
			GameObject enemy = GetNextEnemyFromPool(spawnGO);
			enemy.transform.parent = waveParent.transform;
			enemy.transform.localPosition = Vector3.zero;
			//enemy.SetActive(false);
		}

		enemyParentList.Add(waveParent);
	}

	public void SpawnInstantiatedWave(int waveIndex)
	{
		CatchingMiceWaveDefinition wave = wavesList[waveIndex];
		for (int i = 0; i < wave.enemies.Length; i++)
		{
			spawnRoutine.StartRoutine(SpawnInstantiatedSubWave(i, wave.enemies[i]));
		}
	}

	protected IEnumerator SpawnInstantiatedSubWave(int index, CatchingMiceEnemyDefinition enemy)
	{
		// 1. Find the spawn point for this enemy
		// 2. Find the subwave parent responsible for holding the game objects in the scene
		// 3. The enemies are one by one retrieved from the subwave parent, and place at the spawntile

		if (enemy.spawnDelay > 0)
			yield return new WaitForSeconds(enemy.spawnDelay);

		float spawnIntervalLower = enemy.spawnTimeInterval * 0.9f;
		float spawnIntervalUpper = enemy.spawnTimeInterval * 1.1f;

		// Get spawnlocation from hole id
		CatchingMiceHole spawnTile = null;
		foreach (CatchingMiceHole hole in miceHoles)
		{
			if (hole.id == enemy.holeId)
			{
				spawnTile = hole;
				break;
			}
		}

		if (spawnTile == null)
		{
			Debug.LogError("Could not find the mice hole with name " + enemy.holeId + " for enemy " + enemy.prefabName);
			yield break;
		}

		// Find the parent object container for this subwave
		GameObject parentGO = null;
		foreach (GameObject parent in enemyParentList)
		{
			if (parent == null)
			{
				Debug.LogError("parent objects is null");
				break;
			}
			if (parent.name == "subwave" + index)
			{
				parentGO = parent;
				break;
			}
		}

		if (parentGO == null)
		{
			Debug.LogError("subwaveName has not been found");
			yield break;
		}

		// 
		for (int i = 0; i < enemy.amount; i++)
		{
			Transform child = parentGO.transform.GetChild(0);
			
			if (child == null)
			{
				Debug.Log("No Child has been found");
				break;
			}
			
			CatchingMiceCharacterMouse mouseScript = child.GetComponent<CatchingMiceCharacterMouse>();
			
			if (mouseScript != null)
			{
				//child.gameObject.SetActive(true);
				child.transform.parent = enemyParent;
				child.transform.position = spawnTile.spawnPoint.zAdd(-mouseScript.zOffset);
				mouseScript.currentTile = spawnTile.parentTile;
				mouseScript.GetTarget();
			}
			
			yield return new WaitForSeconds(LugusRandom.use.Uniform.Next(spawnIntervalLower, spawnIntervalUpper));
		}
		
		yield break;
	}

	public GameObject GetNextEnemyFromPool(GameObject gameObjectToGet)
	{
		//TODO pooling: search for inactive objects before instantiating new gameobject
		return (GameObject)Instantiate(gameObjectToGet);
	}

	public void AssignNeighbours()
	{
		for (int i = 0; i < waypointList.Count - 1; i++)
		{
			if (waypointList[i] == null)
			{
				continue;
			}

			// Last row does not need to (and can't) add their neighbor below
			if ((i < waypointList.Count - width) && (waypointList[i + width] != null))
			{
				waypointList[i].neighbours.Add(waypointList[i + width]);
				waypointList[i + width].neighbours.Add(waypointList[i]);
			}

			// Last column can't add his neighbor next to him
			// all waypoints are in one list, so every width length there will be a new row, is that waypoint is in the last column
			if (((i + 1) % width != 0) && (waypointList[i + 1] != null))
			{
				waypointList[i].neighbours.Add(waypointList[i + 1]);
				waypointList[i + 1].neighbours.Add(waypointList[i]);
			}

		}
	}

	public GameObject GetPrefab(string name)
	{
		Transform child = spawnParent.FindChild(name);

		if (child != null)
			return child.gameObject;
		else
		{
			Debug.LogError("Child " + name + " not found.");
			return null;
		}
	}

	// Lookup methods--------------------------------------------------------------------

	// get tile by grid indices (contained in vector2)
	public CatchingMiceTile GetTile(Vector2 coords)
	{
		return GetTile(coords, false);
	}

	// Get tile by grid indices (contained in vector2)
	public CatchingMiceTile GetTile(Vector2 coords, bool clamp)
	{
		int x = Mathf.RoundToInt(coords.x);
		int y = Mathf.RoundToInt(coords.y);

		return GetTile(x, y, clamp);
	}

	// Get tile by grid indices
	public CatchingMiceTile GetTile(int x, int y)
	{
		return GetTile(x, y, false);
	}

	// Get tile by local position under level root
	public CatchingMiceTile GetTileByLocation(float x, float y)
	{
		int xIndex = Mathf.RoundToInt(x / scale);
		int yIndex = Mathf.RoundToInt(y / scale);

		// Instead of returning null, return closest tile?
		if (xIndex >= width)
		{
			xIndex = width - 1;
			//return null;
		}
		else if (xIndex < 0)
		{
			xIndex = 0;
			//return null;
		}
		if (yIndex >= height)
		{
			yIndex = height - 1;
			//return null;
		}
		else if (yIndex < 0)
		{
			yIndex = 0;
			//return null;
		}
		return GetTile(xIndex, yIndex, true);
	}

	public CatchingMiceTile GetTile(int x, int y, bool clamp)
	{
		if (tiles == null)
		{
			return null;
		}

		if (x >= width || x < 0)
		{
			if (clamp)
				x = Mathf.Clamp(x, 0, width - 1);
			else
				return null;
		}
		
		if (y >= height || y < 0)
		{
			if (clamp)
				y = Mathf.Clamp(y, 0, height - 1);
			else
				return null;
		}

		return tiles[x, y];
	}

	public Waypoint GetWaypointFromTile(Vector2 gridIndices)
	{
		return tiles[Mathf.RoundToInt(gridIndices.x), Mathf.RoundToInt(gridIndices.y)].waypoint;
	}

	public Waypoint GetWaypointFromTile(int x, int y)
	{
		return tiles[x, y].waypoint;
	}

	public CatchingMiceTile[] GetTileAround(CatchingMiceTile tile)
	{
		List<CatchingMiceTile> tiles = new List<CatchingMiceTile>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				//don't add your own tile
				if (x == 0 && y == 0)
					continue;
				CatchingMiceTile inspectedTile = GetTile(tile.gridIndices.v3().xAdd(x).yAdd(y));
				if (inspectedTile != null)
					tiles.Add(inspectedTile);
			}
		}

		return tiles.ToArray();
	}

	public void RemoveTrapFromTile(CatchingMiceTile tile)
	{
		if (tile == null)
		{
			Debug.LogError("Cannot remove trap from null tile.");
			return;
		}

		if ((tile.tileType & CatchingMiceTile.TileType.Trap) != CatchingMiceTile.TileType.Trap)
		{
			Debug.LogError("Cannot remove trap from tile " + tile.ToString() + " that does not contain a trap.");
			return;
		}

		// Remove the references of the trap
		trapTiles.Remove(tile);
		
		if((tile.trapObject != null) && (TrapRemoved != null))
		{
			TrapRemoved(tile);
		}

		tile.tileType = tile.tileType ^ CatchingMiceTile.TileType.Trap;
		tile.trapObject = null;
	}

	public void RemoveCheeseFromTile(CatchingMiceTile tile)
	{
		if (tile == null)
		{
			Debug.LogError("Cannot remove cheese from null tile.");
			return;
		}

		if ((tile.tileType & CatchingMiceTile.TileType.Cheese) != CatchingMiceTile.TileType.Cheese)
		{
			Debug.LogError("Cannot remove cheese from tile " + tile.ToString() + " that does not contain cheese.");
			return;
		}

		// Remove the references of the trap
		cheeseTiles.Remove(tile);

		if ((tile.cheese != null) && (CheeseRemoved != null))
		{
			CheeseRemoved(tile);
		}

		tile.tileType = tile.tileType ^ CatchingMiceTile.TileType.Cheese;
		tile.cheese = null;
	}

	protected void DestroyGameObject(GameObject obj)
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			obj.SetActive(false);
			Destroy(obj);
		}
		else
		{
			DestroyImmediate(obj);
		}
#endif
	}
}
