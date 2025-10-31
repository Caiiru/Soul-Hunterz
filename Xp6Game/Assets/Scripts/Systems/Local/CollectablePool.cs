using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;


public class CollectablePool : MonoBehaviour
{
    public int count;
    public GameObject collectableObject;

    private GameObject[] pool;
    async void Start()
    {

        await InitializePool();

    }
    async Task<UniTask> InitializePool()
    {
        if (!collectableObject) return UniTask.CompletedTask;
        pool = new GameObject[count];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(collectableObject, transform);
            pool[i].SetActive(false);
        }

        return UniTask.CompletedTask;
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
