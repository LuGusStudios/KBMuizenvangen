using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatchingMiceLevelManager : LugusSingletonExisting<CatchingMiceLevelManagerDefault>
{
    
}

public class CatchingMiceLevelManagerDefault : MonoBehaviour 
{

    protected Transform _levelRoot = null;
    protected Transform _levelParent = null;
    protected Transform _objectParent = null;
    protected Transform _navigationParent = null;
    protected Transform _characterParent = null;
    protected Transform _spawnParent = null;
    protected Transform _enemyParent = null;

    public CatchingMiceLevelDefinition[] levels = null;
    public int width = 13;
    public int height = 13;

    public CatchingMiceTile[,] levelTiles;
    public float scale = 1;

    public ICatchingMiceCharacter[] characterPrefabs = null;
    public GameObject[] enemyPrefabs = null;
    public GameObject[] tileItems = null;
    public GameObject[] cheeseItems = null;
    public GameObject[] holeItems = null;

    [HideInInspector]
    public List<Waypoint> waypointList = new List<Waypoint>();
    [HideInInspector]
    public List<CatchingMiceWaveDefinition> wavesList = new List<CatchingMiceWaveDefinition>();
    protected List<GameObject> _enemyList = new List<GameObject>();

    public List<CatchingMiceHole> holeTiles = new List<CatchingMiceHole>();

    public List<CatchingMiceTile> cheeseTiles = new List<CatchingMiceTile>();
    public delegate void CheeseRemovedEventHandler(CatchingMiceTile cheeseTile);
    public event CheeseRemovedEventHandler CheeseRemoved;
    void Awake()
    {
        FindReferences();
    }
    void FindReferences()
    {
        // only do this once
        if (_levelRoot != null)
            return;

        if (_levelRoot == null)
            _levelRoot = GameObject.Find("LevelRoot").transform;

        if (_levelRoot == null)
            Debug.Log("LevelManager: Could not find level root.");

        _levelParent = _levelRoot.FindChild("LevelParent");
        _objectParent = _levelRoot.FindChild("ObjectParent");
        _navigationParent = _levelRoot.FindChild("NavigationParent");
        _characterParent = _levelRoot.FindChild("CharacterParent");
        _enemyParent = _levelRoot.FindChild("EnemyParent");
        _spawnParent = _levelRoot.FindChild("SpawnParent"); 
    }

    public void ClearLevel()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            ClearLevelIsPlaying();
        }
        else
        {

            Debug.Log("Clearing level (playing in editor).");
            for (int i = _levelParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_levelParent.GetChild(i).gameObject);
            }
            for (int i = _objectParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_objectParent.GetChild(i).gameObject);
            }
            for (int i = _navigationParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_navigationParent.GetChild(i).gameObject);
            }
            for (int i = _characterParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_characterParent.GetChild(i).gameObject);
            }
            for (int i = _enemyParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_enemyParent.GetChild(i).gameObject);
            }
            for (int i = _spawnParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_spawnParent.GetChild(i).gameObject);
            }
            waypointList.Clear();
            cheeseTiles.Clear();
            holeTiles.Clear();
        }
#else
		ClearLevelIsPlaying();
		
        //for (int i = characterParent.childCount - 1; i >= 0; i--) 
        //{
        //    // NOTE: Ideally, destroying this gameObject would mean it is immediately gone!
        //    // But Destroy() is delayed until the end of the Update loop, which means the scan for new player characters will still find them!
        //    // So: Make sure to also set them disabled, which will make them invisible to GetComponents (provided includeInactive is false)
        //    if (characterParent.GetChild(i).GetComponent<PacmanCharacter>() != null)
        //    {
        //        characterParent.GetChild(i).GetComponent<PacmanCharacter>().enabled = false;
        //        characterParent.GetChild(i).gameObject.SetActive(false);
        //    }
        //    Destroy(characterParent.GetChild(i).gameObject);
        //}
