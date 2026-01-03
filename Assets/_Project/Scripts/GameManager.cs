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

    private float _timeLeft;
    private bool _ended;

    private void Start()
    {
        _timeLeft = roundDurationSeconds;
        _ended = false;

        if (resultPanel != null)
            resultPanel.SetActive(false);

        UpdateTimerUI();
    }

    private void Update()
    {
        if (_ended) return;

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            Win();
        }

        UpdateTimerUI();
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
}
