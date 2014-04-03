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
                    case "Speed":
                        character.speed = float.Parse(parser.content);
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
        rawdata += tabs + "\t<Speed>" + character.speed.ToString() + "</Speed>\r\n";
        rawdata += tabs + "\t<XLocation>" + character.xLocation.ToString() + "</XLocation>\r\n";
        rawdata += tabs + "\t<YLocation>" + character.yLocation.ToString() + "</YLocation>\r\n";
        //rawdata += tabs + "\t<SpawnDelay>" + character.spawnDelay.ToString() + "</SpawnDelay>\r\n";
        rawdata += tabs + "</Character>\r\n";

        return rawdata;
    }

    public string id = "";
    public float speed = 1;
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

