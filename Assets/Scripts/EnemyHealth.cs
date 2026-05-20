using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 1f;


    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null) 
        {
            // Cerem Directorului să decidă dacă ne dă loot sau nu
            gm.RollForLoot(transform.position);

            // Scorul
            if (gameObject.name.Contains("Tank")) gm.AddScore(3);
            else if (gameObject.name.Contains("Boss")) 
            {
                gm.AddScore(50);
                gm.BossDefeated(); 
            }
            else gm.AddScore(1); 
        }
        
        Destroy(gameObject);
    }
}