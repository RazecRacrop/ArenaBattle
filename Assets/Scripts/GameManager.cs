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

    // Funcție pe care o va citi ML-Agents mai târziu
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

    IEnumerator SpawnEnemyRoutine()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-xLimit, xLimit), 0.05f, Random.Range(-zLimit, zLimit));
        GameObject warning = Instantiate(warningPrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(warningTime);

        // Mai facem o verificare în caz că Boss-ul a apărut cât timp dura avertismentul
        if (isGameActive && !bossSpawned)
        {
            Destroy(warning); 
            
            // AI-ul alege: 80% șanse inamic normal, 20% șanse Tanc
            GameObject prefabToSpawn = (Random.value > 0.2f) ? normalEnemyPrefab : tankEnemyPrefab;
            
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity); 
        }
        else
        {
            Destroy(warning);
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
        
        // Spawnăm Boss-ul fix în centrul hărții
        Vector3 centerPos = new Vector3(0, 0.5f, 0);
        if(bossPrefab != null) 
        {
            Instantiate(bossPrefab, centerPos, Quaternion.identity);
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
}