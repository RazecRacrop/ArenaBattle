using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Speed, Damage }
    public PowerUpType type;
    
    public float duration = 5f; 
    public float value = 2f;    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            
            if (player != null)
            {
                if (type == PowerUpType.Speed)
                {
                    player.StartCoroutine(player.TriggerSpeedBoost(value, duration));
                }
                else if (type == PowerUpType.Damage)
                {
                    player.StartCoroutine(player.TriggerDamageBuff((int)value, duration));
                }
            }

            Destroy(gameObject); 
            
        }
    }
}