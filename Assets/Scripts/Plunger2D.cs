using UnityEngine;
using UnityEngine.UI;

public class Plunger2D : MonoBehaviour
{
    [Header("Plunger Settings")]
    public KeyCode launchKey = KeyCode.Space;
    public float maxChargeForce = 100f;
    public float chargeRate = 30f;
    public Vector2 launchDirection = Vector2.up;

    [Header("References")]
    public Slider chargeSlider;
    public GameObject initialBall;
    public Transform respawnPoint; 

    private Rigidbody2D currentBall;
    private float currentCharge;
    private bool isBallReady;

    void Start()
    {
        // (ส่วน Start เหมือนเดิม ไม่ต้องแก้)
        if (chargeSlider != null)
        {
            chargeSlider.maxValue = maxChargeForce;
            chargeSlider.value = 0;
            chargeSlider.gameObject.SetActive(false);
        }

        if (initialBall != null)
        {
            if (respawnPoint == null)
            {
                Debug.LogError("Plunger: ยังไม่ได้ตั้งค่า Respawn Point!");
                return;
            }
            initialBall.transform.position = respawnPoint.position;
            Rigidbody2D ballRb = initialBall.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector2.zero;
                ballRb.angularVelocity = 0f;
            }
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
            // (ส่วน GetKey เหมือนเดิม)
            if (Input.GetKey(launchKey))
            {
                currentCharge = Mathf.Min(currentCharge + chargeRate * Time.deltaTime, maxChargeForce);
                if (chargeSlider != null) chargeSlider.value = currentCharge;
            }

            // --- 🌟 แก้ไข Logic ตรงนี้ ---
            
            // เมื่อผู้เล่น "ปล่อย" ปุ่มยิง
            if (Input.GetKeyUp(launchKey))
            {
                // 1. ยิงลูกบอลด้วยพลังที่มี (แม้จะน้อยก็ตาม)
                if (currentBall != null)
                {
                    currentBall.AddForce(launchDirection.normalized * currentCharge, ForceMode2D.Impulse);
                }

                // 2. ตรวจสอบว่าพลังชาร์จ "มากพอ" ที่จะยิงหรือไม่
                // (เช่น มากกว่า 1f หรือ 5% ของ maxChargeForce)
                float minChargeToLaunch = 1f; // ตั้งค่าขั้นต่ำ

                if (currentCharge > minChargeToLaunch)
                {
                    // 3. ถ้ายิงสำเร็จ (พลังมากพอ) -> "ปิดการทำงาน" Plunger
                    isBallReady = false; 
                    
                    if (chargeSlider != null)
                    {
                        chargeSlider.gameObject.SetActive(false);
                    }
                }
                
                // 4. รีเซ็ตพลังชาร์จ (ไม่ว่ายิงสำเร็จหรือไม่)
                //    เพื่อให้การ "แตะ" ครั้งต่อไปเริ่มชาร์จใหม่
                currentCharge = 0f;
                if (chargeSlider != null)
                {
                    chargeSlider.value = 0;
                }
            }
            // --- 🌟 จบส่วนที่แก้ไข ---
        }
    }

    // (ส่วน SetBallReady เหมือนเดิม)
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