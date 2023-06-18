using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public List<Group> Groups;
    private Dictionary<string, Queue<GameObject>> _poolDictionary;

    // Start is called before the first frame update
    void Start()
    {
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Group g in Groups)
        {
            foreach (Pool pool in g.Pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int i = 0; i < pool.Size; i++)
                {
                    GameObject obj = Instantiate(pool.Prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                _poolDictionary.Add(pool.Tag, objectPool);
            }
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't excist.");
            return null;
        }

        GameObject objectToSpawn = _poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        _poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject Prefab;
        public int Size;
    }
    [System.Serializable]
    public class Group
    {
        public string GroupName;
        public List<Pool> Pools;
    }
}
