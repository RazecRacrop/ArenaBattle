using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifeTime); 
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHP = collision.gameObject.GetComponent<EnemyHealth>();
            
            if (enemyHP != null)
            {
                enemyHP.TakeDamage(1); 
            }
            else
            {
                Destroy(collision.gameObject);
                
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null) gm.AddScore(1);
            }

            Destroy(gameObject); 
        }
        else if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Bullet")) 
        {
            Destroy(gameObject); 
        }
    }
}