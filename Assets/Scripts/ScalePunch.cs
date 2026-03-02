using System.Collections;
using UnityEngine;

public class ScalePunch: MonoBehaviour
{
    public GameObject go;
    public float sizeScale = 0.5f;
    public float maxSizeScale = 5.0f;
    public float duration = 0.15f;

    private void Awake()
    {
        if (go == null) this.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (go.transform.localScale.x == (sizeScale * maxSizeScale))
        {
            return;
        }
        StartCoroutine(changeScale());
    }

    private IEnumerator changeScale()
    {
        go.transform.localScale = go.transform.localScale + Vector3.one * sizeScale;

        yield return new WaitForSeconds(duration);

        go.transform.localScale = go.transform.localScale - Vector3.one * sizeScale;
    }
}
