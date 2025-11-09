using System.Collections;
using UnityEngine;

public class HitTarget : MonoBehaviour
{
    public int scoreValue = 250; 

    [Header("Feedback (Optional)")]
    public AudioClip hitSound;
    public Color hitColor = Color.cyan; 

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    // private Coroutine flashCoroutine; // เราไม่ใช้ Coroutine แล้ว

    // --- 1. (เพิ่ม) ตัวแปร "สถานะ" ---
    private bool isHit = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.material.color;
        }
        // spriteRenderer.material.color = originalColor; // (ย้ายไปไว้ใน ResetTarget)
        ResetTarget(); // สั่งรีเซ็ต 1 ครั้งตอนเริ่ม
    }

    // --- 2. (เพิ่ม) ฟังก์ชันสำหรับ "รีเซ็ต" ---
    // BonusTimeManager จะเรียกฟังก์ชันนี้ ตอนที่ InitializeLevel()
    public void ResetTarget()
    {
        isHit = false; // "เปิด" ให้โดนชนได้อีกครั้ง
        if (spriteRenderer != null)
        {
            spriteRenderer.material.color = originalColor; // คืนสีเดิม
        }
    }


    // --- (ส่วน Collision/Trigger เหมือนเดิม) ---
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            HandleHit();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            HandleHit();
        }
    }
    
    private void HandleHit()
    {
        // --- 3. (สำคัญ!) "เช็ค" ก่อนทำงาน ---
        // ถ้า "เคย" โดนชนไปแล้ว (isHit = true) -> "ไม่ต้องทำอะไรเลย"
        if (isHit) return; 
        
        // 4. ถ้าเป็น "ครั้งแรก" -> "ตั้งค่า" ว่าโดนชนแล้วทันที
        isHit = true; 

        // 5. รัน Logic ที่เหลือ (บวกคะแนน, เล่นเสียง ฯลฯ)
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreValue);
        }

        if (AudioManager.instance != null && hitSound != null)
        {
            AudioManager.instance.PlaySFXRandomPitch(hitSound);
        }

        if (BonusTimeManager.instance != null)
        {
            BonusTimeManager.instance.NotifyTargetHit();
        }

        // 6. เปลี่ยนสี (ถาวร จนกว่าจะ Reset)
        if (spriteRenderer != null)
        {
            spriteRenderer.material.color = hitColor;
        }
        
        // (เราลบ Coroutine ออก เพราะเราต้องการให้สี "ค้าง" ไว้)
    }

    // (Coroutine ไม่จำเป็นแล้ว ลบออกได้)
    // private IEnumerator FlashColor() { ... }
}