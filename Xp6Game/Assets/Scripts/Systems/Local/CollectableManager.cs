using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    CollectablePool _collectablePool;


    Transform m_PlayerTransform;

    protected static float k_DropForce = 300f;

    //Events
    EventBinding<OnDropComponent> m_OnPlayerDropComponent;

    void Start()
    {
        BindObjects();
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
            collectable.GetComponent<CollectableItem>().SetComponentData(eventData.data);
            collectable.GetComponent<CollectableItem>().SpawnOnPosition(m_dropPosition, m_PlayerTransform.forward * k_DropForce);
            collectable.SetActive(true);
        }
    }

    void OnDisable()
    {

        EventBus<OnDropComponent>.Unregister(m_OnPlayerDropComponent);
    }
}
