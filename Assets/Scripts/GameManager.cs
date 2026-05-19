using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; 

public class GameManager : MonoBehaviour
{
    [Header("--- Setari Inamici ---")]
    public GameObject enemyPrefab;    
    public GameObject warningPrefab;  
    public float spawnInterval = 2.0f; 
    public float warningTime = 1.5f;  
    public float xLimit = 25f; 
    public float zLimit = 25f;

    [Header("--- Setari UI ---")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public GameObject gameOverPanel;
    
    private int score = 0;
    private bool isGameActive = true;
    private float timer;

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
        if (timer >= spawnInterval)
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

        if (isGameActive)
        {
            Destroy(warning); 
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity); 
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