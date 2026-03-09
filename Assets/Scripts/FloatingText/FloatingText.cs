using UnityEngine;
using TMPro;
using UnityEngine.Pool;

public class FloatingText : MonoBehaviour
{
    public float lifeTime = 1f;
    public float moveSpeed = 50f;

    float timer;
    bool isReleased;

    TextMeshProUGUI text;
    Color originalColor;

    IObjectPool<FloatingText> pool;

    public void SetPool(IObjectPool<FloatingText> objectPool)
    {
        pool = objectPool;
    }

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
    }

    public void Activate(string value , Canvas canvas)
    {
        text.transform.SetParent(canvas.transform);
        isReleased = false;

        timer = lifeTime;
        text.text = "+"+value;

        Color c = originalColor;
        c.a = 1f;
        text.color = c;

        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null ) 
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out pos
            );
            rect.localScale = new Vector3(1f, 1f, 1f); 
            rect.localPosition = pos;
        }

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (isReleased) return;

        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        Color c = text.color;
        c.a = timer / lifeTime;
        text.color = c;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isReleased = true;
            pool?.Release(this);
        }
    }
}