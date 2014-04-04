using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CatchingMiceLevelDefinition : ScriptableObject {

	
	public static CatchingMiceLevelDefinition FromXML(string rawData)
	{
		TinyXmlReader parser = new TinyXmlReader(rawData);

		while (parser.Read())
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
				(parser.tagName == "Level"))
			{
				return CatchingMiceLevelDefinition.FromXML(parser);
			}
		}

		// If we end up here, then no level tag was found...
		return null;
	}

	public static CatchingMiceLevelDefinition FromXML(TinyXmlReader parser)
	{
		CatchingMiceLevelDefinition level = ScriptableObject.CreateInstance<CatchingMiceLevelDefinition>();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Level"))
		{
			Debug.Log("PacmanLevelDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

        //List<string> updaters = new List<string>();
        List<CatchingMiceCharacterDefinition> characters = new List<CatchingMiceCharacterDefinition>();
		List<CatchingMiceTileItemDefinition> tileitems = new List<CatchingMiceTileItemDefinition>();
        List<CatchingMiceEnemyDefinition> enemies = new List<CatchingMiceEnemyDefinition>();
		while (parser.Read("Level"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
                    //case "BackgroundMusicName":
                    //    level.backgroundMusicName = parser.content;
                    //    break;
					case "Width":
						level.width = int.Parse(parser.content);
						break;
					case "Height":
						level.height = int.Parse(parser.content);
						break;
                    //case "CameraTracksPlayer":
                    //    level.cameraTracksPlayer = bool.Parse(parser.content);
                    //    break;
					case "Layout":
						char[] separators = { ' ', '\t', '\n', '\r' };
						string[] rows = parser.content.Split(separators);
						foreach (string row in rows)
						{
							level.level += row;
						}
						break;
                    case "Character":
                        characters.Add(CatchingMiceCharacterDefinition.FromXML(parser));
                        break;
					case "TileItem":
						tileitems.Add(CatchingMiceTileItemDefinition.FromXML(parser));
						break;
                    case "Enemy":
                        enemies.Add(CatchingMiceEnemyDefinition.FromXML(parser));
                        break;
				}
			}
		}

		level.tileItems = tileitems.ToArray();

		return level;
	}

	public static string ToXML(CatchingMiceLevelDefinition level)
	{
		string rawdata = string.Empty;

		if (level == null)
		{
			Debug.Log("PacmanLevelDefinition.ToXML(): The level to be serialized is null.");
			return rawdata;
		}

		rawdata += "<Level>\r\n";
		//rawdata += "\t<BackgroundMusicName>" + level.backgroundMusicName + "</BackgroundMusicName>\r\n";
		rawdata += "\t<Width>" + level.width.ToString() + "</Width>\r\n";
		rawdata += "\t<Height>" + level.height.ToString() + "</Height>\r\n";
		//rawdata += "\t<CameraTracksPlayer>" + level.cameraTracksPlayer.ToString() + "</CameraTracksPlayer>\r\n";
		rawdata += "\t<Layout>\r\n";
		for (int i = 0; i < level.height; ++i)
		{
			rawdata += "\t\t" + level.level.Substring(i * level.width, level.width) + "\r\n";
		}
		rawdata += "\t</Layout>\r\n";
        rawdata += "\t<Characters>\r\n";
        foreach (CatchingMiceCharacterDefinition character in level.characters)
        {
            rawdata += CatchingMiceCharacterDefinition.ToXML(character, 2);
        }
        rawdata += "\t</Characters>\r\n";
		rawdata += "\t<TileItems>\r\n";
		foreach(CatchingMiceTileItemDefinition tileitem in level.tileItems)
		{
			rawdata += CatchingMiceTileItemDefinition.ToXML(tileitem, 2);
		}
		rawdata += "\t</TileItems>\r\n";
        rawdata += "\t<Enemies>\r\n";
        foreach (CatchingMiceEnemyDefinition enemy in level.enemies)
        {
            rawdata += CatchingMiceEnemyDefinition.ToXML(enemy, 2);
        }
        rawdata += "\t</Enemies>\r\n";
		rawdata += "</Level>\r\n";

		return rawdata;
	}

	//public string backgroundMusicName = "";
	public int width = 13;
	public int height = 13;
	//public bool cameraTracksPlayer = false;
	public string level;
    public CatchingMiceCharacterDefinition[] characters;
	public CatchingMiceTileItemDefinition[] tileItems;
    public CatchingMiceEnemyDefinition[] enemies;

	// Arrays of serialized classes are not created with default values
	// Instead, initialize values once in OnEnable (which runs AFTER deserialization), checking for null / zero value
	// http://forum.unity3d.com/threads/155352-Serialization-Best-Practices-Megapost
	void OnEnable()
	{
	
	}
}
[System.Serializable]
public class CatchingMiceCharacterDefinition
{

