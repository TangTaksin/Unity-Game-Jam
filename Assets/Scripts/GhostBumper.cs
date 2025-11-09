using UnityEngine;
using System.Collections;

public class GhostBumper : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;      // Sprite ตอนปกติ (อมตะ)
    public Sprite vulnerableSprite;  // Sprite ตอน Bonus Time (สีน้ำเงิน)

    [Header("Stats")]
    public int maxHitPoints = 3;     // จำนวนครั้งที่ต้องชน
    public int scoreValue = 10000;   // คะแนนที่ได้ตอนทำลาย
    public float downTime = 7f;


    [Header("Feedback (Optional)")]
    public AudioClip hitSound;
    public AudioClip destroySound;

    private SpriteRenderer spriteRenderer;
    private int currentHitPoints;
    Collider2D _collider2D;
    SimpleBumper _bumper;
    private Coroutine downTimeCoroutine;


    enum GhostState
    {
        normal,
        vulnerable,
        down
    }
    GhostState cur_state = GhostState.normal;

    private Coroutine flashCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        _bumper = GetComponent<SimpleBumper>();
        // เริ่มเกมมาให้ Reset Ghost 1 ครั้ง
        SetState(GhostState.normal);
    }

    private void OnEnable()
    {
        BonusTimeManager.OnBonusStart += EnterBonus;
        BonusTimeManager.OnBonusEnd += ExitBonus;
    }

    private void OnDisable()
    {
        BonusTimeManager.OnBonusStart -= EnterBonus;
        BonusTimeManager.OnBonusEnd -= ExitBonus;
    }

    // --- ฟังก์ชัน "สาธารณะ" ที่ BonusTimeManager จะเรียก ---

    void EnterBonus()
    {
        SetState(GhostState.vulnerable);
    }

    void ExitBonus()
    {
        if (downTimeCoroutine != null)
        {
            StopCoroutine(downTimeCoroutine);
            downTimeCoroutine = null;
        }

        SetState(GhostState.normal);
    }

    // BonusTimeManager เรียกฟังก์ชันนี้ ตอน "เริ่ม" Bonus Time
    void SetState(GhostState state)
    {
        cur_state = state;

        switch (cur_state)
        {
            case GhostState.normal:
                spriteRenderer.enabled = true;
                _collider2D.isTrigger = false;

                spriteRenderer.sprite = normalSprite;
                break;
            case GhostState.vulnerable:
                _collider2D.isTrigger = true;
                spriteRenderer.enabled = true;

                spriteRenderer.sprite = vulnerableSprite;
                currentHitPoints = maxHitPoints; // รีเซ็ตเลือด
                break;
            case GhostState.down:
                spriteRenderer.enabled = false;
                _collider2D.isTrigger = true;

                if (downTimeCoroutine != null)
                    StopCoroutine(downTimeCoroutine);

                downTimeCoroutine = StartCoroutine(DownTimeProcess());
                break;
        }
    }

    // --- Logic การชน ---

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. ถ้า "ไม่" อยู่ในโหมดโบนัส -> ไม่ต้องทำอะไรเลย (อมตะ)
        if (cur_state != GhostState.vulnerable) return;

        // 2. ถ้าอยู่ในโหมดโบนัส และชนกับ "Ball"
        if (collision.gameObject.CompareTag("Ball"))
        {
            // 3. ลดเลือด
            currentHitPoints--;

            // (Optional) กระพริบตัวตอนโดนชน
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashHit());

            // 4. ตรวจสอบว่าตายหรือยัง
            if (currentHitPoints <= 0)
            {
                HandleDestruction();
            }
            else
            {
                // ถ้ายังไม่ตาย ก็เล่นแค่เสียง "ชน"
                if (AudioManager.instance != null && hitSound != null)
                {
                    AudioManager.instance.PlaySFXRandomPitch(hitSound);
                }
            }
        }
    }

    // เมื่อถูกทำลาย
    private void HandleDestruction()
    {
        // 1. (Optional) เล่นเสียง "ตาย"
        if (AudioManager.instance != null && destroySound != null)
        {
            AudioManager.instance.PlaySFX(destroySound);
        }

        // 2. (Optional) เพิ่มคะแนน
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreValue);
        }


        // 4. (Optional) บอก Manager ว่า "ฉันตายแล้ว" (สำหรับนับ Jackpot)
        if (BonusTimeManager.instance != null)
        {
            // BonusTimeManager.instance.NotifyGhostDestroyed();
        }

        // 5. (ตามที่คุณขอ!) "ซ่อน" ตัวเอง
        SetState(GhostState.down);
    }

    IEnumerator DownTimeProcess()
    {
        yield return new WaitForSeconds(downTime);
        SetState(GhostState.vulnerable);
    }

    // (Optional) Coroutine กระพริบตัว
    private IEnumerator FlashHit()
    {
        spriteRenderer.color = Color.red; // กระพริบสีแดง
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white; // กลับสีเดิม
    }
}
