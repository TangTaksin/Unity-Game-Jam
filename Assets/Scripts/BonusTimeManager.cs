using System.Collections;
using UnityEngine;

public class BonusTimeManager : MonoBehaviour
{
    // Singleton (เพื่อให้สคริปต์อื่นเรียกง่าย)
    public static BonusTimeManager instance;

    [Header("Bonus Conditions")]
    public int totalPelletsInLevel; // (จะถูกตั้งค่าอัตโนมัติ)
    public int hitsRequiredForBonus = 5; // จำนวนครั้งที่ "เป้าหมาย" ต้องถูกชน

    [Header("Bonus State")]
    public float bonusTimeDuration = 30f; // 30 วินาที
    public float scoreMultiplier = 2.5f;

    // (Optional) UI ที่จะโชว์
    public GameObject bonusTimeUI;

    // ตัวแปรติดตามสถานะ
    private int currentPelletCount;
    private int currentHitCount;
    private bool isBonusTimeActive = false;



    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // (Optional) หา Ghost Bumpers ทั้งหมดในฉาก (ถ้าคุณจะทำระบบทำลาย Ghost)
        // ghostBumpers = FindObjectsOfType<GhostBumper>();

        InitializeBonusTime();
    }

    // รีเซ็ตทุกอย่างตอนเริ่มด่าน (หรือตอน Bonus Time จบ)
    void InitializeBonusTime()
    {
        // 1. นับเม็ดทั้งหมด (Pellets)
        //currentPelletCount = FindObjectsOfType<Pellet>().Length;
        currentPelletCount = FindObjectsByType<Pellet>(FindObjectsSortMode.None).Length;
        totalPelletsInLevel = currentPelletCount;

        // 2. รีเซ็ตตัวนับการชน
        currentHitCount = 0;

        // 3. รีเซ็ตสถานะ
        isBonusTimeActive = false;

        // 4. (Optional) บอก ScoreManager ให้ใช้ตัวคูณปกติ
        if (GameManager.instance != null)
            GameManager.instance.SetMultiplier(1f);

        // 5. (Optional) ซ่อน UI โบนัส
        if (bonusTimeUI != null)
            bonusTimeUI.SetActive(false);

        Debug.Log("Level Initialized. Pellets: " + totalPelletsInLevel + ", Hits Required: " + hitsRequiredForBonus);
    }

    // --- ฟังก์ชันที่ "Pellet.cs" จะเรียก ---
    public void NotifyPelletEaten()
    {
        if (isBonusTimeActive) return; // ไม่นับเม็ดตอนโบนัส

        currentPelletCount--;
        CheckForBonusTime();
    }

    // --- ฟังก์ชันที่ "HitTarget.cs" จะเรียก ---
    public void NotifyTargetHit()
    {
        if (isBonusTimeActive) return; // ไม่นับการชนตอนโบนัส

        if (currentHitCount < hitsRequiredForBonus)
        {
            currentHitCount++;
            Debug.Log("Hit Target! Progress: " + currentHitCount + "/" + hitsRequiredForBonus);
        }
        CheckForBonusTime();
    }

    // --- ตรวจสอบเงื่อนไข ---
    private void CheckForBonusTime()
    {
        // 1. ตรวจสอบว่าเงื่อนไขครบ 2 อย่างหรือไม่
        bool pelletsCleared = (currentPelletCount <= 0);
        bool targetsHit = (currentHitCount >= hitsRequiredForBonus);

        // 2. ถ้าครบ และยังไม่ได้อยู่ในโบนัส
        if (targetsHit && !isBonusTimeActive)
        {
            // 3. เริ่ม Bonus Time!
            StartCoroutine(BonusTimeSequence());
        }

        if (pelletsCleared && !isBonusTimeActive)
        {
            Debug.Log("All pellets eaten!");
            StartCoroutine(BonusTimeSequence());
        }
    }

    // --- Coroutine สำหรับ Bonus Time ---
    private IEnumerator BonusTimeSequence()
    {
        Debug.Log("--- BONUS TIME STARTED! ---");
        isBonusTimeActive = true;

        // 1. (Optional) โชว์ UI
        if (bonusTimeUI != null)
            bonusTimeUI.SetActive(true);

        // 2. (Optional) บอก ScoreManager ให้คูณ 2.5
        if (GameManager.instance != null)
            GameManager.instance.SetMultiplier(scoreMultiplier);

        // 3. (Optional) บอก Ghost Bumpers ให้ "อ่อนแอ"
        // foreach (GhostBumper ghost in ghostBumpers)
        // {
        //     ghost.SetVulnerable(true);
        // }

        // 4. รอตามเวลาที่กำหนด
        yield return new WaitForSeconds(bonusTimeDuration);

        // 5. เมื่อเวลาหมด
        Debug.Log("--- BONUS TIME ENDED! ---");

        // 6. (Optional) คำนวณแจ็คพอตใหญ่ (ถ้าทำลาย Ghost หมด)
        // ... (Logic ตรวจสอบ Ghost) ...

        // 7. รีเซ็ตด่าน (Level Reset)
        InitializeBonusTime();
    }
}
