using UnityEditor;
using UnityEngine;
using System.Collections;

public class CatchingMiceLevelWindow : EditorWindow
{
    protected int levelIndex = 0;

    int width = 13;
    int height = 13;
    string level = "o";
    string previousLevel = "o";
    Transform cursor = null;
    Transform levelParent = null;

    [MenuItem("KikaAndBob/CathingMice/LevelWindow")]
	// Use this for initialization
    static void Init()
    {
        CatchingMiceLevelWindow window = (CatchingMiceLevelWindow)EditorWindow.GetWindow(typeof(CatchingMiceLevelWindow));
        window.Show();
    }
    protected void Update()
    {
        if (levelParent == null)
        {
            levelParent = GameObject.Find("LevelParent").transform;
        }
    }
    void OnGUI()
    {
        if (GUILayout.Button("Create new Catching Mice level (root folder)"))
        {
            CatchingMiceLevelDefinition levelDef = ScriptableObject.CreateInstance<CatchingMiceLevelDefinition>();
            AssetDatabase.CreateAsset(levelDef, "Assets/NewCatchingMiceLevel.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = levelDef;
        }

        width = EditorGUILayout.IntField(width);
        height = EditorGUILayout.IntField(height);
        levelIndex = EditorGUILayout.IntField("Level index", levelIndex);

        if (GUILayout.Button("Build Level " + levelIndex))
        {
            CatchingMiceLevelManager.use.BuildLevel(levelIndex);
        }
        if (GUILayout.Button("Build Catching Mice level"))
        {
            CatchingMiceLevelManager.use.BuildLevelDebug(width,height);

        }
        if (GUILayout.Button("Snap selection to grid"))
        {
            SnapToGrid(); 
        }
        if(GUILayout.Button("Test pathfinding"))
        {
            GameObject pathfindingGO = GameObject.Find("PathFindingObject");
            if(pathfindingGO != null)
            {
                CatchingMicePathFinding pathfindScript = pathfindingGO.GetComponent<CatchingMicePathFinding>();
                if(pathfindScript != null)
                {
                    pathfindScript.SetupLocal();
                    pathfindScript.MoveToRoutine(pathfindScript.navigationGraph[2]);
                }
            }

        }
    }

    //Source: http://wiki.unity3d.com/index.php?title=SnapToGrid
    protected void SnapToGrid()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);

        float gridx = 1.0f;
        float gridy = 1.0f;
        float gridz = 1.0f;

        foreach (Transform transform in transforms)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.Round(newPosition.x / gridx) * gridx;
            newPosition.y = Mathf.Round(newPosition.y / gridy) * gridy;
            newPosition.z = Mathf.Round(newPosition.z / gridz) * gridz;
            transform.position = newPosition;
        }
    }
}
