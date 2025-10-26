using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip windowsErrorSound;
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public void PlayButtonClickSound() =>
        audioSource.PlayOneShot(buttonClickSound);

    public void PlayAIErrorSound() =>
        audioSource.PlayOneShot(windowsErrorSound);

}
