using UnityEngine;

namespace GemBlast.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Clips")]
        [SerializeField] private AudioClip popSound;
        [SerializeField] private AudioClip bigPopSound;
        [SerializeField] private AudioClip dropSound;

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;

        private AudioSource _audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
        }

        public void PlayPop()
        {
            if (popSound != null)
                _audioSource.PlayOneShot(popSound, sfxVolume);
        }

        public void PlayBigPop()
        {
            if (bigPopSound != null)
                _audioSource.PlayOneShot(bigPopSound, sfxVolume);
        }

        public void PlayDrop()
        {
            if (dropSound != null)
                _audioSource.PlayOneShot(dropSound, sfxVolume * 0.5f);
        }

        public void PlayBlast(bool isBig)
        {
            if (isBig)
                PlayBigPop();
            else
                PlayPop();
        }
    }
}
