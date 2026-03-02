using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;
using System.Collections;

public class BG_ChangeScene : MonoBehaviour
{
    public float duration = 0.5f;

    [Header("Backgroud Scene - เรียงด้วย")]
    public List<CanvasGroup> BGScenes = new List<CanvasGroup>();
    public CanvasGroup BlackScene;

    public Button NextButton;
    public Button PreviousButton;

    private int currentSecne;

    private void Awake()
    {
        PreviousButton.gameObject.SetActive(false);
    }

    public void ChangeScene(int NumScene)
    {
        StartCoroutine(ChangeSceneRoutine(NumScene));
        NextButton.gameObject.SetActive(currentSecne+2 > BGScenes.Count);
        PreviousButton.gameObject.SetActive(currentSecne+1 < BGScenes.Count);
    }

    IEnumerator ChangeSceneRoutine(int numScene)
    {
        yield return StartCoroutine(FadeCanvas(0, 1));

        BGScenes[currentSecne].alpha = 0;
        BGScenes[currentSecne].blocksRaycasts = false;

        currentSecne += numScene;

        BGScenes[currentSecne].alpha = 1;
        BGScenes[currentSecne].blocksRaycasts = true;

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
        BlackScene.blocksRaycasts = false ;
        BlackScene.alpha = end;
    }

}
