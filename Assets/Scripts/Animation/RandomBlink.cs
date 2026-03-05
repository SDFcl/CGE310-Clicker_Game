using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RandomBlink : MonoBehaviour
{
    [SerializeField] private Image closeEyes;

    [Header("Blink Settings")]
    [SerializeField] private float minBlinkDelay = 2f;
    [SerializeField] private float maxBlinkDelay = 5f;
    [SerializeField] private float blinkDuration = 0.1f;

    private void Start()
    {
        if (closeEyes == null)
            closeEyes = GetComponent<Image>();

        closeEyes.enabled = false; // เริ่มต้นลืมตา

        StartCoroutine(BlinkLoop());
    }

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            float wait = Random.Range(minBlinkDelay, maxBlinkDelay);
            yield return new WaitForSeconds(wait);

            closeEyes.enabled = true; // ปิดตา
            yield return new WaitForSeconds(blinkDuration);

            closeEyes.enabled = false; // ลืมตา
        }
    }
}