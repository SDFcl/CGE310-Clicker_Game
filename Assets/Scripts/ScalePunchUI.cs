using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScalePunchUI: MonoBehaviour , IPointerClickHandler
{
    public GameObject ui;
    public float sizeScale = 0.5f;
    public float maxSizeScale = 5.0f;
    public float duration = 0.15f;

    public bool require = false;

    private Vector3 originalScale;


    private void Awake()
    {
        if (ui == null) this.gameObject.SetActive(false);
        originalScale = ui.transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!require)
        {
            StartCoroutine(changeScale());
            return;
        }
    }

    public void Activate()
    {
        StartCoroutine(changeScale());
    }

    private IEnumerator changeScale()
    {
        ui.transform.localScale = originalScale + Vector3.one * sizeScale;

        yield return new WaitForSeconds(duration);

        ui.transform.localScale = originalScale;
    }
}
