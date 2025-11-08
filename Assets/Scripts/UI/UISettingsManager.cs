using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using DG.Tweening;
using TMPro;
using System.Collections;
using System;

public class UISettingsManager : MonoBehaviour
{

    [Header("Audio")]
    public AudioMixer masterMixer;
    public Slider masterVolumeSlider;
    public TextMeshProUGUI masterAmountText;
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicAmountText;
    public Slider ambientVolumeSlider;
    public TextMeshProUGUI ambientAmountText;
    public Slider sfxVolumeSlider;
    public TextMeshProUGUI sfxAmountText;

    [Header("UI Feedback")]
    public TextMeshProUGUI savedText;

    [Header("UI Settings")]
    [SerializeField] private GameObject settingsPanel;

    private Animator animator;
    private bool isPanelOpen = false;
    private bool isAnimating = false;

    // ปรับให้ตรงกับชื่อใน Animator ของคุณ
    private const string OpenStateName = "Open_Setting_ui_anim";
    private const string CloseStateName = "Close_Setting_ui_anim";

    public static Action OnSettingApplied;

    void Start()
    {
        if (settingsPanel == null)
        {
            Debug.LogError("[UISettingsManager] settingsPanel is not assigned!");
            enabled = false;
            return;
        }

        animator = settingsPanel.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("[UISettingsManager] Animator component not found on settingsPanel!");
            enabled = false;
            return;
        }

        if (savedText != null)
        {
            savedText.alpha = 0;
            savedText.gameObject.SetActive(false);
        }

        // Listener อัปเดตค่าแบบเรียลไทม์
        masterVolumeSlider.onValueChanged.AddListener((v) => UpdateAmountText(masterAmountText, v));
        musicVolumeSlider.onValueChanged.AddListener((v) => UpdateAmountText(musicAmountText, v));
        ambientVolumeSlider.onValueChanged.AddListener((v) => UpdateAmountText(ambientAmountText, v));
        sfxVolumeSlider.onValueChanged.AddListener((v) => UpdateAmountText(sfxAmountText, v));

        LoadSettings();

