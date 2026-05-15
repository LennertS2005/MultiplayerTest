using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxPrefab;

    [Header("Sound Library")]
    [SerializeField] private List<Sound> sounds = new();

    private Dictionary<string, Sound> soundDictionary = new();
    private Queue<AudioSource> pool = new();

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Build lookup dictionary
        foreach (var sound in sounds)
        {
            if (!soundDictionary.ContainsKey(sound.id))
            {
                soundDictionary.Add(sound.id, sound);
            }
        }

        // Create pool
        for (int i = 0; i < 20; i++)
        {
            AudioSource source = Instantiate(sfxPrefab, transform);
            source.gameObject.SetActive(false);
            pool.Enqueue(source);
        }

        PlayMusic("MainTheme");
    }

    // ------------------------
    // PLAY SFX (2D)
    // ------------------------

    public void PlaySFX(string id)
    {
        if (!soundDictionary.TryGetValue(id, out Sound sound))
        {
            Debug.LogWarning($"Sound {id} not found.");
            return;
        }

        AudioSource source = GetSource();

        source.transform.position = Vector3.zero;
        source.spatialBlend = 0f;

        SetupSource(source, sound);

        source.Play();

        print("Playing audio: " + sound.id);

        StartCoroutine(ReturnToPool(source, sound.clip.length));
    }

    // ------------------------
    // PLAY SFX (3D)
    // ------------------------

    public void PlaySFX(string id, Vector3 position)
    {
        if (!soundDictionary.TryGetValue(id, out Sound sound))
        {
            Debug.LogWarning($"Sound {id} not found.");
            return;
        }

        AudioSource source = GetSource();

        source.transform.position = position;
        source.spatialBlend = 1f;

        SetupSource(source, sound);

        source.Play();

        StartCoroutine(ReturnToPool(source, sound.clip.length));
    }

    // ------------------------
    // MUSIC
    // ------------------------

    public void PlayMusic(string id)
    {
        if (!soundDictionary.TryGetValue(id, out Sound sound))
        {
            Debug.LogWarning($"Music {id} not found.");
            return;
        }

        musicSource.clip = sound.clip;
        musicSource.volume = sound.volume;
        musicSource.loop = sound.loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ------------------------
    // HELPERS
    // ------------------------

    private void SetupSource(AudioSource source, Sound sound)
    {
        source.gameObject.SetActive(true);

        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = Random.Range(sound.pitchMin, sound.pitchMax);
        source.loop = sound.loop;

        Debug.Log($"SFX id: {sound.id}, clip null? {sound.clip == null}");
    }

    private AudioSource GetSource()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        AudioSource source = Instantiate(sfxPrefab, transform);
        return source;
    }

    private System.Collections.IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);

        source.Stop();
        source.gameObject.SetActive(false);

        pool.Enqueue(source);
    }
}