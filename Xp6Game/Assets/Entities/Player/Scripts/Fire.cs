using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject bulletPrefab;
    [Space]
    [SerializeField]
    private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFire = 0.0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (Time.time >= nextFire)
            {
                Shoot();
                nextFire = Time.time + fireRate;
            }
        }
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
