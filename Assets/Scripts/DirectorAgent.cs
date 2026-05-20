using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DirectorAgent : Agent
{
    [Header("--- Referințe ---")]
    private GameManager gameManager;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private Transform playerTransform;

    [Header("--- Detectare Camping ---")]
    private Vector3 lastPlayerPosition;
    private float campTimer = 0f;
    private float distanceMovedInTimeWindow = 0f;
    public float campCheckInterval = 3f; // Verificăm la fiecare 3 secunde

    public override void Initialize()
    {
        gameManager = FindObjectOfType<GameManager>();
        FindPlayer();
    }

    public override void OnEpisodeBegin()
    {
        FindPlayer();
        if (playerTransform != null) lastPlayerPosition = playerTransform.position;
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerController = player.GetComponent<PlayerController>();
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        // Calculăm cât se mișcă jucătorul la fiecare 3 secunde
        if (playerTransform != null)
        {
            campTimer += Time.deltaTime;
            if (campTimer >= campCheckInterval)
            {
                distanceMovedInTimeWindow = Vector3.Distance(playerTransform.position, lastPlayerPosition);
                lastPlayerPosition = playerTransform.position;
                campTimer = 0f;
            }
        }
    }

    // --- 1. OBSERVAȚIILE ---
    public override void CollectObservations(VectorSensor sensor)
    {
        if (playerHealth == null)
        {
            sensor.AddObservation(0f); // Viață
            sensor.AddObservation(0f); // Acuratețe
            sensor.AddObservation(0f); // Muniție
            sensor.AddObservation(0f); // Distanță parcursă
            return;
        }

        sensor.AddObservation((float)playerHealth.currentHealth / playerHealth.maxHealth);
        sensor.AddObservation(gameManager.GetPlayerAccuracy());
        
        float totalAmmo = playerController.rifleAmmo + playerController.shotgunAmmo;
        sensor.AddObservation(Mathf.Clamp01(totalAmmo / 40f)); 
        
        // Cât de mult s-a mișcat (dacă e sub 2.0f, e clar că stă pe loc / campează)
        sensor.AddObservation(distanceMovedInTimeWindow);
    }

    // --- 2. ACȚIUNILE ---
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (playerHealth == null) 
        {
            SetReward(-1.0f); EndEpisode(); return;
        }

        // Citim cele 3 decizii luate de AI
        int pacingAction = actions.DiscreteActions[0]; // Ritm
        int lootAction = actions.DiscreteActions[1];   // Loot
        int campAction = actions.DiscreteActions[2];   // Anti-Camp

        // Trimitem deciziile către GameManager
        gameManager.UpdateDirectorStates(pacingAction, lootAction, campAction);

        // --- RECOMPENSA (Flow Theory) ---
        float healthPercent = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        if (healthPercent >= 0.3f && healthPercent <= 0.8f) AddReward(0.01f); 
        else AddReward(-0.005f); 
    }

    // --- 3. MODUL DEMO (Heuristic) ---
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        if (playerHealth == null) return;

        float healthPercent = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        float accuracy = gameManager.GetPlayerAccuracy();

        // 1. Logica pentru Ritm (Pacing)
        if (healthPercent < 0.35f) discreteActions[0] = 0; // Relaxare
        else if (healthPercent > 0.75f && accuracy > 0.4f) discreteActions[0] = 2; // Tensiune
        else discreteActions[0] = 1; // Normal

        // 2. Logica pentru Loot
        if (healthPercent < 0.25f) discreteActions[1] = 0; // Înger (Dă viață!)
        else if (healthPercent > 0.9f) discreteActions[1] = 2; // Demon (Zgârcit)
        else discreteActions[1] = 1; // Normal

        // 3. Logica Anti-Camping
        if (distanceMovedInTimeWindow < 2.5f && campTimer < 0.1f) 
        {
            discreteActions[2] = 1; // Flush Out!
            Debug.Log("⚠️ AI: Jucătorul campează! Inițiez Flush Out!");
        }
        else discreteActions[2] = 0; 
    }
}