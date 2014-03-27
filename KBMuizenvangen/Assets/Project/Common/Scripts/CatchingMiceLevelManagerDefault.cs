using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatchingMiceLevelManager : LugusSingletonExisting<CatchingMiceLevelManagerDefault>
{
    
}

public class CatchingMiceLevelManagerDefault : MonoBehaviour {

    protected Transform levelRoot = null;
    protected Transform levelParent = null;
    protected Transform objectParent = null;
    protected Transform navigationParent = null;

    public CatchingMiceLevelDefinition[] levels = null;
    public int width = 13;
    public int height = 13;

    public CatchingMiceTile[,] levelTiles;
    public float scale = 1;

    public GameObject[] tileItems = null;

    [HideInInspector]
    public List<Waypoint> WaypointList = new List<Waypoint>();

    public List<CatchingMiceWorldObject> CheeseTiles = new List<CatchingMiceWorldObject>();
    void Awake()
    {
        FindReferences();
    }
    void FindReferences()
    {
        // only do this once
        if (levelRoot != null)
            return;

        if (levelRoot == null)
            levelRoot = GameObject.Find("LevelRoot").transform;

        if (levelRoot == null)
            Debug.Log("LevelManager: Could not find level root.");

        levelParent = levelRoot.FindChild("LevelParent");
        objectParent = levelRoot.FindChild("ObjectParent");
        navigationParent = levelRoot.FindChild("NavigationParent");
        //characterParent = levelRoot.FindChild("CharacterParent");
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
            for (int i = levelParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(levelParent.GetChild(i).gameObject);
            }
            for (int i = objectParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(objectParent.GetChild(i).gameObject);
            }
            for (int i = navigationParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(navigationParent.GetChild(i).gameObject);
            }
            WaypointList.Clear();
            CheeseTiles.Clear();
            //for (int i = characterParent.childCount - 1; i >= 0; i--)
            //{
            //    DestroyImmediate(characterParent.GetChild(i).gameObject);
            //}
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
        for (int i = levelParent.childCount - 1; i >= 0; i--)
        {
            Destroy(levelParent.GetChild(i).gameObject);
        }
        for (int i = objectParent.childCount - 1; i >= 0; i--)
        {
            Destroy(objectParent.GetChild(i).gameObject);
        }
        for (int i = navigationParent.childCount - 1; i >= 0; i--)
        {
            Destroy(navigationParent.GetChild(i).gameObject);
        }
        WaypointList.Clear();
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

        CreateGrid();

        PlaceWaypoints();
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
                quad.transform.parent = levelParent;

                if (levelTiles[x, y].tileType != CatchingMiceTile.TileType.Ground)
                {
                    Material tempMaterial = new Material(quad.renderer.sharedMaterial);
                    tempMaterial.color = Color.red;
                    //quad.transform.position = levelTile.location.v3().z(-0.5f);
                    quad.transform.localScale = Vector3.one * 1.1f * scale;
                    quad.renderer.sharedMaterial = tempMaterial;
                }
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
                wayPoint.transform.parent = navigationParent.transform;
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

                WaypointList.Add(wp);
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
                if (go.name == definition.id)
                {
                    tileItemPrefab = go;
                    break;
                }
            }

            if (tileItemPrefab == null)
            {
                Debug.LogError("Did not find tile item ID: " + definition.id);
                return;
            }

            CatchingMiceTile targetTile = GetTile(definition.tileCoordinates, false);
            
            if (targetTile == null)
            {
                Debug.LogError("Did not find tile with coordinates:" + definition.tileCoordinates + ". Skipping placing tile item: " + definition.id);
                return;
            }

            GameObject tileItem = (GameObject)Instantiate(tileItemPrefab);
            tileItem.transform.parent = objectParent;
            tileItem.transform.name += " " + targetTile.gridIndices;
            tileItem.transform.localPosition = targetTile.location.z(targetTile.location.z);

            //When placing the furniture, check if the it has the object script, so it can set the right tiles to furniture type
            CatchingMiceWorldObject tileObjectScript = tileItem.GetComponent<CatchingMiceWorldObject>();
            if (tileObjectScript != null)
            {
                tileObjectScript.CalculateColliders();

                //put the needed tiletypes in the right list
                if(tileObjectScript.tileType == CatchingMiceTile.TileType.Cheese)
                {
                    CheeseTiles.Add(tileObjectScript);
                }

            }
            else
            {
                Debug.LogError("Did not find script " + tileObjectScript.ToString() + ". Item will not affect tiles " );
            }
        }
    }

    public void AssignNeighbours()
    {
        for (int i = 0; i < WaypointList.Count-1; i++)
        {
            //Debug.Log("Adding waypoint : " + (i + width) + " to " + i);
            //Debug.Log("Adding waypoint : " + (i+1) + " to " + i);
            //Debug.Log(WaypointList.Count);
             
            //last row does not need to (and can't) add their neighbor below
            if (i < WaypointList.Count - width)
            {
                WaypointList[i].neighbours.Add(WaypointList[i + width]);
                WaypointList[i + width].neighbours.Add(WaypointList[i]);
            }
            //Last column can't add his neighbor next to him
            //all waypoints are in one list, so every width length there will be a new row, is that waypoint is in the last column
            if ((i + 1) % width != 0)
            {
                WaypointList[i].neighbours.Add(WaypointList[i + 1]);
                WaypointList[i + 1].neighbours.Add(WaypointList[i]);
            }
            
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

        if (xIndex >= width || x < 0)
            return null;
        else if (yIndex >= height || y < 0)
            return null;

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
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
