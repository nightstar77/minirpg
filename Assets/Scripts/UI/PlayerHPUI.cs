using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHPUI : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;
    public TMP_Text hpText;

    [Header("Player")]
    public PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth.OnPlayerDead += Hide;
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        float current = playerHealth.CurrentHP;
        float max = playerHealth.maxHP;
        slider.value = current / max;
        hpText.text = $"{current}/{max}";
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDead -= Hide;
        }
    }

}