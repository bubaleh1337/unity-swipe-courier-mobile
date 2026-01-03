using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip tapStart;
    [SerializeField] private AudioClip swipe;
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip lose;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    public void PlayTapStart() => Play(tapStart);
    public void PlaySwipe() => Play(swipe);
    public void PlayHit() => Play(hit);
    public void PlayWin() => Play(win);
    public void PlayLose() => Play(lose != null ? lose : hit);

    private void Play(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
