using UnityEngine;
using UnityEngine.Events;

public class FloatingTextSpawner : MonoBehaviour
{
    public FloatingTextPool pool;
    public Canvas canvas;

    private GM_LikesScore _gmLikesScore;

    private void Awake()
    {
        _gmLikesScore = GM_LikesScore.instance;
    }

    private void OnMouseDown()
    {
        _gmLikesScore.Click();
        Spawn(_gmLikesScore._clickAmount.ToString());
    }

    void Spawn(string value)
    {
        var text = pool.Get();

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out pos
        );

        text.transform.SetParent(canvas.transform);
        text.GetComponent<RectTransform>().localPosition = pos;

        text.Activate(value);
    }
}