        // อัปเดตค่าเริ่มต้น
        UpdateAmountText(masterAmountText, masterVolumeSlider.value);
        UpdateAmountText(musicAmountText, musicVolumeSlider.value);
        UpdateAmountText(ambientAmountText, ambientVolumeSlider.value);
        UpdateAmountText(sfxAmountText, sfxVolumeSlider.value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isAnimating)
        {
            ToggleSettingsPanel();
        }
    }

    public void ToggleSettingsPanel()
    {
        if (isAnimating) return;
        isAnimating = true;

        isPanelOpen = !isPanelOpen;

        if (isPanelOpen)
            OpenSettingsPanel();
        else
            CloseSettingsPanel();
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel == null || animator == null)
        {
            isAnimating = false; // ป้องกันการค้าง
            return;
        }

        // เรียกใช้ Coroutine แทน
        StartCoroutine(OpenAnimationCoroutine());
    }

    public void CloseSettingsPanel()
    {
        if (animator == null || settingsPanel == null)
        {
            isAnimating = false; // ป้องกันการค้าง
            return;
        }

        // เรียกใช้ Coroutine แทน
        StartCoroutine(CloseAnimationCoroutine());
    }

    private IEnumerator OpenAnimationCoroutine()
    {
        Debug.Log("Opening Settings Panel...");

        settingsPanel.SetActive(true);
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("Open");

        int layer = 0;
        yield return null;

        Debug.Log("Waiting for Open state...");
        yield return new WaitUntil(() =>
        {
            var cur = animator.GetCurrentAnimatorStateInfo(layer);
            var next = animator.GetNextAnimatorStateInfo(layer);
            return cur.IsName(OpenStateName) || next.IsName(OpenStateName);
        });
        Debug.Log("...Found Open state!");

        Debug.Log("Waiting for transition to end...");
        yield return new WaitWhile(() => animator.IsInTransition(layer));
        Debug.Log("...Transition ended!");

        // --- 🔴 นี่คือส่วนที่แก้ไข 🔴 ---
        Debug.Log("Waiting for animation to finish...");
        // เราจะรอตราบใดที่ State ยังเป็น "Open" และเวลา animation ยังไม่ถึง 1 (ยังเล่นไม่จบ)
        while (animator.GetCurrentAnimatorStateInfo(layer).IsName(OpenStateName) &&
          animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f)
        {
            // รอเฟรมถัดไป
            yield return null;
        }
        Debug.Log("...Animation finished or state changed!");
        // -------------------------------

        isAnimating = false; // ปลดล็อคให้กดปุ่มได้
        isPanelOpen = true;
        Debug.Log("✅ Open animation finished");
    }

    private IEnumerator CloseAnimationCoroutine()
    {
        Debug.Log("Closing Settings Panel...");

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("Close");

        int layer = 0;
        yield return null;

        Debug.Log("Waiting for Close state...");
        yield return new WaitUntil(() =>
        {
            var cur = animator.GetCurrentAnimatorStateInfo(layer);
            var next = animator.GetNextAnimatorStateInfo(layer);
            return cur.IsName(CloseStateName) || next.IsName(CloseStateName);
        });
        Debug.Log("...Found Close state!");

        Debug.Log("Waiting for transition to end...");
        yield return new WaitWhile(() => animator.IsInTransition(layer));
        Debug.Log("...Transition ended!");

        Debug.Log("Waiting for animation to finish...");
        while (animator.GetCurrentAnimatorStateInfo(layer).IsName(CloseStateName) &&
               animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f)
        {
            yield return null;
        }
        Debug.Log("...Animation finished or state changed!");

        settingsPanel.SetActive(false);
        isPanelOpen = false;
        isAnimating = false; // ปลดล็อคให้กดปุ่มได้

        Debug.Log("✅ Close animation finished and panel deactivated");
    }

    public void ApplySettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("AmbientVolume", ambientVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);

        PlayerPrefs.Save();

        Debug.Log("✅ Settings saved via Apply button.");
        ShowSavedMessage();

        OnSettingApplied?.Invoke();
    }

    private void LoadSettings()
    {
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 50f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 50f);
        float ambientVol = PlayerPrefs.GetFloat("AmbientVolume", 50f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 50f);

        masterVolumeSlider.value = masterVol;
        musicVolumeSlider.value = musicVol;
        ambientVolumeSlider.value = ambientVol;
        sfxVolumeSlider.value = sfxVol;

        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetAmbientVolume(ambientVol);
        SetSFXVolume(sfxVol);

        Debug.Log("🔄 Settings loaded from PlayerPrefs.");

        OnSettingApplied?.Invoke();
    }

    public void SetMasterVolume(float volume)
    {
        float db = (volume <= 0) ? -80f : Mathf.Log10(volume / 100f) * 20;
        masterMixer.SetFloat("MasterVolume", db);
    }

    public void SetMusicVolume(float volume)
    {
        float db = (volume <= 0) ? -80f : Mathf.Log10(volume / 100f) * 20;
        masterMixer.SetFloat("MusicVolume", db);
    }

    public void SetAmbientVolume(float volume)
    {
        float db = (volume <= 0) ? -80f : Mathf.Log10(volume / 100f) * 20;
        masterMixer.SetFloat("AmbientVolume", db);
    }

    public void SetSFXVolume(float volume)
    {
        float db = (volume <= 0) ? -80f : Mathf.Log10(volume / 100f) * 20;
        masterMixer.SetFloat("SFXVolume", db);
    }

    public void SetDefaults()
    {
        masterVolumeSlider.value = 50;
        musicVolumeSlider.value = 50;
        ambientVolumeSlider.value = 50;
        sfxVolumeSlider.value = 50;

        Debug.Log("🔁 Settings reset to default values.");
        ApplySettings();
    }

    private void ShowSavedMessage()
    {
        if (savedText == null) return;

        savedText.gameObject.SetActive(true);
        savedText.DOKill();

        Color c = savedText.color;
        c.a = 0;
        savedText.color = c;

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(DOTween.To(() => savedText.color.a, x =>
        {
            Color col = savedText.color;
            col.a = x;
            savedText.color = col;
        }, 1f, 0.3f))
           .AppendInterval(1.2f)
           .Append(DOTween.To(() => savedText.color.a, x =>
           {
               Color col = savedText.color;
               col.a = x;
               savedText.color = col;
           }, 0f, 0.8f))
           .OnComplete(() => savedText.gameObject.SetActive(false));
    }

    private void UpdateAmountText(TextMeshProUGUI textElement, float value)
    {
        if (textElement == null) return;

        textElement.text = Mathf.RoundToInt(value).ToString();
    }

    private void OnDestroy()
    {
        OnSettingApplied = null;
    }

}
