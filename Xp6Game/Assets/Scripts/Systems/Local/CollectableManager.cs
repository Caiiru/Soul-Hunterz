using System;
using UnityEngine;

[RequireComponent(typeof(CollectablePool))]
public class CollectableManager : MonoBehaviour
{
    CollectablePool _collectablePool;


    Transform m_PlayerTransform;

    protected static float k_DropForce = 300f;

    //Events
    EventBinding<OnDropComponent> m_OnPlayerDropComponent;

    EventBinding<GameReadyToStartEvent> m_OnGameReadyToStart;


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
        m_OnPlayerDropComponent = new EventBinding<OnDropComponent>(HandlePlayerDropComponent);
        EventBus<OnDropComponent>.Register(m_OnPlayerDropComponent);
        m_OnGameReadyToStart = new EventBinding<GameReadyToStartEvent>(HandleGameReadyToStart);
        EventBus<GameReadyToStartEvent>.Register(m_OnGameReadyToStart);


    }
    private void UnbindEvents()
    {
        EventBus<OnDropComponent>.Unregister(m_OnPlayerDropComponent);
        EventBus<GameReadyToStartEvent>.Unregister(m_OnGameReadyToStart);
    }

    private void HandleGameReadyToStart(GameReadyToStartEvent arg0)
    {
        BindObjects();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void HandlePlayerDropComponent(OnDropComponent eventData)
    {
        GameObject collectable = _collectablePool.GetCollectable();
        if (collectable != null)
        {
            Vector3 m_dropPosition = new Vector3(
                m_PlayerTransform.position.x,
                m_PlayerTransform.position.y + 0.5f,
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
