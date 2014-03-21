using UnityEngine;
using System.Collections;

public class CatchingMiceLevelManager : LugusSingletonExisting<CatchingMiceLevelManagerDefault>
{
    
}

public class CatchingMiceLevelManagerDefault : MonoBehaviour {

    protected Transform levelRoot = null;
    protected Transform levelParent = null;

    public CatchingMiceLevelDefinition[] levels = null;
    public int width = 13;
    public int height = 13;

    public Tile[,] levelTiles;
    protected int scale = 1;

    public GameObject[] tileItems = null;

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
        //characterParent = levelRoot.FindChild("CharacterParent");
    }

    public void ClearLevel()
    {
#if UNITY_EDITOR
        Debug.Log("Clearing level (playing in editor).");
        for (int i = levelParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(levelParent.GetChild(i).gameObject);
        }

        //for (int i = characterParent.childCount - 1; i >= 0; i--)
        //{
        //    DestroyImmediate(characterParent.GetChild(i).gameObject);
        //}
#else
		Debug.Log("Clearing level (build).");
		for (int i = levelParent.childCount - 1; i >= 0; i--) 
		{
			Destroy(levelParent.GetChild(i).gameObject);
		}
		
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

    // only used for testing and for quickly building a level
    public void BuildLevelDebug(int _width, int _height)
    {
        FindReferences();

        ClearLevel();

        width = _width;
        height = _height;

        ParseLevelTiles(_width, _height);

        foreach (Tile levelTile in levelTiles)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.position = levelTile.location;
            quad.transform.parent = levelParent;
            
        }

    }
    public void BuildLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Level index was out of bounds!");
            return;
        }

        CatchingMiceLevelDefinition level = levels[levelIndex];

        //FindReferences();

        //ClearLevel();

        width = level.width;
        height = level.height;

        ParseLevelTiles(width, height);

        PlaceLevelTileItems(level.tileItems);

    }
    public void ParseLevelTiles( int _width, int _height)
    {
        // clear grid
        levelTiles = new Tile[_width, _height];

        // iterate over entire grid
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                Tile currentTile = new Tile();
                levelTiles[x, y] = currentTile;

                // register this tile's grid indices and its true location, which is its index * scale
                currentTile.gridIndices = new Vector2(x, y);
                currentTile.location = currentTile.gridIndices * scale;


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
        
        
        
        }
    }


    // Lookup methods--------------------------------------------------------------------

    // get tile by grid indices (contained in vector2)
    public Tile GetTile(Vector2 coords)
    {
        return GetTile(coords, false);
    }

    // get tile by grid indices (contained in vector2)
    public Tile GetTile(Vector2 coords, bool clamp)
    {
        int x = Mathf.RoundToInt(coords.x);
        int y = Mathf.RoundToInt(coords.y);

        return GetTile(x, y, clamp);
    }

    // get tile by grid indices
    public Tile GetTile(int x, int y)
    {
        return GetTile(x, y, false);
    }

    // get tile by local position under level root
    public Tile GetTileByLocation(float x, float y)
    {
        int xIndex = Mathf.RoundToInt(x / scale);
        int yIndex = Mathf.RoundToInt(y / scale);

        if (xIndex >= width || x < 0)
            return null;
        else if (yIndex >= height || y < 0)
            return null;

        return GetTile(xIndex, yIndex, true);
    }

    public Tile GetTile(int x, int y, bool clamp)
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
