#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dev.Scripts.Level;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor.Experimental.SceneManagement;

namespace Dacodelaac.LevelSystem
{
    public class LevelEditor : EditorWindow
    {
        string AssetPath => "Assets/Dev/Prefabs/Level";
        string FolderPath => Path.Combine(Application.dataPath, "Dev/Prefabs").Replace('\\','/');

        static GameObject _selectedPrefab;
        static Vector2 _scrollPosition;
        static int _levelIndex;

        [UnityEditor.MenuItem("Window/LevelEditor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(LevelEditor));
        }

        void OnEnable()
        {
            SceneView.duringSceneGui += DuringSceneGui;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
        }

        void OnGUI()
        {
            if (Application.isPlaying) return;

            EditorGUILayout.Space();
            var stageHandle = StageUtility.GetCurrentStageHandle();
            var level = stageHandle.FindComponentOfType<LevelMap>();
            if (level && PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                var levelAsset =
                    AssetDatabase.LoadAssetAtPath<LevelMap>(PrefabStageUtility.GetCurrentPrefabStage().prefabAssetPath);
                if (levelAsset)
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("Level", levelAsset, typeof(LevelMap), true);
                    GUI.enabled = true;

                    EditorGUILayout.BeginVertical("HelpBox");
                    GUILayout.Label("Select prefab => Ctrl + Right click: Place object");
                    // GUILayout.Label("Ctrl + Q: Align X, Ctrl + W: Align Y, Ctrl + E: Align Z, Ctrl + R: Snap down");
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.Space();

                    DrawGameSpecific(level);

                    EditorGUILayout.Space();

                    DrawPrefabSections();

                    EditorGUILayout.Space();

                    return;
                }
            }

            EditorGUILayout.BeginHorizontal("HelpBox");
            GUILayout.Label("Open a level prefab first!");
            EditorGUILayout.EndHorizontal();
        }

        void DrawPrefabSections()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            if (!string.IsNullOrEmpty(FolderPath))
            {
                var directoryInfo = new DirectoryInfo(FolderPath);
                if (directoryInfo.Exists)
                {
                    var directories = directoryInfo.GetDirectories();
                    Debug.Log($"directories count: {directories.Length}");
                    foreach (var directory in directories)
                    {
                        DrawPrefabSelector(directory);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        void DrawPrefabSelector(DirectoryInfo directory)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Label(directory.Name, EditorStyles.boldLabel);
            var fileInfos = directory.GetFiles();
            var prefabs = new List<GameObject>();
            foreach (var fileInfo in fileInfos)
            {
                var s = fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal);
                var path = fileInfo.FullName.Substring(s);
                var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (prefab != null)
                {
                    prefabs.Add(prefab);
                }
            }

            const float buttonSize = 100;
            const float buttonSpace = 4;
            var column = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth - 10) / (buttonSize + buttonSpace));
            var row = Mathf.CeilToInt(prefabs.Count * 1.0f / column);
            for (var i = 0; i < row; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var j = 0; j < column; j++)
                {
                    var index = i * column + j;
                    if (index >= prefabs.Count)
                    {
                        break;
                    }

                    EditorGUILayout.BeginVertical();
                    GUI.color = _selectedPrefab == prefabs[index] ? Color.cyan : Color.white;
                    if (GUILayout.Button(AssetPreview.GetAssetPreview(prefabs[index].gameObject), GUILayout.Width(100),
                        GUILayout.Height(100)))
                    {
                        if (_selectedPrefab != prefabs[index])
                        {
                            _selectedPrefab = prefabs[index];
                        }
                        else
                        {
                            _selectedPrefab = null;
                        }
                    }

                    GUILayout.Label(prefabs[index].name, GUILayout.MaxWidth(100));
                    EditorGUILayout.EndVertical();
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        void DrawGameSpecific(LevelMap level)
        {
            
        }
        
        GameObject FindPrefab(string path)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(AssetPath, $"{path}.prefab"));
        }

        GameObject SpawnPrefab(string path)
        {
            var prefab = FindPrefab(path);
            var newGo = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            StageUtility.PlaceGameObjectInCurrentStage(newGo);
            return newGo;
        }

        void DuringSceneGui(SceneView sceneView)
        {
            sceneView.Repaint();
            CustomOnSceneGUI(sceneView);
            EditorUtility.SetDirty(sceneView);
        }

