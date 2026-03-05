using UnityEngine;

public class DogAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Time When Dont Click to Trigger Animation")]
    [SerializeField] private float itemBeforeTriggerSusAnim = 5f;

    private float timer;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= itemBeforeTriggerSusAnim)
        {
            animator.SetTrigger("SusTrigger");
            timer = 0f;
        }
    }

    public void OnClick()
    {
        timer = 0f;
    }
}
