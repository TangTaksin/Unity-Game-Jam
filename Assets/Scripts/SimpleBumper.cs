using UnityEngine;
using System.Collections;

public class SimpleBumper : MonoBehaviour
{
 public float bounceForce = 15f; // แรงถีบ (2D ใช้ค่าน้อยกว่า 3D)
    public int scoreValue = 100;

    public Renderer bumperRenderer; // ลาก Sprite Renderer มาใส่
    public Color hitColor = Color.yellow;
    private Color originalColor;
    public AudioClip hitSound;

    void Start()
    {
        if (bumperRenderer == null)
            bumperRenderer = GetComponent<Renderer>();
        
        // สำหรับ Sprite Renderer เราต้องใช้ material.color
        originalColor = bumperRenderer.material.color; 
    }

    // สำคัญมาก: ต้องเป็น OnCollisionEnter2D
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // 1. เพิ่มคะแนน
            // ScoreManager.instance.AddScore(scoreValue);
            Debug.Log("Hit! Score: " + scoreValue);
            // เล่นเสียง
            if (hitSound != null && AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(hitSound);
            }

            // 2. ถีบลูกบอลออกไป!
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                // หาทิศทาง (เป็น Vector2)
                Vector2 bounceDirection = (collision.transform.position - transform.position).normalized;

                // ถีบลูกบอล (ใช้ ForceMode2D.Impulse)
                ballRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
            }

            // 3. กระพริบไฟ
            StartCoroutine(FlashColor());
        }
    }
    
    private IEnumerator FlashColor()
    {
        bumperRenderer.material.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        bumperRenderer.material.color = originalColor;
    }
}