        public Camera cam;

        bool MouseInView
        {
            get
            {
                try
                {
                    return mouseOverWindow != null && mouseOverWindow.titleContent != null &&
                           string.Equals("Scene", mouseOverWindow.titleContent.text);
                }
                catch
                {
                    return false;
                }
            }
        }

        Vector2 EventMousePoint
        {
            get
            {
                var v = Event.current.mousePosition;
                v.y = Screen.height - v.y - 60f;
                return v;
            }
        }

        void CustomOnSceneGUI(SceneView sceneView)
        {
            if (Application.isPlaying)
            {
                return;
            }

            var stageHandle = StageUtility.GetCurrentStageHandle();
            var level = stageHandle.FindComponentOfType<LevelMap>();
            if (!level) return;

            PlaceObject(level);
            TransformHelper(level);
        }

        void PlaceObject(LevelMap level)
        {
            cam = SceneView.currentDrawingSceneView.camera;
            if (!MouseInView) return;
            if (_selectedPrefab == null) return;

            var mouseCast = RaycastPoint(level, EventMousePoint);
            var ec = Event.current;
            var hsize = HandleUtility.GetHandleSize(mouseCast) * 0.4f;
            Handles.color = new Color(1, 0, 0, 0.5f);
            Handles.DrawSolidDisc(mouseCast, Vector3.up, hsize * 0.5f);

            if (ec.type == EventType.MouseDown && ec.button == 1 && ec.control)
            {
                var newGo = PrefabUtility.InstantiatePrefab(_selectedPrefab) as GameObject;
                StageUtility.PlaceGameObjectInCurrentStage(newGo);

                newGo.transform.parent = level.transform;
                newGo.transform.position = new Vector3(Mathf.RoundToInt(mouseCast.x), mouseCast.y,
                    Mathf.RoundToInt(mouseCast.z));
                newGo.transform.localPosition = new Vector3(newGo.transform.localPosition.x,
                    _selectedPrefab.transform.localPosition.y, newGo.transform.localPosition.z);
                newGo.transform.localRotation = _selectedPrefab.transform.localRotation;
                Selection.activeObject = newGo;
                EditorUtility.SetDirty(level.gameObject);

                Undo.RegisterCreatedObjectUndo(newGo, "place object");
            }
        }

        void TransformHelper(LevelMap level)
        {
            var ec = Event.current;
            if (ec.type == EventType.KeyDown && ec.control)
            {
                var snapDown = ec.keyCode == KeyCode.R;
                var alignX = ec.keyCode == KeyCode.Q;
                var alignY = ec.keyCode == KeyCode.W;
                var alignZ = ec.keyCode == KeyCode.E;

                if (snapDown || alignX || alignY || alignZ)
                {
                    var tfs = Selection.objects.OfType<GameObject>().Select(go => go.transform).ToArray();
                    if (tfs.Length > 0)
                    {
                        Undo.RecordObjects(tfs, "transform helper");
                        foreach (var tf in tfs)
                        {
                            if (snapDown)
                            {
                                if (RayCast(level, new Ray(tf.position + Vector3.up * 0.1f, Vector3.down), out var v))
                                {
                                    tf.position = v;
                                    EditorUtility.SetDirty(tf.transform);
                                }
                            }

                            if (alignX)
                            {
                                var pos = tf.position;
                                pos.x = tfs[0].position.x;
                                tf.position = pos;
                            }

                            if (alignY)
                            {
                                var pos = tf.position;
                                pos.y = tfs[0].position.y;
                                tf.position = pos;
                            }

                            if (alignZ)
                            {
                                var pos = tf.position;
                                pos.z = tfs[0].position.z;
                                tf.position = pos;
                            }
                        }
                    }
                }
            }
        }

        Vector3 RaycastPoint(LevelMap level, Vector2 screenPoint, float dist = 10)
        {
            var r = cam.ScreenPointToRay(screenPoint);
            if (!RayCast(level, r, out var v))
            {
                v = r.origin + r.direction.normalized * dist;
            }

            return v;
        }

        bool RayCast(LevelMap level, Ray r, out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;

            RaycastHit hit;
            if (level.gameObject.scene.GetPhysicsScene().Raycast(r.origin, r.direction, out hit))
            {
                hitPoint = hit.point;
                return true;
            }

            return false;
        }
    }
}
#endif