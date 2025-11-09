using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;

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

    // ตัวแปรติดตามสถานะ
    private int currentPelletCount;
    private int currentHitCount;

    Pellet[] pelletinScene;
    

    private bool isBonusTimeActive = false;

    float bonusTimer;

    public static Action OnBonusStart;
    public static Action OnBonusEnd;



    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameManager.OnGameStart += Initialize;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= Initialize;
    }

    void Start()
    {
        // (Optional) หา Ghost Bumpers ทั้งหมดในฉาก (ถ้าคุณจะทำระบบทำลาย Ghost)
        // ghostBumpers = FindObjectsOfType<GhostBumper>();

        Initialize();
    }

    private void Update()
    {
        BonusTimerProcess();
    }

    void Initialize()
    {
        pelletinScene = FindObjectsByType<Pellet>(FindObjectsSortMode.None);

        EndBonusTime();
    }

    // รีเซ็ตทุกอย่างตอนเริ่มด่าน (หรือตอน Bonus Time จบ)
    void EndBonusTime()
    {
        OnBonusEnd?.Invoke();

        // 1. นับเม็ดทั้งหมด (Pellets)
        //currentPelletCount = FindObjectsOfType<Pellet>().Length;
        foreach(var pel in pelletinScene)
        {
            pel.gameObject.SetActive(true);
        }

        currentPelletCount = pelletinScene.Length;

        // 2. รีเซ็ตตัวนับการชน
        currentHitCount = 0;

        isBonusTimeActive = false;

        GameManager.instance?.SetMultiplier(1f);
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
            StartBonusTime();
        }

        if (pelletsCleared && !isBonusTimeActive)
        {
            Debug.Log("All pellets eaten!");
            StartBonusTime();
        }
    }

    [Button("Trigger Bonus")]
    void StartBonusTime()
    {
        isBonusTimeActive = true;
        OnBonusStart?.Invoke();

        bonusTimer = bonusTimeDuration;

        GameManager.instance?.SetMultiplier(scoreMultiplier);
    }

    void BonusTimerProcess()
    {
        if (!isBonusTimeActive)
            return;

        bonusTimer -= Time.deltaTime;

        // end and reset bonus time
        if (bonusTimer <= 0)
            EndBonusTime();
    }

    public (float, float) GetTimers()
    {
        return (bonusTimer, bonusTimeDuration);
    }
}
