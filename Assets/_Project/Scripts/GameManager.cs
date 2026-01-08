using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Round")]
    [SerializeField] private float roundDurationSeconds = 45f;

    [Header("UI - HUD")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;       // HUD "Score: X"
    [SerializeField] private TMP_Text packagesText;    // HUD "Packages: X"
    [SerializeField] private TMP_Text moneyText;       // HUD "Money: X"
    [SerializeField] private TMP_Text orderText;       // HUD "Order: X/Y"
    [SerializeField] private GameObject tapToStartText;

    [Header("UI - Result")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultTitle;
    [SerializeField] private TMP_Text finalScoreText;  // ResultPanel "Score: X"
    [SerializeField] private TMP_Text bestScoreText;   // ResultPanel "Best: X"

    [Header("UI - Pause")]
    [SerializeField] private GameObject pausePanel;

    [Header("UI - Delivery Toast")]
    [SerializeField] private DeliveryToastUI deliveryToastUI;

    [Header("Score")]
    [SerializeField] private float scorePerSecond = 2f;

    [Header("Order / Delivery")]
    [SerializeField] private int orderTarget = 3;      // сколько пакетов нужно для “доставки”
    [SerializeField] private int rewardMoney = 50;     // награда за доставку

    private float _timeLeft;
    private bool _ended;

    public bool IsRunning { get; private set; }

    private int _score;
    private float _scoreAcc;

    private int _packages;         // сколько пакетов подняли в текущей попытке (для HUD)
    private int _money;            // деньги (пока только в рантайме)
    private int _orderCollected;   // сколько пакетов собрано в текущем заказе

    private int _bestScore;

    private bool _paused;
    public bool IsPaused => _paused;

    private void Start()
    {
        Time.timeScale = 1f;

        _timeLeft = roundDurationSeconds;
        _ended = false;

        IsRunning = false;
        if (tapToStartText != null) tapToStartText.SetActive(true);

        if (resultPanel != null) resultPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (deliveryToastUI != null) deliveryToastUI.HideImmediate();

        _score = 0;
        _scoreAcc = 0f;

        _packages = 0;
        _money = 0;
        _orderCollected = 0;

        _bestScore = PlayerPrefs.GetInt("BEST_SCORE", 0);

        UpdateTimerUI();
        UpdateScoreUI();
        UpdateEconomyUI();
        UpdateOrderUI();
        UpdateBestUI();
    }

    private void Update()
    {
        if (_ended) return;
        if (_paused) return;

        if (!IsRunning)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                StartRun();
            return;
        }

        // таймер
        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            Win();
        }
        UpdateTimerUI();

        // скор (плавно)
        _scoreAcc += scorePerSecond * Time.deltaTime;
        int newScore = Mathf.FloorToInt(_scoreAcc);
        if (newScore != _score)
        {
            _score = newScore;
            UpdateScoreUI();
        }
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

    private void UpdateBestUI()
    {
        if (bestScoreText != null) bestScoreText.text = $"Best: {_bestScore}";
    }

    private void UpdateEconomyUI()
    {
        if (packagesText != null) packagesText.text = $"Packages: {_packages}";
        if (moneyText != null) moneyText.text = $"Money: {_money}";
    }

    private void UpdateOrderUI()
    {
        if (orderText != null) orderText.text = $"Order: {_orderCollected}/{orderTarget}";
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
        // обновим best
        if (_score > _bestScore)
        {
            _bestScore = _score;
            PlayerPrefs.SetInt("BEST_SCORE", _bestScore);
            PlayerPrefs.Save();
        }
        UpdateBestUI();

        Time.timeScale = 0f;

        if (resultTitle != null)
            resultTitle.text = title;

        if (resultPanel != null)
            resultPanel.SetActive(true);
    }

    public void Retry()
    {
        _paused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =========================
    // PUBLIC API for pickups
    // =========================
    public void OnPackageCollected()
    {
        if (_ended) return;
        if (!IsRunning) return;
        if (_paused) return;

        _packages++;
        _orderCollected++;

        UpdateEconomyUI();
        UpdateOrderUI();

        // если заказ выполнен
        if (_orderCollected >= orderTarget)
        {
            _orderCollected = 0;     // новый заказ
            _money += rewardMoney;

            UpdateEconomyUI();
            UpdateOrderUI();

            if (deliveryToastUI != null)
                deliveryToastUI.Show($"+${rewardMoney} DELIVERY COMPLETE");
        }
    }

    // =========================
    // Pause
    // =========================
    public void Pause()
    {
        if (_ended) return;
        if (!IsRunning) return;
        if (_paused) return;

        _paused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void Resume()
    {
        if (!_paused) return;

        _paused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
}
