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
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private GameObject tapToStartText;

    private float _timeLeft;
    private bool _ended;
    private float _score;
    public bool IsRunning { get; private set; }



    private void Start()
    {
        _timeLeft = roundDurationSeconds;
        _ended = false;
        IsRunning = false;
        tapToStartText.gameObject.SetActive(true);

        if (resultPanel != null)
            resultPanel.SetActive(false);

        UpdateTimerUI();
        _score = 0f;
        UpdateScoreUI();

    }

    private void Update()
    {
        if (!IsRunning)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                StartRun();

            return;
        }

        if (_ended) return;

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            Win();
        }

        UpdateTimerUI();
        _score += Time.deltaTime * 10f; // 10 очков в секунду
        UpdateScoreUI();

    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int seconds = Mathf.CeilToInt(_timeLeft);
        int m = seconds / 60;
        int s = seconds % 60;
        timerText.text = $"{m:00}:{s:00}";
    }

    public void Lose()
    {
        if (_ended) return;
        _ended = true;
        ShowResult("YOU LOSE");
    }

    private void Win()
    {
        if (_ended) return;
        _ended = true;
        ShowResult("YOU WIN");
    }

    private void ShowResult(string title)
    {
        Time.timeScale = 0f;

        if (resultTitle != null)
            resultTitle.text = title;

        if (resultPanel != null)
            resultPanel.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void UpdateScoreUI()
    {
        if (scoreText == null) return;
        scoreText.text = $"Score: {Mathf.FloorToInt(_score)}";
    }
    public void StartRun()
    {
        IsRunning = true;
        if (tapToStartText != null) tapToStartText.SetActive(false);
    }

}
