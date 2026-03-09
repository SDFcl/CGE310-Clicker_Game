using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class GM_LikesScore : MonoBehaviour
{
    public static GM_LikesScore instance;

    [HideInInspector] public int _likeCount = 0;
    [SerializeField] private TextMeshProUGUI LikesScoreText;

    public System.Action<int> OnLikesUpdated;  //Observer Pattern

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        AutoLike();
    }

    public void likeUpdatedInvoke()
    {
        OnLikesUpdated?.Invoke(_likeCount); 
    }

    public void UpdateLikesText()
    {
<<<<<<< Updated upstream
        LikesScoreText.text = "Like : " + _likeCount.ToString();
        OnLikesUpdated?.Invoke(_likeCount);
=======
        LikesScoreText.text = NumberFormatter.Format(_likeCount);
        OnLikesUpdated?.Invoke(_likeCount); //notify
>>>>>>> Stashed changes
    }
    private void UpdateLikeScore(int Amount)
    {
        _likeCount += Amount;
        UpdateLikesText();
    }

    public int getScoreLikes()
    {
        return _likeCount;
    }

    public void setScoreLikes(int likeCount)
    {
        _likeCount = likeCount;
        UpdateLikesText();
    }

    #region Click

    public int _clickAmount = 1;
    public void addClickAmount(int amount) 
    {
        _clickAmount += amount;
    }

    public void Click()
    {
        UpdateLikeScore(_clickAmount);
    }

    #endregion


    #region Auto-Click
    public bool _autoClickActive = false;

    private float likeBuffer;

    public int _autoClickAmount = 1;
    public int AutoClickAmount
    {
        get { return _autoClickAmount; }
        set { _autoClickAmount = value; }
    }

    public float _likePerSec = 1.0f;
    public float LikePerSec
    {
        get { return _likePerSec; }
        set { _likePerSec = value; }
    }

    public void addAutoClickAmount(int amount)
    {
        if (!_autoClickActive) _autoClickActive = true;

        _autoClickAmount += amount;
    }

    public void addAutoClickLikePerSec(float amount)
    {
        _likePerSec += amount;

        if (_likePerSec > 0)
            _autoClickActive = true;
    }

    void AutoLike()
    {
        if (!_autoClickActive) return;

        likeBuffer += _autoClickAmount * _likePerSec * Time.deltaTime;

        if (likeBuffer >= 1f)
        {
            int amountToAdd = Mathf.FloorToInt(likeBuffer);
            likeBuffer -= amountToAdd;

            UpdateLikeScore(amountToAdd);
        }
    }
    #endregion
}
