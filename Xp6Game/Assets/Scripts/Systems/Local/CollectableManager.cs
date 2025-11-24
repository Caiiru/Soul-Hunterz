
using UnityEngine;

[RequireComponent(typeof(CollectablePool))]
public class CollectableManager : MonoBehaviour
{
    CollectablePool _collectablePool;


    Transform m_PlayerTransform;

    protected static float k_DropForce = 300f;

    //Events
    EventBinding<OnDropComponent> m_OnPlayerDropComponent;
    EventBinding<SpawnSoulEvent> m_OnEnemyDiedEvent;
    EventBinding<OnGameStart> m_OnGameStart;


    public bool m_isDebug = false;
    void Start()
    {
        if (m_isDebug)
        {
            BindObjects();
            BindEvents();
        }

        BindEvents();

    }

    public void BindObjects()
    {
        _collectablePool = GetComponent<CollectablePool>();
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void BindEvents()
    {
        m_OnPlayerDropComponent = new EventBinding<OnDropComponent>(HandleComponentDrop);
        EventBus<OnDropComponent>.Register(m_OnPlayerDropComponent);

        m_OnGameStart = new EventBinding<OnGameStart>(HandleGameStart);
        EventBus<OnGameStart>.Register(m_OnGameStart);

        m_OnEnemyDiedEvent = new EventBinding<SpawnSoulEvent>(HandleEnemyDied);
        EventBus<SpawnSoulEvent>.Register(m_OnEnemyDiedEvent);




    }


    private void UnbindEvents()
    {
        EventBus<OnDropComponent>.Unregister(m_OnPlayerDropComponent);
        EventBus<OnGameStart>.Unregister(m_OnGameStart);
        EventBus<SpawnSoulEvent>.Unregister(m_OnEnemyDiedEvent);
    }

    private void HandleGameStart()
    {
        BindObjects();

    }

    private void HandleEnemyDied(SpawnSoulEvent eventData)
    {
        // Spawn Souls
        // Debug.Log("Spawning Souls: " + eventData.soulAmount);
        if (eventData.soulAmount > 0)
        {
            GameObject collectable = _collectablePool.GetSoulCollectable();
            if (collectable != null)
            {
                Vector3 spawnPosition = eventData.spawnPosition;
                spawnPosition.y = 3f;
                collectable.transform.position = spawnPosition;
                CollectableSoul soulCollectable = collectable.GetComponent<CollectableSoul>();
                soulCollectable.SetSoulValue(eventData.soulAmount);
                collectable.SetActive(true);
            }
        }
        return;
    }

    void HandleComponentDrop(OnDropComponent eventData)
    {
        if (eventData.isFromPlayer)
        {
            HandlePlayerDropComponent(eventData);
            return;
        }
        HandleEnemyDropComponent(eventData);
    }

    void HandleEnemyDropComponent(OnDropComponent eventData)
    {
        GameObject collectable = _collectablePool.GetComponentCollectable();
        if (collectable != null)
        {
            Vector3 m_dropPosition = new Vector3(
                eventData.position.x,
                0.5f,
                eventData.position.z

            );

            collectable.transform.position = m_dropPosition;
            collectable.GetComponent<CollectableComponent>().SetComponentData(eventData.data);
            collectable.GetComponent<CollectableComponent>().SpawnOnPosition(m_dropPosition, Vector3.zero);
            collectable.SetActive(true);
        }
    }
    void HandlePlayerDropComponent(OnDropComponent eventData)
    {
        GameObject collectable = _collectablePool.GetComponentCollectable();
        if (collectable != null)
        {
            Vector3 m_dropPosition = new Vector3(
                m_PlayerTransform.position.x,
                1f,
                m_PlayerTransform.position.z

            );

            collectable.transform.position = m_dropPosition;
            collectable.GetComponent<CollectableComponent>().SetComponentData(eventData.data);
            collectable.GetComponent<CollectableComponent>().SpawnOnPosition(m_dropPosition, m_PlayerTransform.forward * k_DropForce);
            collectable.SetActive(true);
        }
    }

    void OnDestroy()
    {
        UnbindEvents();
    }
}