    public static CatchingMiceCharacterDefinition FromXML(TinyXmlReader parser)
    {
        CatchingMiceCharacterDefinition character = new CatchingMiceCharacterDefinition();

        if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
            (parser.tagName != "Character"))
        {
            Debug.Log("CatchingMiceDefinition.FromXML(): unexpected tag type or tag name.");
            return null;
        }
        while (parser.Read("Character"))
        {
            if (parser.tagType == TinyXmlReader.TagType.OPENING)
            {
                switch (parser.tagName)
                {
                    case "ID":
                        character.id = parser.content;
                        break;
                    case "TimeToReachTile":
                        character.timeToReachTile = float.Parse(parser.content);
                        break;
                    case "XLocation":
                        character.xLocation = int.Parse(parser.content);
                        break;
                    case "YLocation":
                        character.yLocation = int.Parse(parser.content);
                        break;
                    
                }
            }
        }
        return character;
    }

    public static string ToXML(CatchingMiceCharacterDefinition character, int depth)
    {
        string rawdata = string.Empty;

        if (character == null)
        {
            Debug.Log("PacmanCharacterDefinition.ToXML(): The character to be serialized is null.");
            return rawdata;
        }

        string tabs = string.Empty;
        for (int i = 0; i < depth; ++i)
        {
            tabs += "\t";
        }

        rawdata += tabs + "<Character>\r\n";
        rawdata += tabs + "\t<ID>" + character.id + "</ID>\r\n";
        rawdata += tabs + "\t<Speed>" + character.timeToReachTile.ToString() + "</Speed>\r\n";
        rawdata += tabs + "\t<XLocation>" + character.xLocation.ToString() + "</XLocation>\r\n";
        rawdata += tabs + "\t<YLocation>" + character.yLocation.ToString() + "</YLocation>\r\n";
        //rawdata += tabs + "\t<SpawnDelay>" + character.spawnDelay.ToString() + "</SpawnDelay>\r\n";
        rawdata += tabs + "</Character>\r\n";

        return rawdata;
    }

    public string id = "";
    public float timeToReachTile = 0.5f;
    public int xLocation = 0;
    public int yLocation = 0;
}
[System.Serializable]
public class CatchingMiceTileItemDefinition
{
    public static CatchingMiceTileItemDefinition FromXML(TinyXmlReader parser)
    {
        CatchingMiceTileItemDefinition tileitem = new CatchingMiceTileItemDefinition();

        if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
            (parser.tagName != "TileItem"))
        {
            Debug.Log("PacmanTileItemDefinition.FromXML(): unexpected tag type or tag name.");
            return null;
        }

        while (parser.Read("TileItem"))
        {
            if (parser.tagType == TinyXmlReader.TagType.OPENING)
            {
                switch (parser.tagName)
                {
                    case "ID":
                        tileitem.id = parser.content;
                        break;
                    case "TileCoordinates":
                        Vector2 coordinates = Vector2.zero;
                        while (parser.Read("TileCoordinates"))
                        {
                            if (parser.tagType == TinyXmlReader.TagType.OPENING)
                            {
                                switch (parser.tagName)
                                {
                                    case "X":
                                        coordinates.x = float.Parse(parser.content);
                                        break;
                                    case "Y":
                                        coordinates.y = float.Parse(parser.content);
                                        break;
                                }
                            }
                        }
                        tileitem.tileCoordinates = coordinates;
                        break;
                }
            }
        }

        return tileitem;
    }

    public static string ToXML(CatchingMiceTileItemDefinition tileitem, int depth)
    {
        string rawdata = string.Empty;

        if (tileitem == null)
        {
            Debug.Log("PacmanTileItemDefinition.ToXML(): The tile item to be serialized is null.");
            return rawdata;
        }

        string tabs = string.Empty;
        for (int i = 0; i < depth; ++i)
        {
            tabs += "\t";
        }

        rawdata += tabs + "<TileItem>\r\n";
        rawdata += tabs + "\t<ID>" + tileitem.id + "</ID>\r\n";
        rawdata += tabs + "\t<TileCoordinates>\r\n";
        rawdata += tabs + "\t\t<X>" + tileitem.tileCoordinates.x.ToString() + "</X>\r\n";
        rawdata += tabs + "\t\t<Y>" + tileitem.tileCoordinates.y.ToString() + "</Y>\r\n";
        rawdata += tabs + "\t</TileCoordinates>\r\n";
        rawdata += tabs + "</TileItem>\r\n";

        return rawdata;
    }


    public string id;
    public Vector2 tileCoordinates;
}
[System.Serializable]
public class CatchingMiceEnemyDefinition
{

