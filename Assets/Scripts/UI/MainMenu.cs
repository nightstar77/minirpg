using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void Setaudio(float value)
    {
        AudioListener.volume = value;
    }

}
