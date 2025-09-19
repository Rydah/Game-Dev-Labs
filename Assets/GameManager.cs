using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [System.NonSerialized]
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public Canvas GameOverCanvas;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    public void Reset()
    {
        Time.timeScale = 1.0f;
        GameOverCanvas.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowGameOverScreen()
    {
        GameOverCanvas.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }
}
