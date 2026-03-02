using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySound : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]private AudioClip[] audioClips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartPlaySound()
    {
        if (audioClips.Length == 0) { return; }
        var index = Random.Range(0, audioClips.Length);
        audioSource.PlayOneShot(audioClips[index]);
    }

    public void setAudioCClips(AudioClip[] _audioClips)
    {
        audioClips = _audioClips;
    }

}
