using UnityEngine;
using UnityEngine.AI; 

public class EnemyAI : MonoBehaviour
{
    // Nu mai avem nevoie de 'speed' aici, viteza se seteaza acum din Inspector de la NavMeshAgent
    public int damage = 10; 
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;
    
    private Transform player;
    private NavMeshAgent agent; // Am adaugat "soferul" inamicului

    void Start()
    {
        // Preluam componenta de navigatie (care trebuie sa fie pe inamic)
        agent = GetComponent<NavMeshAgent>();

        // Cautam jucatorul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // Daca jucatorul exista si agentul e activ, ii spunem sa calculeze ruta
        if (player != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.position);
        }
    }

    // Partea ta de coliziune a ramas EXACT la fel! Face damage si moare.
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Verificăm dacă a trecut 1 secundă de la ultimul atac
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
                
                if (health != null)
                {
                    health.TakeDamage(damage); 
                }

                lastAttackTime = Time.time; // Resetăm timer-ul de atac

                
                if (!gameObject.name.Contains("Tank") && !gameObject.name.Contains("Boss"))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}