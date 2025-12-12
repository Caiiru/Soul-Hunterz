using UnityEngine;

public class UiBillboard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool m_isInverse = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        transform.forward = Camera.main.transform.forward;
        if (m_isInverse)
            transform.forward *= -1;
    }
}
