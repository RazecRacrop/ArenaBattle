using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 2; 
    private int currentHealth;

    public GameObject lootDrop; 
    [Range(0, 100)] public int dropChance = 30; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Debug.Log("Enemy HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null) gm.AddScore(1);

        TryDropLoot();

        Destroy(gameObject);
    }

    void TryDropLoot()
    {
        if (lootDrop != null && Random.Range(0, 100) < dropChance)
        {
            Instantiate(lootDrop, transform.position, Quaternion.identity);
        }
    }
}