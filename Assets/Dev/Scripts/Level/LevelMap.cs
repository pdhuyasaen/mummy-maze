using Dacodelaac.Core;
using Dacodelaac.DataStorage;
using Dacodelaac.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace Dev.Scripts.Level
{
    public class LevelMap : BaseMono
    {
        public string levelKey = "current_level";
    
        private void Start()
        {
       
        }

        private void OnDrawGizmos()
        {
            if (Camera.main != null)
            {
                var main = Camera.main;
                var verticalHeightSeen = main.orthographicSize * 2.0f;
                var verticalWidthSeen = verticalHeightSeen * main.aspect;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, new Vector3(verticalWidthSeen, verticalHeightSeen, 0));
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelMap))]
    public class LevelMapEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LevelMap level = (LevelMap)target;
            
            base.OnInspectorGUI();
        
            if (!Application.isPlaying && GUILayout.Button("PLAY"))
            {
                var levelName = target.name;
            
                var str = levelName.Split("_");
                var levelIndex = int.Parse(str[^1]);
            
                var persistentDataPath = DataStorage.GetPersistentDataPath();
                GameData.Set(level.levelKey, levelIndex);
                GameData.Save();
                SceneMenu.OpenGameScene();
                EditorApplication.isPlaying = true;
            }
        }
    }

#endif
}