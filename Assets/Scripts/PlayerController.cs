using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("--- Miscare ---")]
    public float moveSpeed = 5f;
    public Rigidbody rb;
    public Camera cam;
    Vector3 movement;
    Vector3 mousePos;
    private float baseMoveSpeed;

    [Header("--- Arme ---")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    
    public enum WeaponType { Rifle, Shotgun, Melee }
    public WeaponType currentWeapon = WeaponType.Rifle; 

    [Header("--- Setari Rifle ---")]
    public float rifleFireRate = 0.5f; 
    public int rifleAmmo = 30; // Nou: Muniție Rifle

    [Header("--- Setari Shotgun ---")]
    public float shotgunFireRate = 1.0f; 
    public int pelletCount = 5;          
    public float spreadAngle = 15f;      
    public int shotgunAmmo = 10; // Nou: Muniție Shotgun

    [Header("--- Atac Melee (Sabie) ---")]
    public float meleeAttackRate = 0.6f;
    public float meleeDamage = 0.5f; // 2 lovituri inamic normal, 6 tancul
    public float meleeRadius = 2.5f; // Raza atacului în jurul jucătorului
    public LayerMask enemyLayer;    // Setează pe "Enemy" în Inspector

    [Header("--- Power-ups ---")]
    public float baseDamage = 1f; // Schimbat în float pentru consistență
    public float currentDamage = 1f;

    private float nextFireTime = 0f;

    void Start()
    {
        baseMoveSpeed = moveSpeed;
        currentDamage = baseDamage;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            mousePos = hitInfo.point;
        }

        // Schimbare arme doar dacă avem muniție
        if (Input.GetKeyDown(KeyCode.Q) && rifleAmmo > 0)
        {
            currentWeapon = WeaponType.Rifle;
            Debug.Log("Arma: Pusca");
        }
        if (Input.GetKeyDown(KeyCode.E) && shotgunAmmo > 0)
        {
            currentWeapon = WeaponType.Shotgun;
            Debug.Log("Arma: Shotgun");
        }

        // Verificare automată: dacă ambele sunt 0, trecem pe Melee
        if (rifleAmmo <= 0 && shotgunAmmo <= 0 && currentWeapon != WeaponType.Melee)
        {
            currentWeapon = WeaponType.Melee;
            Debug.Log("⚠️ MUNIIE TERMINATĂ! Mod Melee activat (Sabie).");
        }

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            if (currentWeapon == WeaponType.Melee)
            {
                MeleeAttack();
            }
            else
            {
                Shoot();
            }
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
        if (currentWeapon == WeaponType.Rifle && rifleAmmo > 0)
        {
            GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            newBullet.GetComponent<Bullet>().bulletDamage = currentDamage;
            
            rifleAmmo--; // Consumă glonț
            FindObjectOfType<GameManager>().totalShotsFired++;
            nextFireTime = Time.time + rifleFireRate;
        }
        else if (currentWeapon == WeaponType.Shotgun && shotgunAmmo > 0)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                float randomAngle = Random.Range(-spreadAngle, spreadAngle);
                Quaternion spreadRotation = firePoint.rotation * Quaternion.Euler(0, randomAngle, 0);

                GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, spreadRotation);
                newBullet.GetComponent<Bullet>().bulletDamage = currentDamage;
                
                FindObjectOfType<GameManager>().totalShotsFired++;
            }

            shotgunAmmo--; // Consumă un cartuș de shotgun
            nextFireTime = Time.time + shotgunFireRate;
        }
    }

    void MeleeAttack()
    {
        Debug.Log("⚔️ Atac cu sabia!");
        nextFireTime = Time.time + meleeAttackRate;

        // Detectăm toți inamicii din jurul nostru într-o sferă
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, meleeRadius, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHP = enemy.GetComponent<EnemyHealth>();
            if (enemyHP != null)
            {
                enemyHP.TakeDamage(meleeDamage);
            }
        }
    }

    // Funcție apelată când culegem muniție de pe jos
    public void AddAmmo(int rifleAmount, int shotgunAmount)
    {
        rifleAmmo += rifleAmount;
        shotgunAmmo += shotgunAmount;
        
        // Dacă eram pe Melee, ne întoarcem automat la arma principală disponibilă
        if (currentWeapon == WeaponType.Melee)
        {
            currentWeapon = rifleAmount > 0 ? WeaponType.Rifle : WeaponType.Shotgun;
        }
        Debug.Log("Muniție adăugată! Rifle: " + rifleAmmo + " | Shotgun: " + shotgunAmmo);
    }

    public IEnumerator TriggerSpeedBoost(float multiplier, float duration)
    {
        moveSpeed = baseMoveSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = baseMoveSpeed;
    }

    public IEnumerator TriggerDamageBuff(float bonusDamage, float duration)
    {
        currentDamage = baseDamage + bonusDamage;
        yield return new WaitForSeconds(duration);
        currentDamage = baseDamage;
    }

    // Vizualizarea razei de atac în editorul Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRadius);
    }
}