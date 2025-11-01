using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;


public class CollectablePool : MonoBehaviour
{
    public int count;
    public GameObject collectableObject;

    private GameObject[] pool;
    void Start()
    {

        InitializePool();

    }
    void InitializePool()
    {
        if (!collectableObject) return;
        pool = new GameObject[count];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(collectableObject, transform);
            pool[i].SetActive(false);
        }


    }

    public GameObject GetCollectable()
    {
        foreach (var collectable in pool)
        {
            if (!collectable.activeSelf)
            {
                collectable.transform.localScale = Vector3.one;
                return collectable;
            }
        }

        return null;
    }
}
