using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Round")]
    [SerializeField] private float roundDurationSeconds = 45f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultTitle;
    [SerializeField] private GameObject tapToStartText;

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;      // HUD Score
    [SerializeField] private TMP_Text finalScoreText; // ResultPanel Score
    [SerializeField] private float scorePerSecond = 10f;

    private float _timeLeft;
    private bool _ended;

    private int _score;
    private float _scoreAcc;

    public bool IsRunning { get; private set; }

    private void Start()
    {
        Time.timeScale = 1f; // на всякий случай (после предыдущих запусков)

        _timeLeft = roundDurationSeconds;
        _ended = false;

        IsRunning = false;
        if (tapToStartText != null) tapToStartText.SetActive(true);

        if (resultPanel != null) resultPanel.SetActive(false);

        _score = 0;
        _scoreAcc = 0f;

        UpdateTimerUI();
        UpdateScoreUI();
    }

    private void Update()
    {
        // Ждём старта
        if (!IsRunning)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                StartRun();

            return;
        }

        // Если уже завершили — ничего не обновляем
        if (_ended) return;

        // Таймер раунда
        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            Win();
            return;
        }

        UpdateTimerUI();

        // Счёт (за выживание)
        _scoreAcc += scorePerSecond * Time.deltaTime;
        int newScore = Mathf.FloorToInt(_scoreAcc);

        if (newScore != _score)
        {
            _score = newScore;
            UpdateScoreUI();
        }
    }

    public void StartRun()
    {
        IsRunning = true;
        if (tapToStartText != null) tapToStartText.SetActive(false);
    }

    public void Lose()
    {
        if (_ended) return;
        _ended = true;

        IsRunning = false;     // важно
        ShowResult("YOU LOSE");
    }

    private void Win()
    {
        if (_ended) return;
        _ended = true;

        IsRunning = false;     // важно
        ShowResult("YOU WIN");
    }

    private void ShowResult(string title)
    {
        Time.timeScale = 0f;

        if (resultTitle != null) resultTitle.text = title;
        UpdateScoreUI();

        if (resultPanel != null) resultPanel.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int seconds = Mathf.CeilToInt(_timeLeft);
        int m = seconds / 60;
        int s = seconds % 60;

        timerText.text = $"{m:00}:{s:00}";
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {_score}";
        if (finalScoreText != null) finalScoreText.text = $"Score: {_score}";
    }
}
