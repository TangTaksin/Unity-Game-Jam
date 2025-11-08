using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;

    [Header("SFX Object Pool")]
    public int initialPoolSize = 15;

    public AudioSource sfxPrefab;

    private List<AudioSource> sfxPool;

    void Awake()
    {
        // --- ตั้งค่า Singleton ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // --- จบ Singleton ---

        sfxPool = new List<AudioSource>();

        if (sfxPrefab == null)
        {
            GameObject temp = new GameObject("_SFX_Prefab");
            temp.transform.parent = transform;
            sfxPrefab = temp.AddComponent<AudioSource>();
            sfxPrefab.playOnAwake = false;
        }


        // 3. สร้าง AudioSource "เตรียมไว้" ล่วงหน้า
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePooledSource();
        }
    }

    private AudioSource CreatePooledSource()
    {
        // โคลน AudioSource ใหม่
        AudioSource newSource = Instantiate(sfxPrefab, transform);
        newSource.gameObject.SetActive(true); // (เผื่อ Prefab ถูกปิดไว้)
        sfxPool.Add(newSource);
        return newSource;
    }

    private AudioSource GetAvailableSource()
    {
        // 1. วนหาใน Pool ที่เรามี
        for (int i = 0; i < sfxPool.Count; i++)
        {
            // 2. ถ้าเจอตัวที่ "ไม่ได้เล่น"
            if (!sfxPool[i].isPlaying)
            {
                // 3. ส่งตัวนั้นกลับไป
                return sfxPool[i];
            }
        }

        Debug.LogWarning("AudioManager: Pool is full. Creating new AudioSource.");
        return CreatePooledSource();
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource == null)
        {
            Debug.LogError("AudioManager: MusicSource is not assigned!");
            return;
        }

        musicSource.clip = musicClip;
        musicSource.Play();
    }


    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null) return;

        AudioSource source = GetAvailableSource();

        source.clip = sfxClip;
        source.volume = 1f; // ตั้งค่าเริ่มต้น
        source.pitch = 1f;  // ตั้งค่าเริ่มต้น

        source.Play();
    }

    public void PlaySFX(AudioClip sfxClip, float volume = 1f, float pitch = 1f)
    {
        if (sfxClip == null) return;

        AudioSource source = GetAvailableSource();

        source.clip = sfxClip;
        source.volume = volume;
        source.pitch = pitch;

        source.Play();
    }

    // ฟังก์ชันสุ่ม Pitch ที่คุณเคยมี
    public void PlaySFXRandomPitch(AudioClip sfxClip)
    {
        float randomPitch = Random.Range(0.9f, 1.1f);
        PlaySFX(sfxClip, 1f, randomPitch);
    }
}
