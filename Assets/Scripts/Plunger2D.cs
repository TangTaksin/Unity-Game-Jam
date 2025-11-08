using UnityEngine;
using UnityEngine.UI; // สำหรับ Slider (Optional)

public class Plunger2D : MonoBehaviour
{
    [Header("Plunger Settings")]
    public KeyCode launchKey = KeyCode.Space;
    public float maxChargeForce = 100f;
    public float chargeRate = 30f;
    public Vector2 launchDirection = Vector2.up;

    [Header("References")]
    // (Optional) ลาก Slider UI มาใส่
    public Slider chargeSlider;
    
    // ลาก "ลูกบอล" ตัวแรกใน Scene มาใส่
    public GameObject initialBall;
    
    // ลาก "จุดเกิด" (Empty GameObject) มาใส่
    public Transform respawnPoint; 

    private Rigidbody2D currentBall;
    private float currentCharge;
    private bool isBallReady;

    void Start()
    {
        // ตั้งค่า Slider (Optional)
        if (chargeSlider != null)
        {
            chargeSlider.maxValue = maxChargeForce;
            chargeSlider.value = 0;
            chargeSlider.gameObject.SetActive(false);
        }

        // ตรวจสอบลูกบอลตัวแรก
        if (initialBall != null)
        {
            if (respawnPoint == null)
            {
                Debug.LogError("Plunger: ยังไม่ได้ตั้งค่า Respawn Point!");
                return;
            }

            // ย้ายลูกบอลตัวแรกไปที่จุดเกิด
            initialBall.transform.position = respawnPoint.position;

            // เคลียร์ความเร็วเก่า (ถ้ามี)
            Rigidbody2D ballRb = initialBall.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector2.zero;
                ballRb.angularVelocity = 0f;
            }
            
            // สั่งให้ Plunger พร้อมทำงาน
            SetBallReady(initialBall);
        }
        else
        {
            Debug.LogWarning("Plunger: ยังไม่ได้ตั้งค่า Initial Ball!");
        }
    }

    void Update()
    {
        if (isBallReady)
        {
            // กดค้างเพื่อชาร์จ
            if (Input.GetKey(launchKey))
            {
                currentCharge = Mathf.Min(currentCharge + chargeRate * Time.deltaTime, maxChargeForce);
                if (chargeSlider != null) chargeSlider.value = currentCharge;
            }

            // ปล่อยปุ่มเพื่อยิง
            if (Input.GetKeyUp(launchKey))
            {
                if (currentBall != null)
                {
                    currentBall.AddForce(launchDirection.normalized * currentCharge, ForceMode2D.Impulse);
                }
                
                // รีเซ็ตสถานะ
                isBallReady = false;
                currentCharge = 0f;
                if (chargeSlider != null)
                {
                    chargeSlider.value = 0;
                    chargeSlider.gameObject.SetActive(false);
                }
            }
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกโดย DrainAndRespawn
    public void SetBallReady(GameObject ball)
    {
        currentBall = ball.GetComponent<Rigidbody2D>();

        if (currentBall != null)
        {
            isBallReady = true;
            currentCharge = 0f; 

            if (chargeSlider != null)
            {
                chargeSlider.gameObject.SetActive(true);
            }
        }
    }
}