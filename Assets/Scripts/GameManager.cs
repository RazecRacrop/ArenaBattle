using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; 

public class GameManager : MonoBehaviour
{
    [Header("--- Tipuri de Inamici ---")]
    public GameObject normalEnemyPrefab; 
    public GameObject tankEnemyPrefab;   
    public GameObject bossPrefab;       

    [Header("--- Setari Spawn ---")]
    public GameObject warningPrefab;  
    public float spawnInterval = 2.0f; 
    public float warningTime = 1.5f;  
    public float xLimit = 25f; 
    public float zLimit = 25f;

    [Header("--- Setari Boss ---")]
    public int scoreRequiredForBoss = 100;
    private bool bossSpawned = false;

    [Header("--- Setari UI ---")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public GameObject gameOverPanel;
    
    private int score = 0;
    private bool isGameActive = true;
    private float timer;

    [Header("--- Telemetrie AI ---")]
    public int totalShotsFired = 0;
    public int totalShotsHit = 0;

      [Header("--- Setări Director AI ---")]
    public int currentPacing = 1; // 0=Relax, 1=Normal, 2=Tension
    public int currentLootState = 1; // 0=Angel, 1=Normal, 2=Demon
    public bool isFlushingOut = false;

    [Header("--- Sistem Central de Loot ---")]
    public GameObject[] lootTable; 
    public GameObject healthPrefab; 

    
    public float GetPlayerAccuracy()
    {
        if (totalShotsFired == 0) return 0f;
        return (float)totalShotsHit / totalShotsFired;
    }

    void Start()
    {
        score = 0;
        UpdateScoreText();
        Time.timeScale = 1; 
    }

    void Update()
    {
        if (!isGameActive) return;

        timer += Time.deltaTime;
        
        // Dacă a apărut Boss-ul, oprim spawnarea inamicilor mici
        if (timer >= spawnInterval && !bossSpawned)
        {
            StartCoroutine(SpawnEnemyRoutine());
            timer = 0;
        }
    }

     public void UpdateDirectorStates(int pacing, int loot, int camp)
    {
        currentPacing = pacing;
        currentLootState = loot;
        isFlushingOut = (camp == 1);

        if (currentPacing == 0) spawnInterval = 4.0f;
        else if (currentPacing == 1) spawnInterval = 2.0f;
        else if (currentPacing == 2) spawnInterval = 0.8f;
    }


    IEnumerator SpawnEnemyRoutine()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-xLimit, xLimit), 0.05f, Random.Range(-zLimit, zLimit));
        
        // ANTI-CAMPING: Dacă jucătorul campează, spawnăm inamicul FOARTE aproape de el
        if (isFlushingOut)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Spawnăm la 5 unități în spatele/lângă jucător
                spawnPos = player.transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
            }
        }

        GameObject warning = Instantiate(warningPrefab, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(warningTime);

        if (isGameActive && !bossSpawned)
        {
            Destroy(warning); 
            
            GameObject prefabToSpawn = normalEnemyPrefab;

            if (currentPacing == 0) 
            {
                prefabToSpawn = normalEnemyPrefab; 
            }
            else if (currentPacing == 1) 
            {
                prefabToSpawn = (Random.value > 0.2f) ? normalEnemyPrefab : tankEnemyPrefab; // Normal
            }
            else if (currentPacing == 2) 
            {
                prefabToSpawn = (Random.value > 0.5f) ? normalEnemyPrefab : tankEnemyPrefab; // Tensiune: 50% șanse de Tanc!
            }
            
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity); 
        }
        else Destroy(warning);
    }


    public void RollForLoot(Vector3 dropPosition)
    {
        if (lootTable == null || lootTable.Length == 0) return;

        if (currentLootState == 0) // ÎNGER (Jucătorul moare)
        {
            if (Random.value < 0.8f) // 80% șanse să pice ceva
            {
                // Forțăm să pice VIAȚĂ dacă o avem setată
                if (healthPrefab != null) Instantiate(healthPrefab, dropPosition, Quaternion.identity);
                else Instantiate(lootTable[Random.Range(0, lootTable.Length)], dropPosition, Quaternion.identity);
            }
        }
        else if (currentLootState == 1) // NORMAL
        {
            if (Random.value < 0.3f) // 30% șanse
                Instantiate(lootTable[Random.Range(0, lootTable.Length)], dropPosition, Quaternion.identity);
        }
        else if (currentLootState == 2) // DEMON (Jucătorul joacă prea bine)
        {
            if (Random.value < 0.05f) // Doar 5% șanse! Foarte zgârcit.
                Instantiate(lootTable[Random.Range(0, lootTable.Length)], dropPosition, Quaternion.identity);
        }
    }

    public void AddScore(int amount)
    {
        if (!isGameActive) return;
        score += amount;
        UpdateScoreText();

        // Verificăm dacă am atins scorul de Boss
        if (score >= scoreRequiredForBoss && !bossSpawned)
        {
            SpawnBossFight();
        }
    }

    void SpawnBossFight()
    {
        bossSpawned = true;
        Debug.Log("👹 !!! BOSS FIGHT !!! 👹");
        
        if(bossPrefab != null) 
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 spawnPos = new Vector3(0, 0.5f, 0); 

            if (player != null)
            {
                
                spawnPos = player.transform.position + new Vector3(12f, 0f, 12f);
            }

            Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogError("🚨 EROARE: Nu ai pus BossPrefab în GameManager!");
        }
    }

    void UpdateScoreText()
    {
        if(scoreText != null) scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        isGameActive = false;
        if(gameOverPanel != null) gameOverPanel.SetActive(true);

        if(finalScoreText != null)
        {
            finalScoreText.text = "FINAL SCORE: " + score;
        }

        Time.timeScale = 0; 
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BossDefeated()
    {
        Debug.Log("🏆 Boss-ul a fost învins! Reluăm spawn-ul.");
        bossSpawned = false; 
        
        scoreRequiredForBoss += 150; 
    }
}