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

    [Header("Feedback (Optional)")]
    public AudioClip hitSound;
    public AudioClip destroySound;

    private SpriteRenderer spriteRenderer;
    private int currentHitPoints;
    private bool isVulnerable = false;
    private Coroutine flashCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // เริ่มเกมมาให้ Reset Ghost 1 ครั้ง
        ResetGhost();
    }

    // --- ฟังก์ชัน "สาธารณะ" ที่ BonusTimeManager จะเรียก ---

    // BonusTimeManager เรียกฟังก์ชันนี้ ตอน "เริ่ม" Bonus Time
    public void SetVulnerable(bool vulnerable)
    {
        isVulnerable = vulnerable;
        if (isVulnerable)
        {
            spriteRenderer.sprite = vulnerableSprite;
            currentHitPoints = maxHitPoints; // รีเซ็ตเลือด
        }
        else
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    // BonusTimeManager เรียกฟังก์ชันนี้ ตอน "จบ" Bonus Time
    public void ResetGhost()
    {
        gameObject.SetActive(true); // "เกิดใหม่" (ถ้าถูก SetActive(false) ไป)
        SetVulnerable(false);       // กลับร่างปกติ
    }

    // --- Logic การชน ---

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. ถ้า "ไม่" อยู่ในโหมดโบนัส -> ไม่ต้องทำอะไรเลย (อมตะ)
        if (!isVulnerable) return;

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
        gameObject.SetActive(false);
    }

    // (Optional) Coroutine กระพริบตัว
    private IEnumerator FlashHit()
    {
        spriteRenderer.color = Color.red; // กระพริบสีแดง
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white; // กลับสีเดิม
    }
}
