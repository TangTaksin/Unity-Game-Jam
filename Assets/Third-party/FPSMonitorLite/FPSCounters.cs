using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class FPSCounters : MonoBehaviour
{
    // Singleton instance
    public static FPSCounters Instance { get; private set; }

    [Header("Display Settings")]
    [SerializeField] private bool showFPS = true;
    [SerializeField] private TextAnchor alignment = TextAnchor.UpperLeft;
    [SerializeField, Range(10, 50)] private int fontSize = 24;
    [SerializeField] private Vector2 padding = new Vector2(10, 10);
    [SerializeField] private Color goodFPSColor = Color.green;
    [SerializeField] private Color warningFPSColor = Color.yellow;
    [SerializeField] private Color badFPSColor = Color.red;

    [Header("Calculation Settings")]
    [SerializeField, Range(0.1f, 1f)] private float smoothingFactor = 0.1f;
    [SerializeField, Range(10, 60)] private int averageFrameCount = 30;

#if ENABLE_INPUT_SYSTEM
    [SerializeField] private Key toggleKey = Key.F3;
#endif

    // Circular buffer for better performance
    private float[] deltaTimes;
    private int bufferIndex = 0;
    private int filledCount = 0;
    
    private float smoothedDeltaTime = 0f;
    private GUIStyle style;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize circular buffer
        deltaTimes = new float[averageFrameCount];
    }

    private void Start()
    {
        // Initialize GUI style after Unity's GUI system is ready
        InitializeStyle();
    }

    private void InitializeStyle()
    {
        style = new GUIStyle
        {
            alignment = alignment,
            fontSize = fontSize,
            normal = { textColor = goodFPSColor }
        };
    }

    private void Update()
    {
        // Check for toggle key (supports both input systems)
        bool keyPressed = false;
        
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            keyPressed = Keyboard.current[toggleKey].wasPressedThisFrame;
        }
#else
        keyPressed = Input.GetKeyDown(toggleKey);
#endif

        if (keyPressed)
        {
            showFPS = !showFPS;
        }

        if (!showFPS) return;

        // Smooth delta time
        smoothedDeltaTime += (Time.unscaledDeltaTime - smoothedDeltaTime) * smoothingFactor;

        // Add to circular buffer
        deltaTimes[bufferIndex] = smoothedDeltaTime;
        bufferIndex = (bufferIndex + 1) % averageFrameCount;
        
        if (filledCount < averageFrameCount)
            filledCount++;
    }

    private void OnGUI()
    {
        if (!showFPS || filledCount == 0) return;

        // Ensure style is initialized
        if (style == null)
            InitializeStyle();

        style.fontSize = fontSize;
        style.alignment = alignment;

        // Calculate statistics
        float avgDelta = GetAverageDeltaTime();
        float minDelta = float.MaxValue;
        float maxDelta = float.MinValue;

        for (int i = 0; i < filledCount; i++)
        {
            float dt = deltaTimes[i];
            if (dt < minDelta) minDelta = dt;
            if (dt > maxDelta) maxDelta = dt;
        }

        float avgFps = 1f / avgDelta;
        float minFps = 1f / maxDelta;  // Max delta = min FPS
        float maxFps = 1f / minDelta;  // Min delta = max FPS
        float ms = avgDelta * 1000f;
        float memoryMB = (float)System.GC.GetTotalMemory(false) / (1024 * 1024);

        // Update text color based on FPS
        style.normal.textColor = GetColorForFPS(avgFps);

        // Draw the label
        Rect rect = GetLabelRect(Screen.width, Screen.height);
        string text = $"FPS: {avgFps:0.}\n" +
                      $"Min: {minFps:0.} | Max: {maxFps:0.}\n" +
                      $"Frame Time: {ms:0.0} ms\n" +
                      $"Memory: {memoryMB:0.0} MB";
        
        GUI.Label(rect, text, style);
    }

    private float GetAverageDeltaTime()
    {
        float sum = 0f;
        for (int i = 0; i < filledCount; i++)
        {
            sum += deltaTimes[i];
        }
        return sum / filledCount;
    }

    private Color GetColorForFPS(float fps) => fps switch
    {
        > 60f => goodFPSColor,
        > 30f => warningFPSColor,
        _     => badFPSColor
    };

    private Rect GetLabelRect(int screenWidth, int screenHeight)
    {
        float height = fontSize * 4.5f; // Adjusted for line spacing
        float width = 250f;
        
        return alignment switch
        {
            TextAnchor.UpperLeft  => new Rect(padding.x, padding.y, width, height),
            TextAnchor.UpperRight => new Rect(screenWidth - padding.x - width, padding.y, width, height),
            TextAnchor.LowerLeft  => new Rect(padding.x, screenHeight - padding.y - height, width, height),
            TextAnchor.LowerRight => new Rect(screenWidth - padding.x - width, screenHeight - padding.y - height, width, height),
            _                     => new Rect(padding.x, padding.y, width, height)
        };
    }

    // Public method to toggle FPS display
    public void ToggleFPSDisplay(bool show) => showFPS = show;
    
    // Optional: Method to change alignment at runtime
    public void SetAlignment(TextAnchor newAlignment)
    {
        alignment = newAlignment;
        if (style != null)
            style.alignment = newAlignment;
    }
}