using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 主菜单：开始游戏、设置面板开关、退出游戏、音量调节
/// </summary>
public class MainMenu : MonoBehaviour
{
    public GameObject SettingsPanel;

    void Start()
    {
        SettingsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettingsPanel()
    {
        SettingsPanel.SetActive(true);
    }
    public void CloseSettingsPanel()
    {
        SettingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 音量滑块回调，设置全局 AudioListener 音量
    /// </summary>
    public void Setaudio(float value)
    {
        AudioListener.volume = value;
    }

}
