using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System;
using TMPro;

public class BG_ChangeScene : MonoBehaviour
{
    public float duration = 0.5f;

    [Header("Backgroud Scene - ���§����")]
    public List<CanvasGroup> BGScenes = new List<CanvasGroup>();

    [Header("Buy new Scene - ���§����")]
    public List<CanvasGroup> BuyScene = new List<CanvasGroup>();

    [Header("Source - ���§����")]
    public AudioClip[] audioClips;

    [Header("Scene Name")]
    public List<string> SceneNames = new List<string>();
    public CanvasGroup BlackScene;

    public TextMeshProUGUI SceneNameTMP;
    public BuyNewScene _buyNewScene;
    public Button NextButton;
    public Button PreviousButton;

    private int currentSecne;

    private void Start()
    {
        setToScene(0);
    }

    public void ChangeScene(int NumScene)
    {
        StartCoroutine(ChangeSceneRoutine(NumScene));

    }

    public void setToScene(int _scene)
    {
        for (int i = 0; i < BGScenes.Count; i++)
        {
            BGScenes[i].alpha = 0;
            BGScenes[i].blocksRaycasts = false;

            BuyScene[i].alpha = 0;
            BuyScene[i].blocksRaycasts = false;

            StopAudioClips(i);
        }

        currentSecne = _scene;

        BGScenes[currentSecne].alpha = 1;
        BGScenes[currentSecne].blocksRaycasts = true;

        BuyScene[currentSecne].alpha = 1;
        BuyScene[currentSecne].blocksRaycasts = true;

        PlayAudioClips(currentSecne);

        NextButton.gameObject.SetActive(currentSecne < BGScenes.Count - 1);
        PreviousButton.gameObject.SetActive(currentSecne > 0);
    }

    IEnumerator ChangeSceneRoutine(int numScene)
    {
        yield return StartCoroutine(FadeCanvas(0, 1));

        BGScenes[currentSecne].alpha = 0;
        BGScenes[currentSecne].blocksRaycasts = false;
        BuyScene[currentSecne].alpha = 0;
        BuyScene[currentSecne].blocksRaycasts = false;
        StopAudioClips(currentSecne);
        _buyNewScene = BuyScene[currentSecne].GetComponent<BuyNewScene>();
        if (_buyNewScene != null)
        {
            _buyNewScene.Disable();
        }


        currentSecne += numScene;

        BGScenes[currentSecne].alpha = 1;
        BGScenes[currentSecne].blocksRaycasts = true;
        BuyScene[currentSecne].alpha = 1;
        BuyScene[currentSecne].blocksRaycasts = true;
        _buyNewScene = BuyScene[currentSecne].GetComponent<BuyNewScene>();
        if (_buyNewScene != null)
        {
            _buyNewScene.Active();
        }
        PlayAudioClips(currentSecne);

        SceneNameTMP.text = SceneNames[currentSecne];

        Debug.Log(currentSecne + " " + BGScenes.Count);

        NextButton.gameObject.SetActive(currentSecne < BGScenes.Count - 1);
        PreviousButton.gameObject.SetActive(currentSecne > 0);

        yield return StartCoroutine(FadeCanvas(1, 0));
    }

    private void PlayAudioClips(int numClips)
    {
        AudioSource _audioSource = BGScenes[numClips].GetComponent<AudioSource>();
        _audioSource.enabled = true;
        _audioSource.resource = audioClips[numClips];
        _audioSource.Play();
    }

    private void StopAudioClips(int numClips)
    {
        AudioSource _audioSource = BGScenes[numClips].GetComponent<AudioSource>();
        _audioSource.enabled = false;
    }

    IEnumerator FadeCanvas(float start, float end)
    {
        float time = 0;
        BlackScene.blocksRaycasts = true;
        BlackScene.alpha = start;

        while (time < duration)
        {
            time += Time.deltaTime;
            BlackScene.alpha = Mathf.Lerp(start, end, time / duration);
            yield return null;
        }
        BlackScene.blocksRaycasts = false;
        BlackScene.alpha = end;
    }

}
