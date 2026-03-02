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

        if (audioSource == null)
        {
            this.enabled = true;
        }
    }

    public void StartPlaySound()
    {
        var index = Random.Range(0, audioClips.Length);
        audioSource.PlayOneShot(audioClips[index]);
    }

    public void setAudioCClips(AudioClip[] _audioClips)
    {
        audioClips = _audioClips;
    }

}
