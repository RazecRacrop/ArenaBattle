using UnityEngine;
using UnityEngine.AI; 

public class EnemyAI : MonoBehaviour
{
    // Nu mai avem nevoie de 'speed' aici, viteza se seteaza acum din Inspector de la NavMeshAgent
    public int damage = 10; 
    
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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
            
            if (health != null)
            {
                health.TakeDamage(damage); 
            }

            Destroy(gameObject);
        }
    }
}