using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public FloatingTextPool pool;
    public Canvas canvas;

    private float _likeCount = 1f;

    private void OnMouseDown()
    {
        LikesScore.instance.addLikes(_likeCount);
        Spawn(_likeCount.ToString());
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