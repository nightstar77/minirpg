using UnityEngine;

/// <summary>
/// 全局音频管理器（单例）
/// 统一管理 BGM 和音效，避免每个脚本各自创建 AudioSource。
///
/// 使用方式：
/// 1. 在场景里建一个空物体 "AudioManager"，挂上此脚本
/// 2. Inspector 里拖入各个 AudioClip（BGM、攻击、升级、格挡）
/// 3. 其他脚本通过 AudioManager.Instance.PlayXXX() 播放
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    [Tooltip("背景音乐，启动后循环播放")]
    [SerializeField] private AudioClip bgmClip;

    [Tooltip("BGM 音量（0~1）")]
    [SerializeField] [Range(0f, 1f)] private float bgmVolume = 0.5f;

    [Header("音效")]
    [Tooltip("玩家攻击音效")]
    [SerializeField] private AudioClip attackClip;

    [Tooltip("升级音效")]
    [SerializeField] private AudioClip levelUpClip;

    [Tooltip("格挡成功音效（怪物伤害命中玩家防御时）")]
    [SerializeField] private AudioClip blockClip;

    [Tooltip("音效音量（0~1）")]
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        // 单例：场景切换时保留
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 创建两个 AudioSource：一个放 BGM，一个放音效
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    private void Start()
    {
        PlayBGM();
    }

    #region ===== Public 播放接口 =====

    /// <summary>
    /// 播放 BGM（循环）
    /// </summary>
    public void PlayBGM()
    {
        if (bgmClip == null || bgmSource == null) return;
        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }

    /// <summary>
    /// 停止 BGM
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    /// <summary>
    /// 玩家攻击音效
    /// </summary>
    public void PlayAttack()
    {
        PlaySfx(attackClip);
    }

    /// <summary>
    /// 升级音效
    /// </summary>
    public void PlayLevelUp()
    {
        PlaySfx(levelUpClip);
    }

    /// <summary>
    /// 格挡成功音效
    /// </summary>
    public void PlayBlock()
    {
        PlaySfx(blockClip);
    }

    #endregion

    /// <summary>
    /// 播放一次性音效
    /// </summary>
    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
