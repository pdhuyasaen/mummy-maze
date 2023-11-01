using System;
using System.Collections.Generic;
using System.Linq;
using Dacodelaac.Core;
using Dacodelaac.DebugUtils;
using Dacodelaac.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dacodelaac.ObjectPooling
{
    [CreateAssetMenu(menuName = "ObjectPooling/Pools")]
    public class Pools : BaseSO, ISerializationCallbackReceiver
    {
        [SerializeField] private PoolData[] poolDatas;

        private Dictionary<GameObject, Queue<GameObject>> waitPool;
        private LinkedList<GameObject> activePool;
        private Transform container;
        private bool initialized;

        public override void Initialize()
        {
            if (initialized) return;
            initialized = true;
            
            waitPool = new Dictionary<GameObject, Queue<GameObject>>();
            activePool = new LinkedList<GameObject>();
            container = new GameObject("Pool").transform;
            DontDestroyOnLoad(container.gameObject);

            PreSpawn();
        }

        private void PreSpawn()
        {
            foreach (var data in poolDatas)
            {
                for (var i = 0; i < data.preSpawn; i++)
                {
                    SpawnNew(data.prefab);
                }
            }
        }

        private void SpawnNew(GameObject prefab)
        {
            var gameObject = Instantiate(prefab);
            var id = gameObject.AddComponent<PooledObjectId>();
            id.prefab = prefab;
            
            activePool.AddLast(gameObject);
            
            DeSpawn(gameObject, false);
        }

        public void DeSpawn(GameObject gameObject, bool destroy = false)
        {
            var id = gameObject.GetComponent<PooledObjectId>();
            if (id == null)
            {
                Dacoder.LogError($"{gameObject.name} is not a pooled object!");
                return;
            }
            if (!activePool.Contains(gameObject))
            {
                Dacoder.LogError($"{gameObject.name} is not in active pool!");
                return;
            }
            activePool.Remove(gameObject);
            if (!waitPool.ContainsKey(id.prefab))
            {
                waitPool.Add(id.prefab, new Queue<GameObject>());
            }
            var stack = waitPool[id.prefab];
            if (stack.Contains(gameObject))
            {
                Dacoder.LogError($"{gameObject.name} is already pooled!");
                return;
            }
            CleanUp(gameObject);
            if (destroy)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
                gameObject.transform.parent = container;
                stack.Enqueue(gameObject);
            }
        }
        
        public void DespawnAll()
        {
            var arr = activePool.ToArray();
            foreach (var o in arr)
            {
                if (o != null) DeSpawn(o);
            }
        }

        public void DestroyAll()
        {
            var arr = waitPool.Values.SelectMany(g => g).ToArray();
            for (var i = 0; i < arr.Length; i++)
            {
                Destroy(arr[i].gameObject);
            }
            waitPool.Clear();
        }

        public T Spawn<T>(T type, Transform parent = null, bool initialize = true) where T : Component
        {
            return Spawn(type.gameObject, parent, initialize).GetComponent<T>();
        }
        
        public GameObject Spawn(GameObject prefab, Transform parent = null, bool initialize = true)
        {
            if (!waitPool.ContainsKey(prefab))
            {
                waitPool.Add(prefab, new Queue<GameObject>());
            }

            var stack = waitPool[prefab];
            if (stack.Count == 0)
            {
                SpawnNew(prefab);
            }
            var gameObject = stack.Dequeue();
            
            gameObject.transform.parent = parent;

            if (parent == null)
            {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }

            gameObject.SetActive(true);
            
            if (initialize)
            {
                Initialize(gameObject);
            }

            activePool.AddLast(gameObject);
            
            return gameObject;
        }

        private static void Initialize(GameObject go)
        {
            var monos = go.GetComponentsInChildren<BaseMono>(true);
            foreach (var mono in monos)
            {
                mono.Initialize();
            }
        }

        private static void CleanUp(GameObject go)
        {
            var monos = go.GetComponentsInChildren<BaseMono>(true);
            foreach (var mono in monos)
            {
                mono.CleanUp();
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            initialized = false;
        }

#if UNITY_EDITOR
        [ContextMenu("Auto Bind")]
        public void AutoBind()
        {
            var soes = AssetUtils.FindAssetAtFolder<BaseSO>(new string[] {"Assets"});
            foreach (var so in soes)
            {
                so.pools = this;
                EditorUtility.SetDirty(so);
            }

            var goes = AssetUtils.FindAssetAtFolder<GameObject>(new string[] {"Assets"});
            foreach (var go in goes)
            {
                var monoes = go.GetComponentsInChildren<BaseMono>(true);
                foreach (var mono in monoes)
                {
                    mono.pools = this;
                    EditorUtility.SetDirty(mono);
                }
            }
        }
#endif
    }

    [Serializable]
    public class PoolData
    {
        public GameObject prefab;
        public int preSpawn;
    }
}
