using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("--- Miscare ---")]
    public float moveSpeed = 5f;
    public Rigidbody rb;
    public Camera cam;
    Vector3 movement;
    Vector3 mousePos;

    [Header("--- Arme ---")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    
    // Definim tipurile de arme posibile
    public enum WeaponType { Rifle, Shotgun }
    public WeaponType currentWeapon = WeaponType.Rifle; 

    [Header("--- Setari Rifle ---")]
    public float rifleFireRate = 0.5f; 

    [Header("--- Setari Shotgun ---")]
    public float shotgunFireRate = 1.0f; 
    public int pelletCount = 5;          
    public float spreadAngle = 15f;      

    private float nextFireTime = 0f;

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            mousePos = hitInfo.point;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentWeapon = WeaponType.Rifle;
            Debug.Log("Arma: Pusca (Precizie)");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentWeapon = WeaponType.Shotgun;
            Debug.Log("Arma: Shotgun (Imprastiere)");
        }

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        Vector3 lookDir = mousePos - rb.position;
        lookDir.y = 0; 
        transform.rotation = Quaternion.LookRotation(lookDir);
    }

    void Shoot()
    {
        if (currentWeapon == WeaponType.Rifle)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            
            nextFireTime = Time.time + rifleFireRate;
        }
        else if (currentWeapon == WeaponType.Shotgun)
        {
            
            for (int i = 0; i < pelletCount; i++)
            {
                float randomAngle = Random.Range(-spreadAngle, spreadAngle);
                
                Quaternion spreadRotation = firePoint.rotation * Quaternion.Euler(0, randomAngle, 0);

                Instantiate(bulletPrefab, firePoint.position, spreadRotation);
            }

            nextFireTime = Time.time + shotgunFireRate;
        }
    }
}