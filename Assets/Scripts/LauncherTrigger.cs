using UnityEngine;
using UnityEngine.UI;

public class LauncherTrigger : MonoBehaviour
{
    [Header("Launcher Settings")]
    public KeyCode launchKey = KeyCode.Space;
    public float maxChargeForce = 100f;
    public float chargeRate = 30f;
    public Vector2 launchDirection = Vector2.up;

    [Header("References")]
    public Slider chargeSlider; // (Optional)

    private Rigidbody2D currentBall; // ลูกบอลที่ "กำลังจะยิง"
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
        isBallReady = false;
        currentBall = null;
    }

    // --- นี่คือ Logic ใหม่ ---

    // เมื่อ "ลูกบอล" เข้ามาในพื้นที่ยิง
    void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าเป็นลูกบอล และยังไม่มียิงลูกอื่นค้างไว้
        if (other.CompareTag("Ball") && !isBallReady)
        {
            Debug.Log("Ball is ready to launch!");
            currentBall = other.GetComponent<Rigidbody2D>();
            isBallReady = true;
            currentCharge = 0f;

            if (chargeSlider != null)
            {
                chargeSlider.value = 0;
                chargeSlider.gameObject.SetActive(true);
            }
        }
    }

    // เมื่อ "ลูกบอล" ถูกยิงออกไปจากพื้นที่
    void OnTriggerExit2D(Collider2D other)
    {
        // ตรวจสอบว่าใช่ลูกบอลที่เราเพิ่งยิงไปหรือไม่
        if (other.CompareTag("Ball") && other.gameObject == currentBall.gameObject)
        {
            Debug.Log("Ball has been launched!");
            isBallReady = false;
            currentBall = null;
            currentCharge = 0f;

            if (chargeSlider != null)
            {
                chargeSlider.gameObject.SetActive(false);
            }
        }
    }

    // --- จบ Logic ใหม่ ---

    void Update()
    {
        // Logic การชาร์จและยิง (เหมือนเดิม แต่มีการแก้ Bug)
        if (isBallReady && currentBall != null)
        {
            if (Input.GetKey(launchKey))
            {
                currentCharge = Mathf.Min(currentCharge + chargeRate * Time.deltaTime, maxChargeForce);
                if (chargeSlider != null) chargeSlider.value = currentCharge;
            }

            if (Input.GetKeyUp(launchKey))
            {
                // ยิงลูกบอลด้วยพลังที่มี
                currentBall.AddForce(launchDirection.normalized * currentCharge, ForceMode2D.Impulse);

                // *** แก้ Bug: เราจะ "ไม่" ตั้ง isBallReady = false ที่นี่ ***
                // เราจะปล่อยให้ OnTriggerExit2D เป็นคนจัดการเมื่อลูกบอลออกจากพื้นที่ไปแล้ว

                // เรารีเซ็ตแค่พลังชาร์จ
                currentCharge = 0f;
                if (chargeSlider != null)
                {
                    chargeSlider.value = 0;
                }
            }
        }
    }
}
