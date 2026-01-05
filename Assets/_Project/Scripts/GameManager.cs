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


    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;


    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;      // HUD Score
    [SerializeField] private TMP_Text finalScoreText; // ResultPanel Score
    [SerializeField] private float scorePerSecond = 10f;
    [SerializeField] private TMP_Text bestScoreText; // ResultPanel Best: 0

    [Header("Difficulty")]
    [SerializeField] private DifficultyController difficulty;

    [Header("Courier")]
    [SerializeField] private int requiredPackages = 3;
    [SerializeField] private TMP_Text packagesText;     // HUD Packages: 0/3
    [SerializeField] private TMP_Text moneyText;        // HUD Money: 0
    [SerializeField] private TMP_Text finalMoneyText;   // ResultPanel Money: 0 (опционально)
    [SerializeField] private int moneyRewardOnWin = 50;

    private int _bestScore;
    private const string BestScoreKey = "BEST_SCORE";


    private float _timeLeft;
    private bool _ended;

    private int _score;
    private float _scoreAcc;
    public bool IsPaused { get; private set; }

    public bool IsRunning { get; private set; }

    private int _packagesCollected;
    private int _money;

    private void Start()
    {
        Time.timeScale = 0f; // на вс€кий случай (после предыдущих запусков)

        _timeLeft = roundDurationSeconds;
        _ended = false;

        IsRunning = false;
        if (tapToStartText != null) tapToStartText.SetActive(true);

        Time.timeScale = 0f;

        if (resultPanel != null) resultPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        IsPaused = false;

        _score = 0;
        _scoreAcc = 0f;

        UpdateTimerUI();
        UpdateScoreUI();

        _bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        UpdateBestScoreUI();
        if (difficulty == null) difficulty = FindObjectOfType<DifficultyController>();
        if (difficulty != null) difficulty.ResetValues();

        _packagesCollected = 0;
        _money = PlayerPrefs.GetInt("Money", 0);

        UpdatePackagesUI();
        UpdateMoneyUI();

    }

    private void Update()
    {
        // ∆дЄм старта
        if (!IsRunning)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                StartRun();

            return;
        }

        // ≈сли уже завершили Ч ничего не обновл€ем
        if (_ended) return;

        // “аймер раунда
        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;

            if (_packagesCollected >= requiredPackages)
                Win();
            else
                Lose();

            return;
        }


        UpdateTimerUI();

        // —чЄт (за выживание)
        float sps = (difficulty != null) ? difficulty.ScorePerSecond : scorePerSecond;
        _scoreAcc += sps * Time.deltaTime;

        int newScore = Mathf.FloorToInt(_scoreAcc);

        if (newScore != _score)
        {
            _score = newScore;
            UpdateScoreUI();
        }
    }

    public void StartRun()
    {
        AudioManager.Instance?.PlayTapStart();
        IsRunning = true;
        if (pauseButton != null) pauseButton.SetActive(true);
        if (tapToStartText != null) tapToStartText.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Lose()
    {
        if (_ended) return;
        _ended = true;

        IsRunning = false;     // важно
        AudioManager.Instance?.PlayLose();
        ShowResult("YOU LOSE");
    }

    private void Win()
    {
        if (_ended) return;
        _ended = true;

        IsRunning = false;     // важно
        AudioManager.Instance?.PlayWin();
        _money += moneyRewardOnWin;
        PlayerPrefs.SetInt("Money", _money);
        PlayerPrefs.Save();
        UpdateMoneyUI();
        ShowResult("YOU WIN");
    }

    private void ShowResult(string title)
    {
        IsPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);

        Time.timeScale = 0f;

        if (resultTitle != null) resultTitle.text = title;
        UpdateScoreUI();

        // обновл€ем рекорд
        if (_score > _bestScore)
        {
            _bestScore = _score;
            PlayerPrefs.SetInt(BestScoreKey, _bestScore);
            PlayerPrefs.Save();
        }
        UpdateBestScoreUI();


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
    private void UpdateBestScoreUI()
    {
        if (bestScoreText != null)
            bestScoreText.text = $"Best: {_bestScore}";
    }
    private void UpdatePackagesUI()
    {
        if (packagesText != null)
            packagesText.text = $"Packages: {_packagesCollected}/{requiredPackages}";
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"Money: {_money}";

        if (finalMoneyText != null)
            finalMoneyText.text = $"Money: {_money}";
    }

    public void Pause()
    {
        if (_ended) return;               // если уже win/lose Ч пауза не нужна
        if (!IsRunning) return;           // если ещЄ не стартовали Ч тоже не надо

        IsPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void Quit()
    {
        // пока просто перезапускаем сцену (как "выход в меню" в будущем)
        Retry();
    }
    public void AddPackage()
    {
        if (_ended) return;
        if (!IsRunning) return; // чтобы до старта не считалось

        _packagesCollected++;
        if (_packagesCollected > requiredPackages)
            _packagesCollected = requiredPackages;

        UpdatePackagesUI();
    }

}
