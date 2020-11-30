using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;

public class ObjectTool : OdinEditorWindow
{
    [MenuItem("Tools/Object Tool")]
    private static void OpenWindow()
    {
        GetWindow<ObjectTool>().Show();
    }

    public Transform m_objectRoot;

    private OdinMenuTree tree;

    [InlineEditor(InlineEditorModes.GUIOnly)]
    public ObjectBrushAsset m_objectBrushAsset;

    private ObjectBrushObjectList[] allObjectLists;

	protected override void OnEnable()
	{
		base.OnEnable();
        SceneView.duringSceneGui += OnSceneGUI;
        FindAllObjectLists();
        GetToolAssetReferences();
    }

	private void OnDisable()
	{
        SceneView.duringSceneGui -= OnSceneGUI;
    }

	private void OnSceneGUI(SceneView screenView)
    {
        m_objectBrushAsset.RunSceneGUI(screenView, m_objectRoot, allObjectLists);
    }

    public void FindAllObjectLists()
    {
        List<ObjectBrushObjectList> foundObjectLists = new List<ObjectBrushObjectList>();

        string[] guids;
        guids = AssetDatabase.FindAssets("t:ObjectBrushObjectList");

        foreach (string guid in guids)
        {
            foundObjectLists.Add((ObjectBrushObjectList)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(ObjectBrushObjectList)));
        }

