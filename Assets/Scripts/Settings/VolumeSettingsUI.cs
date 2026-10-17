using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 音量设置页：两个滑条分别调节 BGM 与音效音量，实时生效并持久化。
///
/// Unity 搭建：
/// - VolumeSettingsUI 挂在音量设置页上
/// - BgmSlider / SfxSlider 拖入两个 Slider（maxValue 保持默认 1）
/// - AudioManager 上的音量值会在面板打开时同步到滑条
/// </summary>
public class VolumeSettingsUI : MonoBehaviour
{
    [Header("滑条")]
    [Tooltip("BGM 音量滑条")]
    [SerializeField] private Slider bgmSlider;

    [Tooltip("音效音量滑条")]
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if (bgmSlider != null) { bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged); }
        if (sfxSlider != null) { sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged); }
    }

    /// <summary>
    /// 页面显示时把 AudioManager 当前音量同步到滑条
    /// </summary>
    private void OnEnable()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }
        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(AudioManager.Instance.BgmVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SfxVolume);
        }
    }

    private void OnBgmVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBgmVolume(value);
        }
    }

    private void OnSfxVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSfxVolume(value);
        }
    }
}
