using UnityEngine;
using UnityEngine.Events;

public class OnClicker : MonoBehaviour
{
    public UnityEvent OnClick;

    private void OnMouseDown()
    {
        OnClick.Invoke();
    }
}
