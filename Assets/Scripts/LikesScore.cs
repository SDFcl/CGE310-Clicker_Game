using TMPro;
using UnityEngine;

public class LikesScore : MonoBehaviour
{
    public static LikesScore instance;

    [HideInInspector] public float _likeCount = 0f;
    [SerializeField] private TextMeshProUGUI LikesScoreText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }

    public void addLikes(float like)
    {
        _likeCount += like;
        LikesScoreText.text = "Like : " + _likeCount.ToString();
    }
}
