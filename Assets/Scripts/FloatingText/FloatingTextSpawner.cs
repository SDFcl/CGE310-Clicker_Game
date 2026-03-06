using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FloatingTextSpawner : MonoBehaviour , IPointerClickHandler
{
    public FloatingTextPool pool;
    public Canvas canvas;

    public void OnPointerClick(PointerEventData eventData)
    {
        GM_LikesScore.instance.Click();
        Spawn();
    }

    public void Spawn()
    {
        var text = pool.Get();
        string _clickAmount = NumberFormatter.Format(GM_LikesScore.instance._clickAmount);
        text.Activate(_clickAmount.ToString(), canvas);
    }
}