using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip deliveryClip;
    [SerializeField] private AudioClip loseClip;
    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioClip swipeSfx;


    [Header("Volume")]
    [Range(0f, 1f)][SerializeField] private float pickupVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float deliveryVolume = 0.9f;
    [Range(0f, 1f)][SerializeField] private float loseVolume = 1.0f;
    [Range(0f, 1f)][SerializeField] private float uiClickVolume = 0.7f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    public void PlayPickup()
    {
        PlayOneShotSafe(pickupClip, pickupVolume);
    }

    public void PlayDelivery()
    {
        PlayOneShotSafe(deliveryClip, deliveryVolume);
    }

    public void PlayLose()
    {
        PlayOneShotSafe(loseClip, loseVolume);
    }

    public void PlayUIClick()
    {
        PlayOneShotSafe(uiClickClip, uiClickVolume);
    }

    private void PlayOneShotSafe(AudioClip clip, float volume)
    {
        if (sfxSource == null) return;
        if (clip == null) return;

        sfxSource.PlayOneShot(clip, volume);
    }
    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        if (sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }
    public void PlaySwipe()
    {
        PlaySfx(swipeSfx);
    }

}
