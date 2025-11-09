
using UnityEngine; 


public class CollectablePool : MonoBehaviour
{
    [Header("Component Pool Settings")]
    public int m_ComponentCount;
    public GameObject m_ComponentPrefab;
    private GameObject[] m_ComponentPool;

    [Header("Soul Pool Settings")]
    
    public int m_SoulCount;
    public GameObject m_SoulPrefab;
    private GameObject[] m_SoulPool;
    void Start()
    {

        InitializeComponentPool();
        InitializeSoulPool();

    }
    void InitializeComponentPool()
    {
        if (!m_ComponentPrefab) return;
        m_ComponentPool = new GameObject[m_ComponentCount];
        for (int i = 0; i < m_ComponentPool.Length; i++)
        {
            m_ComponentPool[i] = Instantiate(m_ComponentPrefab, transform);
            m_ComponentPool[i].SetActive(false);
        }


    }
    void InitializeSoulPool()
    {
        if (!m_SoulPrefab) return;
        m_SoulPool = new GameObject[m_SoulCount];
        for (int i = 0; i < m_SoulPool.Length; i++)
        {
            m_SoulPool[i] = Instantiate(m_SoulPrefab, transform);
            m_SoulPool[i].SetActive(false);
        }
    }

    public GameObject GetComponentCollectable()
    {
        foreach (var collectable in m_ComponentPool)
        {
            if (!collectable.activeSelf)
            {
                collectable.transform.localScale = Vector3.one;
                return collectable;
            }
        }

        return null;
    }
    public GameObject GetSoulCollectable()
    {
        foreach (var collectable in m_SoulPool)
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
