using UnityEngine;

[System.Serializable]
public class Sound
{
    public string id;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    public bool loop = false;

    [Header("Pitch Randomization")]
    public float pitchMin = 1f;
    public float pitchMax = 1f;
}