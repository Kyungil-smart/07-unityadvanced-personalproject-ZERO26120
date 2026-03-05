using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    [System.Serializable]
    public class PoolInfo
    {
        public string poolName;
        public GameObject prefab;
        public int defaultCapacity = 50;
        public int maxSize = 500;
    }

    [Header("# 오브젝트 풀 설정")]
    [SerializeField] private List<PoolInfo> poolInfos;

    private Dictionary<string, IObjectPool<GameObject>> poolDictionary;
    private Dictionary<string, Transform> poolParents;

    void Awake()
    {
        if (instance == null) 
        { 
            instance = this; 
            InitPools(); 
        }
        else 
        { 
            Destroy(gameObject); 
        }
    }

    private void InitPools()
    {
        poolDictionary = new Dictionary<string, IObjectPool<GameObject>>();
        poolParents = new Dictionary<string, Transform>();

        foreach (PoolInfo info in poolInfos)
        {
            GameObject parentObj = new GameObject($"{info.poolName}_Pool");
            parentObj.transform.SetParent(transform);
            poolParents.Add(info.poolName, parentObj.transform);

            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(info.prefab, poolParents[info.poolName]),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: info.defaultCapacity,
                maxSize: info.maxSize
            );

            poolDictionary.Add(info.poolName, pool);
        }
    }

    public GameObject Get(string poolName)
    {
        if (poolDictionary.ContainsKey(poolName))
        {
            return poolDictionary[poolName].Get();
        }
        return null;
    }

    public void Release(string poolName, GameObject obj)
    {
        if (poolDictionary.ContainsKey(poolName))
        {
            poolDictionary[poolName].Release(obj);
        }
    }
}