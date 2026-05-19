using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 20;
    public float lifeTime = 5f; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item: Am atins Player-ul!"); 
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                if (playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    playerHealth.currentHealth += healthAmount;
                    
                    if (playerHealth.currentHealth > playerHealth.maxHealth)
                    {
                        playerHealth.currentHealth = playerHealth.maxHealth;
                    }

                    if (playerHealth.healthSlider != null)
                    {
                        playerHealth.healthSlider.value = playerHealth.currentHealth;
                    }

                    Debug.Log("Item: Viata adaugata!");
                    Destroy(gameObject); 
                }
                else
                {
                    Debug.Log("Item: Ai viata plina, nu il pot lua!");
                }
            }
        }
    }
}