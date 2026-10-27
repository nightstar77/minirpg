using UnityEngine;
using MiniRPG.Core;

/// <summary>
/// 全局音频管理器（单例）
/// 统一管理 BGM 和音效，避免每个脚本各自创建 AudioSource。
/// 音量设置持久化到 GameManager.SaveData。
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

    private const string BgmVolumeKey = "BGMVolume";
    private const string SfxVolumeKey = "SFXVolume";

    /// <summary>
    /// 当前 BGM 音量（0~1）
    /// </summary>
    public float BgmVolume => bgmVolume;

    /// <summary>
    /// 当前音效音量（0~1）
    /// </summary>
    public float SfxVolume => sfxVolume;

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
        // 在 Start 中读取存档音量（确保 GameManager.Awake 已先执行完毕）
        var save = GameManager.Instance?.SaveData;
        if (save != null)
        {
            bgmVolume = Mathf.Clamp01(save.bgmVolume);
            sfxVolume = Mathf.Clamp01(save.sfxVolume);
            if (bgmSource != null) bgmSource.volume = bgmVolume;
            if (sfxSource != null) sfxSource.volume = sfxVolume;
        }

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

    #region ===== 音量调节（设置面板用） =====

    /// <summary>
    /// 设置 BGM 音量（0~1），实时生效并持久化
    /// </summary>
    public void SetBgmVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
        var save = GameManager.Instance?.SaveData;
        if (save != null)
        {
            save.bgmVolume = bgmVolume;
            GameManager.Instance.Save();
        }
    }

    /// <summary>
    /// 设置音效音量（0~1），实时生效并持久化
    /// </summary>
    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        var save = GameManager.Instance?.SaveData;
        if (save != null)
        {
            save.sfxVolume = sfxVolume;
            GameManager.Instance.Save();
        }
    }

    #endregion

    /// <summary>
    /// 播放一次性音效（音量由 sfxSource.volume 统一控制）
    /// </summary>
    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