    public static CatchingMiceEnemyDefinition FromXML(TinyXmlReader parser)
    {
        CatchingMiceEnemyDefinition enemy = new CatchingMiceEnemyDefinition();

        if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
            (parser.tagName != "Enemy"))
        {
            Debug.Log("CatchingMiceDefinition.FromXML(): unexpected tag type or tag name.");
            return null;
        }
        while (parser.Read("Enemy"))
        {
            if (parser.tagType == TinyXmlReader.TagType.OPENING)
            {
                switch (parser.tagName)
                {
                    case "ID":
                        enemy.id = parser.content;
                        break;
                    case "TileCoordinates":
                        Vector2 coordinates = Vector2.zero;
                        while (parser.Read("TileCoordinates"))
                        {
                            if (parser.tagType == TinyXmlReader.TagType.OPENING)
                            {
                                switch (parser.tagName)
                                {
                                    case "X":
                                        coordinates.x = float.Parse(parser.content);
                                        break;
                                    case "Y":
                                        coordinates.y = float.Parse(parser.content);
                                        break;
                                }
                            }
                        }
                        enemy.tileCoordinates = coordinates;
                        break;
                    case "StartDirection":
                        switch (parser.content)
                        {
                            case "down":
                                enemy.startDirection = ICatchingMiceCharacter.CharacterDirections.Down;
                                break;
                            case "left":
                                enemy.startDirection = ICatchingMiceCharacter.CharacterDirections.Left;
                                break;
                            case "up":
                                enemy.startDirection = ICatchingMiceCharacter.CharacterDirections.Up;
                                break;
                            case "right":
                                enemy.startDirection = ICatchingMiceCharacter.CharacterDirections.Right;
                                break;
                            case "undefined":
                                enemy.startDirection = ICatchingMiceCharacter.CharacterDirections.Undefined;
                                break;
                        }
                        break;
                    case "StartWave":
                        enemy.startWave = int.Parse(parser.content);
                        break;
                    case "StartCount":
                        enemy.startCount = int.Parse(parser.content);
                        break;
                    case "AddedPerWave":
                        enemy.addedPerWave = int.Parse(parser.content);
                        break;
                    case "SpawnTimeInterval":
                        enemy.spawnTimeInterval = float.Parse(parser.content);
                        break;
                    case "SpawnTimeOffset":
                        enemy.spawnTimeOffset = float.Parse(parser.content);
                        break;
                }
            }
        }
        return enemy;
    }

    public static string ToXML(CatchingMiceEnemyDefinition character, int depth)
    {
        string rawdata = string.Empty;

        if (character == null)
        {
            Debug.Log("PacmanCharacterDefinition.ToXML(): The character to be serialized is null.");
            return rawdata;
        }

        string tabs = string.Empty;
        for (int i = 0; i < depth; ++i)
        {
            tabs += "\t";
        }

        rawdata += tabs + "<Enemy>\r\n";
        rawdata += tabs + "\t<ID>" + character.id + "</ID>\r\n";
        rawdata += tabs + "\t<TileCoordinates>\r\n";
        rawdata += tabs + "\t\t<X>" + character.tileCoordinates.x.ToString() + "</X>\r\n";
        rawdata += tabs + "\t\t<Y>" + character.tileCoordinates.y.ToString() + "</Y>\r\n";
        rawdata += tabs + "\t</TileCoordinates>\r\n";
        rawdata += tabs + "\t<StartDirection>";
        switch (character.startDirection)
        {
            case ICatchingMiceCharacter.CharacterDirections.Down:
                rawdata += "down";
                break;
            case ICatchingMiceCharacter.CharacterDirections.Left:
                rawdata += "left";
                break;
            case ICatchingMiceCharacter.CharacterDirections.Up:
                rawdata += "up";
                break;
            case ICatchingMiceCharacter.CharacterDirections.Right:
                rawdata += "right";
                break;
            case ICatchingMiceCharacter.CharacterDirections.Undefined:
            default:
                rawdata += "undefined";
                break;
        }
        rawdata += "</StartDirection>\r\n";
        rawdata += tabs + "\t<StartWave>" + character.startWave + "</StartWave>\r\n";
        rawdata += tabs + "\t<StartCount>" + character.startCount + "</StartCount>\r\n";
        rawdata += tabs + "\t<AddedPerWave>" + character.addedPerWave + "</AddedPerWave>\r\n";
        rawdata += tabs + "\t<SpawnTimeInterval>" + character.spawnTimeInterval + "</SpawnTimeInterval>\r\n";
        rawdata += tabs + "\t<SpawnTimeOffset>" + character.spawnTimeOffset + "</SpawnTimeOffset>\r\n";
        rawdata += tabs + "</Enemy>\r\n";

        return rawdata;
    }

    public string id = "";
    public Vector2 tileCoordinates = Vector2.zero;
    public ICatchingMiceCharacter.CharacterDirections startDirection = ICatchingMiceCharacter.CharacterDirections.Undefined;
    public int startWave = 0;
    public int startCount = 1;
    public int addedPerWave = 0;
    public float spawnTimeInterval = 0.5f;
    public float spawnTimeOffset = 0.0f;
}

