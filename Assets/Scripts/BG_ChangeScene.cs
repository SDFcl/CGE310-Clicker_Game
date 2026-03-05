using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System;
using TMPro;

public class BG_ChangeScene : MonoBehaviour
{
    public float duration = 0.5f;

    [Header("Backgroud Scene - เรียงด้วย")]
    public List<CanvasGroup> BGScenes = new List<CanvasGroup>();

    [Header("Buy new Scene - เรียงด้วย")]
    public List<CanvasGroup> BuyScene = new List<CanvasGroup>();

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
        }

        currentSecne = _scene;

        BGScenes[currentSecne].alpha = 1;
        BGScenes[currentSecne].blocksRaycasts = true;

        BuyScene[currentSecne].alpha = 1;
        BuyScene[currentSecne].blocksRaycasts = true;

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

        SceneNameTMP.text = SceneNames[currentSecne];

        Debug.Log(currentSecne + " " + BGScenes.Count);

        NextButton.gameObject.SetActive(currentSecne < BGScenes.Count - 1);
        PreviousButton.gameObject.SetActive(currentSecne > 0);

        yield return StartCoroutine(FadeCanvas(1, 0));
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
