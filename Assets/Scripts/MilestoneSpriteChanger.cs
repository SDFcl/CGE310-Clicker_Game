using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class SpriteMilestone
{
    public int requiredLikes;          // จำนวนไลค์ที่ต้องถึงเพื่อปลดล็อก milestone นี้
    public Sprite sprite;              // Sprite ที่จะเปลี่ยนเป็น
    public AudioClip[] audioClips;     // เสียงที่จะเล่นแบบสุ่มเมื่อเพิ่งถึง milestone นี้
}

public class MilestoneSpriteChanger : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Image targetImage;
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    [Header("Sound")]
    [SerializeField] private PlaySound playSound;                  // ลาก PlaySound component มาวางที่นี่
    [SerializeField] private AudioClip[] initialAudioClips;        // เสียงสำหรับตอน likes ยังไม่ถึง milestone แรก (หรือตอนรีเซ็ตกลับมา)

    [Header("Settings")]
    [SerializeField] private Sprite initialSprite;                 // Sprite เริ่มต้น
    [SerializeField] private SpriteMilestone[] milestones;         // เรียงจากน้อยไปมาก

    private int currentMilestoneIndex = 0;

    private void Start()
    {
        // หา target อัตโนมัติถ้ายังไม่ได้ลาก
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetSpriteRenderer == null) targetSpriteRenderer = GetComponent<SpriteRenderer>();

        if (targetImage == null && targetSpriteRenderer == null)
        {
            Debug.LogError("MilestoneSpriteChanger: ต้องมี Image หรือ SpriteRenderer หรือลากมาที่ field");
            return;
        }

        // หา initialSprite อัตโนมัติ
        if (initialSprite == null)
        {
            if (targetImage != null) initialSprite = targetImage.sprite;
            else if (targetSpriteRenderer != null) initialSprite = targetSpriteRenderer.sprite;
        }

        // เรียง milestones ตาม requiredLikes
        if (milestones != null && milestones.Length > 0)
        {
            System.Array.Sort(milestones, (a, b) => a.requiredLikes.CompareTo(b.requiredLikes));
        }

        // เชื่อม event
        if (GM_LikesScore.instance != null)
        {
            GM_LikesScore.instance.OnLikesUpdated += OnLikesChanged;
        }
        else
        {
            Debug.LogWarning("ไม่พบ GM_LikesScore.instance");
        }

        // อัพเดทสถานะปัจจุบัน (สำหรับตอน load เกม)
        if (GM_LikesScore.instance != null)
        {
            OnLikesChanged(GM_LikesScore.instance.getScoreLikes());
        }
    }

    private void OnDestroy()
    {
        if (GM_LikesScore.instance != null)
        {
            GM_LikesScore.instance.OnLikesUpdated -= OnLikesChanged;
        }
    }

    private void OnLikesChanged(int currentLikes)
    {
        // คำนวณ milestone index ใหม่
        int newIndex = 0;
        for (int i = 0; i < milestones.Length; i++)
        {
            if (currentLikes >= milestones[i].requiredLikes)
            {
                newIndex = i + 1;
            }
            else
            {
                break;
            }
        }

        // 1. เพิ่งถึง milestone ใหม่ (index เพิ่มขึ้น)
        if (newIndex > currentMilestoneIndex && playSound != null)
        {
            int justReachedIndex = newIndex - 1;

            if (justReachedIndex < milestones.Length &&
                milestones[justReachedIndex].audioClips != null &&
                milestones[justReachedIndex].audioClips.Length > 0)
            {
                playSound.setAudioCClips(milestones[justReachedIndex].audioClips);
                //playSound.StartPlaySound();
                Debug.Log($"Milestone reached → Played sound | Likes: {currentLikes} | Index: {justReachedIndex}");
            }
        }

        // 2. เพิ่งกลับมาที่ initial state (จาก milestone ใด ๆ ลงมาเหลือ 0)
        else if (newIndex == 0 && currentMilestoneIndex > 0 && playSound != null)
        {
            if (initialAudioClips != null && initialAudioClips.Length > 0)
            {
                playSound.setAudioCClips(initialAudioClips);
                playSound.StartPlaySound();
                Debug.Log($"กลับมาที่ initial → Played initial sound | Likes: {currentLikes}");
            }
        }

        // อัพเดท index ปัจจุบัน
        currentMilestoneIndex = newIndex;

        // เลือก Sprite ตาม index
        Sprite newSprite = initialSprite;

        if (currentMilestoneIndex > 0)
        {
            int spriteIndex = currentMilestoneIndex - 1;
            newSprite = (spriteIndex < milestones.Length)
                ? milestones[spriteIndex].sprite
                : milestones[milestones.Length - 1].sprite;
        }

        SetTargetSprite(newSprite);

        Debug.Log($"Sprite updated | Likes: {currentLikes} | Milestone index: {currentMilestoneIndex}");
    }

    private void SetTargetSprite(Sprite sprite)
    {
        if (targetImage != null)
            targetImage.sprite = sprite;

        if (targetSpriteRenderer != null)
            targetSpriteRenderer.sprite = sprite;
    }
}