        allObjectLists = foundObjectLists.ToArray();
    }

    private void GetToolAssetReferences()
    {
        ObjectBrushAsset foundObjectBrushAsset = (ObjectBrushAsset)AssetDatabase.LoadAssetAtPath("Assets/_GreenLeaves/Object Brush/ObjectBrushAsset.asset", typeof(ObjectBrushAsset));

        if (foundObjectBrushAsset != null)
        {
            //m_objectBrushAsset = foundObjectBrushAsset;
        }

        /*
        ObjectEraserAsset foundObjectEraserAsset = (ObjectEraserAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Object Brush/ObjectEraserAsset.asset", typeof(ObjectEraserAsset));

        if (foundObjectEraserAsset != null)
        {
            eraserAsset = foundObjectEraserAsset;
        }

        ObjectRepainterAsset foundRepainterAsset = (ObjectRepainterAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Object Brush/ObjectRepainterAsset.asset", typeof(ObjectRepainterAsset));

        if (foundRepainterAsset != null)
        {
            repainterAsset = foundRepainterAsset;
        }
        */
    }

    /*
        private Transform objectBrushRoot;
        [InlineEditor(InlineEditorModes.GUIAndPreview)]
        public ObjectBrushAsset brushAsset;
        //private ObjectEraserAsset eraserAsset;
        //private ObjectRepainterAsset repainterAsset;

        public Editor scriptableObjectEditor;
        public ScriptableObject scriptableObject;

        public int currentTool = 0;
        public Vector2 scrollPosition = Vector2.zero;

        private ObjectBrushObjectList[] allObjectLists;

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;

            GetToolAssetReferences();
            FindAllObjectLists();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void GetToolAssetReferences()
        {
            ObjectBrushAsset foundObjectBrushAsset = (ObjectBrushAsset)AssetDatabase.LoadAssetAtPath("Assets/_GreenLeaves/Object Brush/ObjectBrushAsset.asset", typeof(ObjectBrushAsset));

            if (foundObjectBrushAsset != null)
            {
                brushAsset = foundObjectBrushAsset;
            }

            ObjectEraserAsset foundObjectEraserAsset = (ObjectEraserAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Object Brush/ObjectEraserAsset.asset", typeof(ObjectEraserAsset));

            if (foundObjectEraserAsset != null)
            {
                eraserAsset = foundObjectEraserAsset;
            }

            ObjectRepainterAsset foundRepainterAsset = (ObjectRepainterAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Object Brush/ObjectRepainterAsset.asset", typeof(ObjectRepainterAsset));

            if (foundRepainterAsset != null)
            {
                repainterAsset = foundRepainterAsset;
            }
        }

        public void FindAllObjectLists()
        {
            List<ObjectBrushObjectList> foundObjectLists = new List<ObjectBrushObjectList>();

            string[] guids;
            guids = AssetDatabase.FindAssets("t:ObjectBrushObjectList");

            foreach (string guid in guids)
            {
                foundObjectLists.Add((ObjectBrushObjectList)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(ObjectBrushObjectList)));
            }

            allObjectLists = foundObjectLists.ToArray();
        }

        private void OnGUI()
        {
            //scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.MaxHeight(1000), GUILayout.ExpandHeight(true));
            //EditorGUIUtility.labelWidth = 300;

            brushAsset = (ObjectBrushAsset)EditorGUILayout.ObjectField("Brush Asset", brushAsset, typeof(ObjectBrushAsset), false);

            if (brushAsset == null)
            {

            }

            if (coinAsset == null)
            {
                coinAsset = (CoinBrushAsset)EditorGUILayout.ObjectField("Coin Asset", coinAsset, typeof(CoinBrushAsset), false);
            }
            if (eraserAsset == null)
            {
                eraserAsset = (ObjectEraserAsset)EditorGUILayout.ObjectField("Eraser Asset", eraserAsset, typeof(ObjectEraserAsset), false);
            }
            if (repainterAsset == null)
            {
                repainterAsset = (ObjectRepainterAsset)EditorGUILayout.ObjectField("Repainter Asset", repainterAsset, typeof(ObjectRepainterAsset), false);
            }
            if (fenceBrushAsset == null)
            {
                fenceBrushAsset = (FenceBrushAsset)EditorGUILayout.ObjectField("Fence Brush Asset", fenceBrushAsset, typeof(FenceBrushAsset), false);
            }

            //currentTool = GUILayout.Toolbar(currentTool, new string[] { "Object Brush", "Coin Brush", "Object Eraser", "Object Repainter", "Fence Brush" });
            currentTool = GUILayout.Toolbar(currentTool, new string[] { "Object Brush" });

            scriptableObject = brushAsset;
            objectBrushRoot = (Transform)EditorGUILayout.ObjectField("Object Brush Root", objectBrushRoot, typeof(Transform), true);

            scriptableObjectEditor = Editor.CreateEditor(scriptableObject);
            scriptableObjectEditor.OnInspectorGUI();

            if (currentTool == 0)
            {

            }

            if (currentTool == 0)
            {
                scriptableObject = brushAsset;
                objectBrushRoot = (Transform)EditorGUILayout.ObjectField("Object Brush Root", objectBrushRoot, typeof(Transform), true);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            else if (currentTool == 1)
            {
                scriptableObject = coinAsset;
                coinBrushRoot = (Transform)EditorGUILayout.ObjectField("Coin Brush Root", coinBrushRoot, typeof(Transform), true);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            else if (currentTool == 2)
            {
                scriptableObject = eraserAsset;
            }
            else if (currentTool == 3)
            {
                scriptableObject = repainterAsset;
            }
            else if (currentTool == 4)
            {
                scriptableObject = fenceBrushAsset;
                fenceBrushRoot = (Transform)EditorGUILayout.ObjectField("Fence Brush Root", fenceBrushRoot, typeof(Transform), true);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            //GUILayout.EndScrollView();
        }

        private void OnSceneGUI(SceneView screenView)
        {
            if (scriptableObject != null)
            {
                if (currentTool == 0)
                {
                    brushAsset.RunSceneGUI(screenView, objectBrushRoot, allObjectLists);
                }
            }

            if (scriptableObject != null)
            {
                if (currentTool == 0)
                {
                    brushAsset.RunSceneGUI(screenView, objectBrushRoot, allObjectLists);
                }
                else if (currentTool == 1)
                {
                    coinAsset.RunSceneGUI(screenView, coinBrushRoot, allObjectLists);
                }
                else if (currentTool == 2)
                {
                    eraserAsset.RunSceneGUI(screenView, objectBrushRoot, allObjectLists);
                }
                else if (currentTool == 3)
                {
                    repainterAsset.RunSceneGUI(screenView, objectBrushRoot, allObjectLists);
                }
                else if (currentTool == 4)
                {
                    fenceBrushAsset.RunSceneGUI(screenView, fenceBrushRoot, allObjectLists);
                }
            }
        }
        */
}