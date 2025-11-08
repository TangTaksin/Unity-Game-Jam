using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LauncherTrigger : MonoBehaviour
{
    [Header("Launcher Settings")]
    public float maxChargeForce = 100f;
    public float chargeRate = 30f;
    public Vector2 launchDirection = Vector2.up;

    [Header("References")]
    public Slider chargeSlider;

    private Rigidbody2D currentBall;
    private float currentCharge;
    private bool isBallReady;
    private bool isCharging;

    // Input System action
    private InputAction launchAction;

    void Awake()
    {
        launchAction = InputSystem.actions.FindAction("LaunchBall");
    }

    void OnEnable()
    {
        launchAction.Enable();
        launchAction.started += OnLaunchStarted;
        launchAction.canceled += OnLaunchCanceled;
    }

    void OnDisable()
    {
        launchAction.started -= OnLaunchStarted;
        launchAction.canceled -= OnLaunchCanceled;
        launchAction.Disable();
    }

    void Start()
    {
        if (chargeSlider != null)
        {
            chargeSlider.maxValue = maxChargeForce;
            chargeSlider.value = 0;
            chargeSlider.gameObject.SetActive(false);
        }

        isBallReady = false;
        currentBall = null;
        isCharging = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ball") && currentBall != null && other.gameObject == currentBall.gameObject)
        {
            Debug.Log("Ball has been launched!");
            isBallReady = false;
            currentBall = null;
            currentCharge = 0f;
            isCharging = false;

            if (chargeSlider != null)
            {
                chargeSlider.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (isBallReady && currentBall != null && isCharging)
        {
            currentCharge = Mathf.Min(currentCharge + chargeRate * Time.deltaTime, maxChargeForce);
            if (chargeSlider != null)
                chargeSlider.value = currentCharge;
        }
    }

    private void OnLaunchStarted(InputAction.CallbackContext context)
    {
        if (isBallReady)
        {
            isCharging = true;
            currentCharge = 0f;
            Debug.Log("Charging...");
        }
    }

    private void OnLaunchCanceled(InputAction.CallbackContext context)
    {
        if (isBallReady && currentBall != null)
        {
            currentBall.AddForce(launchDirection.normalized * currentCharge, ForceMode2D.Impulse);
            Debug.Log($"Ball launched with force: {currentCharge}");

            currentCharge = 0f;
            isCharging = false;
            if (chargeSlider != null)
                chargeSlider.value = 0;
        }
    }
}
