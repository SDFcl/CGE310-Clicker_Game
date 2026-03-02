using UnityEngine;
using System.Collections.Generic;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }

    [SerializeField] private GM_LikesScore likesManager;           // ลาก GM_LikesScore มาวาง
    [SerializeField] private List<SlotUpgradeScript> upgradeSlots;  // ลากทุก Slot ที่อยากเซฟมาวางทั้งหมด

    private const string KEY_LIKES = "Likes_Total";
    private const string KEY_CLICK_AMOUNT = "Click_Amount";
    private const string KEY_AUTO_ACTIVE = "AutoClick_Active";
    private const string KEY_AUTO_AMOUNT = "AutoClick_Amount";
    private const string KEY_LIKE_PER_SEC = "Like_PerSec";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        var found = FindObjectsOfType<SlotUpgradeScript>(true);

        foreach (var slot in found)
        {
            if (!upgradeSlots.Contains(slot))
            {
                upgradeSlots.Add(slot);
            }
        }

        // โหลดข้อมูลทันทีที่เริ่มเกม
        LoadAllData();
    }

    // =====================================================================
    //                          SAVE / LOAD ทั้งหมด
    // =====================================================================

    public void SaveAllData()
    {
        if (likesManager == null) return;

        // 1. เซฟข้อมูลหลักของ GM_LikesScore
        PlayerPrefs.SetInt(KEY_LIKES, likesManager.getScoreLikes());
        PlayerPrefs.SetInt(KEY_CLICK_AMOUNT, likesManager._clickAmount);
        PlayerPrefs.SetInt(KEY_AUTO_ACTIVE, likesManager._autoClickActive ? 1 : 0);
        PlayerPrefs.SetInt(KEY_AUTO_AMOUNT, likesManager._autoClickAmount);
        PlayerPrefs.SetFloat(KEY_LIKE_PER_SEC, likesManager._likePerSec);

        // 2. เซฟทุก upgrade slot
        for (int i = 0; i < upgradeSlots.Count; i++)
        {
            var slot = upgradeSlots[i];
            if (slot == null) continue;

            string slotKey = $"Slot_{i:00}";  // Slot_00, Slot_01, Slot_02, ...

            PlayerPrefs.SetInt(slotKey + "_Level", slot.level);
            // ถ้าอยากเซฟราคาปัจจุบันด้วย (optional)
            // PlayerPrefs.SetInt(slotKey + "_Price", slot.price);
        }

        PlayerPrefs.Save();
        Debug.Log($"[Save] เซฟเรียบร้อย → Likes = {likesManager.getScoreLikes()} | Slots = {upgradeSlots.Count}");
    }

    public void LoadAllData()
    {
        if (likesManager == null) return;

        // 1. โหลดข้อมูลหลัก
        if (PlayerPrefs.HasKey(KEY_LIKES))
        {
            int loadedLikes = PlayerPrefs.GetInt(KEY_LIKES, 0);
            likesManager.setScoreLikes(loadedLikes);
            likesManager.OnLikesUpdated?.Invoke(loadedLikes);

            likesManager._clickAmount = PlayerPrefs.GetInt(KEY_CLICK_AMOUNT, 1);
            likesManager._autoClickActive = PlayerPrefs.GetInt(KEY_AUTO_ACTIVE, 0) == 1;
            likesManager._autoClickAmount = PlayerPrefs.GetInt(KEY_AUTO_AMOUNT, 1);
            likesManager._likePerSec = PlayerPrefs.GetFloat(KEY_LIKE_PER_SEC, 1f);

            Debug.Log($"[Load] โหลดข้อมูลหลัก → Likes = {loadedLikes}");
        }

        // 2. โหลดทุก slot
        for (int i = 0; i < upgradeSlots.Count; i++)
        {
            var slot = upgradeSlots[i];
            if (slot == null) continue;

            string slotKey = $"Slot_{i:00}";

            if (PlayerPrefs.HasKey(slotKey + "_Level"))
            {
                int loadedLevel = PlayerPrefs.GetInt(slotKey + "_Level", 0);
                slot.level = loadedLevel;

                // คำนวณราคาใหม่ตาม level ที่โหลดมา
                double calcPrice = slot.PriceAddLater + (slot.basePrice * Mathf.Pow(slot.growthRate, loadedLevel));
                slot.price = Mathf.RoundToInt((float)calcPrice);  // หรือ Convert.ToInt32 ก็ได้

                // อัพเดท UI
                if (slot.AmountUpgrade != null)
                    slot.AmountUpgrade.text = loadedLevel.ToString();

                if (slot.PriceTMP != null)
                    slot.PriceTMP.text = slot.price.ToString();

                Debug.Log($"[Load] Slot {i} → Level = {loadedLevel}, Price = {slot.price}");
            }
        }
        GM_LikesScore.instance.likeUpdatedInvoke();
    }

    public void StartNewGame()
    {
        // 1. ลบข้อมูลเซฟทั้งหมด
        PlayerPrefs.DeleteAll();

        // 2. รีเซ็ตค่าทั้งหมดใน memory ทันที (สำคัญ!)
        if (likesManager != null)
        {
            likesManager.setScoreLikes(0);
            likesManager._clickAmount = 1;
            likesManager.OnLikesUpdated?.Invoke(0);
            likesManager._autoClickActive = false;
            likesManager._autoClickAmount = 1;
            likesManager._likePerSec = 1f;
            likesManager.UpdateLikesText(); // อัพเดท UI
        }

        foreach (var slot in upgradeSlots)
        {
            if (slot == null) continue;
            slot.level = 0;
            slot.price = slot.basePrice; // หรือคำนวณใหม่ตามสูตรเริ่มต้น
            if (slot.AmountUpgrade != null) slot.AmountUpgrade.text = "0";
            if (slot.PriceTMP != null) slot.PriceTMP.text = slot.GetPrice().ToString();
        }

        // 3. เซฟสถานะใหม่ (optional แต่แนะนำ)
        SaveAllData();

        Debug.Log("เริ่มเกมใหม่เรียบร้อย → ทุกอย่างรีเซ็ตเป็น 0");

        // ถ้ามี UI เริ่มเกมใหม่ หรืออยากรีโหลดฉาก → ทำตรงนี้
        // เช่น: SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // =====================================================================
    //               ฟังก์ชันช่วยเหลือ (เรียกจากที่อื่นได้)
    // =====================================================================

    public void SaveNow()
    {
        SaveAllData();
    }

    // เรียกใช้ตอนออกเกม / พักเกม
    private void OnApplicationQuit()
    {
        SaveAllData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveAllData();
    }

    // ถ้าอยากเซฟอัตโนมัติทุก X วินาที (เช่น ทุก 20–30 วินาที)
    // เปิดใช้งานโดย uncomment บรรทัดด้านล่าง
    
    private void Start()
    {
        InvokeRepeating(nameof(SaveAllData), 30f, 30f);
    }
   

    // สำหรับ debug / ปุ่มรีเซ็ตใน editor หรือในเกม
    [ContextMenu("Delete All Save Data")]
    public void DeleteAllSaveData()
    {
        PlayerPrefs.DeleteAll();
        Debug.LogWarning("ลบข้อมูลเซฟทั้งหมดเรียบร้อย → เริ่มเกมใหม่");
        // ถ้าอยากให้รีโหลดทันทีก็เรียก LoadAllData() หรือ Application.Quit() / Scene reload ก็ได้
    }
}