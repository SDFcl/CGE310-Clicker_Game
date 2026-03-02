using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class SpriteMilestone
{
    public int requiredLikes;  // เช่น 500, 1000, 5000
    public Sprite sprite;      // Sprite ที่จะเปลี่ยนเป็น
}

public class MilestoneSpriteChanger : MonoBehaviour
{
    [Header("Target (ลาก Image หรือ SpriteRenderer มา")]
    [SerializeField] private Image targetImage;
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    [Header("Settings")]
    [SerializeField] private Sprite initialSprite;  // Sprite เริ่มต้น (ก่อนถึง milestone แรก)
    [SerializeField] private SpriteMilestone[] milestones;  // List milestone (เรียงน้อย → มาก)

    private int currentMilestoneIndex = 0;  // Track ว่าถึง milestone ไหนแล้ว

    private void Start()
    {
        // Auto หา target ถ้ายังไม่ลาก
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetSpriteRenderer == null) targetSpriteRenderer = GetComponent<SpriteRenderer>();
        if (targetImage == null && targetSpriteRenderer == null)
        {
            Debug.LogError("MilestoneSpriteChanger: ต้องลาก Image หรือ SpriteRenderer มาด้วย!");
            return;
        }

        // Auto หา initialSprite ถ้ายังไม่ตั้ง
        if (initialSprite == null)
        {
            if (targetImage != null) initialSprite = targetImage.sprite;
            else if (targetSpriteRenderer != null) initialSprite = targetSpriteRenderer.sprite;
        }

        // เรียง milestones อัตโนมัติ (เผื่อลืมเรียง)
        if (milestones != null && milestones.Length > 0)
        {
            System.Array.Sort(milestones, (a, b) => a.requiredLikes.CompareTo(b.requiredLikes));
        }

        // เชื่อม event
        if (GM_LikesScore.instance != null)
        {
            GM_LikesScore.instance.OnLikesUpdated += OnLikesChanged;
        }

        // เช็คสถานะปัจจุบันทันที (สำหรับ LoadGame)
        OnLikesChanged(GM_LikesScore.instance.getScoreLikes());
    }

    private void OnDestroy()
    {
        // ลบ event เพื่อไม่ leak memory
        if (GM_LikesScore.instance != null)
        {
            GM_LikesScore.instance.OnLikesUpdated -= OnLikesChanged;
        }
    }

    private void OnLikesChanged(int currentLikes)
    {
        // ⭐ แก้ใหม่: คำนวณ index ใหม่ทุกครั้ง (handle เพิ่ม/ลด/รีเซ็ต)
        int newIndex = 0;
        for (int i = 0; i < milestones.Length; i++)
        {
            if (currentLikes >= milestones[i].requiredLikes)
            {
                newIndex = i + 1;  // ผ่าน milestone นี้แล้ว
            }
            else
            {
                break;  // หยุดเมื่อไม่ถึง
            }
        }
        currentMilestoneIndex = newIndex;

        // เปลี่ยน Sprite
        Sprite newSprite;
        if (currentMilestoneIndex == 0)
        {
            newSprite = initialSprite;  // ยังไม่ถึง milestone แรก
        }
        else
        {
            int spriteIndex = currentMilestoneIndex - 1;
            newSprite = (spriteIndex < milestones.Length) ?
                        milestones[spriteIndex].sprite :
                        milestones[milestones.Length - 1].sprite;
        }

        SetTargetSprite(newSprite);
        Debug.Log($"Sprite Update! Likes={currentLikes} → Index={currentMilestoneIndex}");
    }

    private void SetTargetSprite(Sprite sprite)
    {
        if (targetImage != null)
            targetImage.sprite = sprite;
        else if (targetSpriteRenderer != null)
            targetSpriteRenderer.sprite = sprite;
    }
}