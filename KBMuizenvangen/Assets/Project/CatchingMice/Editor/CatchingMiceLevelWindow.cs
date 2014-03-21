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

    void OnGUI()
    {
        if (GUILayout.Button("Create new Catching Mice level (root folder)"))
        {
            CatchingMiceLevelWindow levelDef = ScriptableObject.CreateInstance<CatchingMiceLevelWindow>();
            AssetDatabase.CreateAsset(levelDef, "Assets/NewCatchingMiceLevel.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = levelDef;
        }

        width = EditorGUILayout.IntField(width);
        height = EditorGUILayout.IntField(height);

        if (GUILayout.Button("build Catching Mice level"))
        {
            CatchingMiceLevelManager.use.BuildLevelDebug(width,height);
        }
       
    }

    protected void Update()
    {
        if (levelParent == null)
        {
            levelParent = GameObject.Find("LevelParent").transform;
        }
    }
}
