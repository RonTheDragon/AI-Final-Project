using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, bool despawnExisting = false)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't excist.");
            return null;
        }

        GameObject objectToSpawn = _poolDictionary[tag].Dequeue();

        if (!despawnExisting)
        {
            for (int i = 0; i < _poolDictionary[tag].Count; i++)
            {
                if (objectToSpawn.activeSelf)
                {
                    _poolDictionary[tag].Enqueue(objectToSpawn);
                    objectToSpawn = _poolDictionary[tag].Dequeue();
                }
                else
                {
                    break;
                }
                if (i == _poolDictionary[tag].Count)
                {
                    Debug.LogError($"Not Enough {tag} in pool");
                    return null;
                }
            }
        }

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