#endif
    }

    public void ClearLevelIsPlaying()
    {
        Debug.Log("Clearing level (build).");
        for (int i = _levelParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_levelParent.GetChild(i).gameObject);
        }
        for (int i = _objectParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_objectParent.GetChild(i).gameObject);
        }
        for (int i = _navigationParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_navigationParent.GetChild(i).gameObject);
        }
        for (int i = _characterParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_characterParent.GetChild(i).gameObject);
        }
        for (int i = _enemyParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_enemyParent.GetChild(i).gameObject);
        }
        for (int i = _spawnParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_spawnParent.GetChild(i).gameObject);
        }
        waypointList.Clear();
        cheeseTiles.Clear();
        holeTiles.Clear();
    }
    

    // only used for testing and for quickly building a level
    public void BuildLevelDebug(int _width, int _height)
    {
        FindReferences();

        ClearLevel();

        width = _width;
        height = _height;

        ParseLevelTiles(_width, _height);

        //PlaceLevelTileItemsDebug();

        CreateGrid();


    }
    public void BuildLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Level index was out of bounds!");
            return;
        }

        CatchingMiceLevelDefinition level = levels[levelIndex];

        FindReferences();

        ClearLevel();

        width = level.width;
        height = level.height;

        ParseLevelTiles(width, height);

        PlaceLevelTileItems(level.tileItems);

        PlaceLevelCheeses(level.cheeses);

        CreateGrid();

        PlaceWaypoints();

        PlaceCharacters(level.characters);

        PlaceEnemySpawnpoint(level.holeItems);

        wavesList = new List<CatchingMiceWaveDefinition>(level.waves);
    }
    public void CreateGrid()
    {
        // iterate over entire grid
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.gameObject.name = levelTiles[x, y].gridIndices.ToString();
                quad.transform.localScale = Vector3.one * 0.98f * scale;
                quad.transform.position = levelTiles[x,y].location;
                quad.transform.parent = _levelParent;
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
        // iterate over entire grid
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                //make child object with waypoint script
                GameObject wayPoint = new GameObject();
                wayPoint.gameObject.name = "Waypoint " + levelTiles[x, y].gridIndices;


                wayPoint.transform.parent = _navigationParent.transform;
                wayPoint.transform.position = levelTiles[x, y].location;

                Waypoint wp = wayPoint.AddComponent<Waypoint>();

                if ((levelTiles[x, y].tileType & CatchingMiceTile.TileType.Furniture) == CatchingMiceTile.TileType.Furniture) 
                {
                    wp.waypointType = Waypoint.WaypointType.Furniture;
                    CatchingMiceWorldObject catchingMiceObject = levelTiles[x, y].worldObject;
                    if (catchingMiceObject != null)
                    {
                        wayPoint.transform.position = wayPoint.transform.position.yAdd(catchingMiceObject.gridOffset);
                    }
                    else
                    {
                        Debug.LogError("Tile " + levelTiles[x,y].gridIndices + " is of type furniture, but misses script CatchingMiceWorldObject");
                    }
                }
                wp.parentTile = levelTiles[x, y];
                levelTiles[x, y].waypoint = wp;
                waypointList.Add(wp);
            }
        }

        AssignNeighbours();
    }
   
    public void ParseLevelTiles( int _width, int _height)
    {
        // clear grid
        levelTiles = new CatchingMiceTile[_width, _height];

        // iterate over entire grid
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                CatchingMiceTile currentTile = new CatchingMiceTile();
                levelTiles[x, y] = currentTile;

                // register this tile's grid indices and its true location, which is its index * scale
                currentTile.gridIndices = new Vector2(x, y);
                //We want to use the y position for the z axis because the tiles that are the highest are also the ones
                //that are the furthers away from us
                currentTile.location = currentTile.gridIndices.v3().z(y) * scale;

            }
        }
    }
    protected void PlaceLevelTileItems(CatchingMiceTileItemDefinition[] tileItemDefinitions)
    {
        foreach (CatchingMiceTileItemDefinition definition in tileItemDefinitions)
        {
            GameObject tileItemPrefab = null;
            foreach (GameObject go in tileItems)
            {
                if (go.name == definition.prefabName)
                {
                    tileItemPrefab = go;
                    break;
                }
            }

            if (tileItemPrefab == null)
            {
                Debug.LogError("Did not find tile item ID: " + definition.prefabName);
                return;
            }

            CatchingMiceTile targetTile = GetTile(definition.tileCoordinates, false);
            
            if (targetTile == null)
            {
                Debug.LogError("Did not find tile with coordinates:" + definition.tileCoordinates + ". Skipping placing tile item: " + definition.prefabName);
                return;
            }

            GameObject tileItem = (GameObject)Instantiate(tileItemPrefab);
            tileItem.transform.parent = _objectParent;
            tileItem.transform.name += " " + targetTile.gridIndices;
            tileItem.transform.localPosition = targetTile.location.z(targetTile.location.z);

            //When placing the furniture, check if the it has the object script, so it can set the right tiles to furniture type
            CatchingMiceWorldObject tileObjectScript = tileItem.GetComponent<CatchingMiceWorldObject>();
            if (tileObjectScript != null)
            {
                tileObjectScript.CalculateColliders();
            }
            else
            {
                Debug.LogError("Did not find script " + tileObjectScript.ToString() + ". Item will not affect tiles " );
            }
        }
    }
    protected void PlaceLevelCheeses(CatchingMiceCheeseDefinition[] cheeseDefinitions)
    {
        foreach (CatchingMiceCheeseDefinition definition in cheeseDefinitions)
        {
            GameObject tileItemPrefab = null;
            foreach (GameObject go in cheeseItems)
            {
                if (go.name == definition.prefabName)
                {
                    tileItemPrefab = go;
                    break;
                }
            }

            if (tileItemPrefab == null)
            {
                Debug.LogError("Did not find tile item ID: " + definition.prefabName);
                return;
            }

            CatchingMiceTile targetTile = GetTile(definition.tileCoordinates, false);

            if (targetTile == null)
            {
                Debug.LogError("Did not find tile with coordinates:" + definition.tileCoordinates + ". Skipping placing tile item: " + definition.prefabName);
                return;
            }

            GameObject tileItem = (GameObject)Instantiate(tileItemPrefab);
            tileItem.transform.parent = _objectParent;
            tileItem.transform.name += " " + targetTile.gridIndices;
            tileItem.transform.localPosition = targetTile.location.z(targetTile.location.z);
            targetTile.tileType = targetTile.tileType | CatchingMiceTile.TileType.Cheese;

            //When placing the furniture, check if the it has the object script, so it can set the right tiles to furniture type
            CatchingMiceWorldObjectTrapGround tileObjectScript = tileItem.GetComponent<CatchingMiceWorldObjectTrapGround>();
            if (tileObjectScript != null)
            {
                tileObjectScript.CalculateColliders();

                cheeseTiles.Add(targetTile);

                tileObjectScript.Stacks = definition.stacks;
            }
            else
            {
                Debug.LogError("Did not find script " + tileObjectScript.ToString() + ". Item will not affect tiles ");
            }
        }
    }
    protected void PlaceCharacters(CatchingMiceCharacterDefinition[] characterDefinitions)
    {
        if (characterDefinitions == null || characterDefinitions.Length < 1)
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

            CatchingMiceTile startTile = GetTile(characterDefinition.xLocation, characterDefinition.yLocation);

            if (startTile == null)
            {
                Debug.LogWarning("Character " + characterDefinition.prefabName + " was placed on a non-existing tile. Not placing.");
                return;
            }

            ICatchingMiceCharacter characterSpawned = (ICatchingMiceCharacter)Instantiate(characterPrefabFound);

            characterSpawned.transform.parent = _characterParent;
            characterSpawned.transform.localPosition = startTile.waypoint.transform.position.zAdd(-characterSpawned.zOffset);
            characterSpawned.currentTile = startTile;

            // set speed
            if (characterDefinition.timeToReachTile < 0)
            {
                Debug.LogError("Speed is negative for character " + characterDefinition.prefabName + ". Setting speed to 1.");
                characterSpawned.timeToReachTile = 1;
            }
            else
            {
                characterSpawned.timeToReachTile = characterDefinition.timeToReachTile;
            }
        }
    }
    public void PlaceEnemySpawnpoint(CatchingMiceHoleDefinition[] holeDefinitions)
    {
        if (holeDefinitions == null || holeDefinitions.Length < 1)
        {
            Debug.LogError("This level has no characters!");
            return;
        }

        foreach (CatchingMiceHoleDefinition holeDefinition in holeDefinitions)
        {
            if (string.IsNullOrEmpty(holeDefinition.prefabName))
            {
                Debug.LogError("Character ID is null or empty!");
                continue;
            }
            GameObject holeItemPrefab = null;
            foreach (GameObject go in holeItems)
            {
                if (go.name == holeDefinition.prefabName)
                {
                    holeItemPrefab = go;
                    break;
                }
            }
            if (holeItemPrefab == null)
            {
                Debug.LogError("Did not find hole item ID: " + holeDefinition.prefabName);
                return;
            }

            CatchingMiceTile spawnTile = GetTile(holeDefinition.tileCoordinates);

            //Placing tile
            spawnTile.tileType = spawnTile.tileType | CatchingMiceTile.TileType.Hole;
            CatchingMiceHole holeSpawnpoint = new CatchingMiceHole(holeDefinition.startDirection, spawnTile);
            holeSpawnpoint.id = holeDefinition.holeId;
            holeSpawnpoint.spawnDirection = holeDefinition.startDirection;
            holeTiles.Add(holeSpawnpoint);

            GameObject holeItem = (GameObject)Instantiate(holeItemPrefab);
            holeItem.transform.parent = _levelParent;
            holeItem.transform.position = holeSpawnpoint.parentTile.location;

            //place the right orientation of the prefab
            switch (holeSpawnpoint.spawnDirection)
            {
                case ICatchingMiceCharacter.CharacterDirections.Down:
                    //this is the right rotation already
                    break;
                case ICatchingMiceCharacter.CharacterDirections.Left:
                    holeItem.transform.Rotate(new Vector3(0, 0, -90));
                    break;
                case ICatchingMiceCharacter.CharacterDirections.Right:
                    holeItem.transform.Rotate(new Vector3(0, 0, 90));
                    break;
                case ICatchingMiceCharacter.CharacterDirections.Up:
                    holeItem.transform.localScale = holeItemPrefab.transform.localScale.y(-1);
                    break;
                case ICatchingMiceCharacter.CharacterDirections.Undefined:
                    Debug.LogError("Undefined direcion passed. Spawnpoint could not be made.");
                    break;
            }

            Debug.Log("hole added : " + holeSpawnpoint.spawnPoint + " with parent location " + holeSpawnpoint.parentTile.location + " and direction " + holeSpawnpoint.spawnDirection);
          
        }
    }
    public void InstantiateWave(int waveIndex)
    {
        if (waveIndex < 0)
        {
            Debug.LogError("wave can't be negative");
            return;
        }
        if (waveIndex >= wavesList.Count)
        {
            Debug.LogError("wave exceeds the waves list. useing last wave count.");
            waveIndex = wavesList.Count - 1;
        }

        //now it's not pooling so delete every child in enemy parent
        for (int i = _enemyParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_enemyParent.GetChild(i).gameObject);
        }

        CatchingMiceWaveDefinition wave = wavesList[waveIndex];
        int amountToKill = 0;
        for (int i = 0; i < wave.enemies.Length; i++)
        {
            //get the prefab from string name
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
                Debug.LogError("Did not find hole item ID: " + wave.enemies[i].prefabName);
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
        GameObject waveParent = GameObject.Find("subwave"+index);
        if(waveParent == null)
            waveParent = new GameObject("subwave"+index);

        waveParent.transform.position = new Vector3(-1000, -100, 0);
        waveParent.transform.parent = _spawnParent;
         
        for (int i = 0; i < amount; i++)
        {
            GameObject enemy = GetNextEnemyFromPool(spawnGO);
            enemy.transform.parent = waveParent.transform;
            enemy.transform.localPosition = Vector3.zero;
        }

        _enemyList.Add(waveParent);
    }
    public void SpawnInstantiatedWave(int waveIndex)
    {
        CatchingMiceWaveDefinition wave = wavesList[waveIndex];
        for (int i = 0; i < wave.enemies.Length; i++)
        {
           LugusCoroutines.use.StartRoutine(SpawnInstantiatedSubWave(i, wave.enemies[i]));
        }
    }
    protected IEnumerator SpawnInstantiatedSubWave(int index, CatchingMiceEnemyDefinition enemy)
    {
        if (enemy.spawnDelay > 0)
            yield return new WaitForSeconds(enemy.spawnDelay);

        float spawnIntervalLower = enemy.spawnTimeInterval * 0.9f;
        float spawnIntervalUpper = enemy.spawnTimeInterval * 1.1f;

        //get spawnlocation from hole id
        CatchingMiceHole spawnTile = null;
        foreach (CatchingMiceHole hole in holeTiles)
        {
            if (hole.id == enemy.holeId)
            {
                spawnTile = hole;
                break;
            }
        }
        if (spawnTile == null)
        {
            Debug.LogError("hole id has not been found for " + enemy.prefabName);
            yield break;
        }
        GameObject parentGO = null;
        foreach (GameObject parent in _enemyList)
        {
            if(parent.name == "subwave"+index)
            {
                parentGO = parent;
                break;
            }
        }
        if(parentGO==null)
        {
            Debug.LogError("subwaveName has not been found");
            yield break;
        }
        for (int i = 0; i < enemy.amount; i++)
        {
            Transform child = parentGO.transform.GetChild(0);
            CatchingMiceCharacterMouse mouseScript = child.GetComponent<CatchingMiceCharacterMouse>();
            if (mouseScript != null)
            {
                child.transform.parent = _enemyParent;
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
        for (int i = 0; i < waypointList.Count-1; i++)
        {
            //Debug.Log("Adding waypoint : " + (i + width) + " to " + i);
            //Debug.Log("Adding waypoint : " + (i+1) + " to " + i);
            //Debug.Log(WaypointList.Count);
             
            //last row does not need to (and can't) add their neighbor below
            if (i < waypointList.Count - width)
            {
                waypointList[i].neighbours.Add(waypointList[i + width]);
                waypointList[i + width].neighbours.Add(waypointList[i]);
            }
            //Last column can't add his neighbor next to him
            //all waypoints are in one list, so every width length there will be a new row, is that waypoint is in the last column
            if ((i + 1) % width != 0)
            {
                waypointList[i].neighbours.Add(waypointList[i + 1]);
                waypointList[i + 1].neighbours.Add(waypointList[i]);
            }
            
        }
    }
    public GameObject GetPrefab(string name)
    {
        Transform child = _spawnParent.FindChild(name);

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

    // get tile by grid indices (contained in vector2)
    public CatchingMiceTile GetTile(Vector2 coords, bool clamp)
    {
        int x = Mathf.RoundToInt(coords.x);
        int y = Mathf.RoundToInt(coords.y);

        return GetTile(x, y, clamp);
    }

    // get tile by grid indices
    public CatchingMiceTile GetTile(int x, int y)
    {
        return GetTile(x, y, false);
    }

    // get tile by local position under level root
    public CatchingMiceTile GetTileByLocation(float x, float y)
    {
        int xIndex = Mathf.RoundToInt(x / scale);
        int yIndex = Mathf.RoundToInt(y / scale);

        //instead of returning null, return closest tile?
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
        if (x >= width || x < 0)
        {
            if (clamp)
                x = Mathf.Clamp(x, 0, width - 1);
            else
                return null;
        }
        else if (y >= height || y < 0)
        {
            if (clamp)
                y = Mathf.Clamp(y, 0, height - 1);
            else
                return null;
        }

        return levelTiles[x, y];
    }
    public Waypoint GetWaypointFromTile(Vector2 gridIndices)
    {
        return levelTiles[Mathf.RoundToInt(gridIndices.x), Mathf.RoundToInt(gridIndices.y)].waypoint;
    }
    public Waypoint GetWaypointFromTile(int x, int y)
    {
        return levelTiles[x, y].waypoint;
    }

    public void RemoveTrapFromTile(int x, int y, CatchingMiceTile.TileType tileType = CatchingMiceTile.TileType.Trap)
    {
        CatchingMiceTile tile = levelTiles[x, y];
        foreach (Transform go in _objectParent)
        {
            if(tile.location.v2() == go.position.v2())
            {
                go.gameObject.SetActive(false);

                if ((tile.tileType & CatchingMiceTile.TileType.Cheese) == CatchingMiceTile.TileType.Cheese)
                {
                    CatchingMiceLevelManager.use.cheeseTiles.Remove(tile);
                    //announce this cheese tile has been removed
                    if(CheeseRemoved != null)
                    {
                        CheeseRemoved(tile);
                    }
                }
                else if ((tile.tileType & CatchingMiceTile.TileType.Trap) == CatchingMiceTile.TileType.Trap)
                {
                    //remove from trapTiles
                }


                tile.tileType = tile.tileType ^ tileType;
                tile.trapObject = null;
                
                break;
            }
        }
    }
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
