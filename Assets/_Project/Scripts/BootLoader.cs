using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "MainMenu";

    private void Awake()
    {
        // Здесь позже будут инициализации: локализация, настройки, сервисы.
        SceneManager.LoadScene(nextSceneName);
    }
}
