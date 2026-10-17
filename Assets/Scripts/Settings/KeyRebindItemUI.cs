using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 单条改键设置行：显示动作名称与当前按键，点击按钮后按任意键完成改键。
///
/// Unity 搭建（每行一个物体）：
/// ─ KeyRebindItem           ← 挂本脚本
///    ├─ ActionName (TMP)    ← 动作名，如"攻击"（留空则自动用 Action 枚举名）
///    ├─ KeyText (TMP)       ← 当前按键显示
///    └─ RebindButton        ← "更改"按钮
/// </summary>
public class KeyRebindItemUI : MonoBehaviour
{
    [Header("绑定动作")]
    [Tooltip("本行要修改的动作")]
    [SerializeField] private GameInputs.Action action;

    [Header("UI")]
    [SerializeField] private TMP_Text actionNameText;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private Button rebindButton;

    /// <summary>
    /// 当前正在捕获按键的行（全局只有一行在改键）
    /// </summary>
    private static KeyRebindItemUI capturingItem;

    private void Awake()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.AddListener(StartCapture);
        }
        // 动作名留空时自动填充枚举名
        if (actionNameText != null && string.IsNullOrEmpty(actionNameText.text))
        {
            actionNameText.text = GetActionDisplayName(action);
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        if (capturingItem == this)
        {
            capturingItem = null;
            GameInputs.EndCapture();
        }
    }

    private void Update()
    {
        if (capturingItem != this)
        {
            return;
        }

        // 任意键按下即捕获（只支持键盘，鼠标点击忽略，避免误绑鼠标键）
        if (Input.anyKeyDown)
        {
            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(code))
                {
                    continue;
                }

                // 鼠标按键不绑定
                if (code >= KeyCode.Mouse0 && code <= KeyCode.Mouse6)
                {
                    continue;
                }

                if (code == KeyCode.Escape)
                {
                    // Esc 取消改键
                    CancelCapture();
                    return;
                }

                // 排除 Mouse 枚举后的 KeyCode 中仍有 None
                if (code == KeyCode.None)
                {
                    continue;
                }

                GameInputs.SetKey(action, code);
                FinishCapture();
                return;
            }
        }
    }

    #region ===== 捕获流程 =====

    public void StartCapture()
    {
        // 取消其他行的捕获
        if (capturingItem != null && capturingItem != this)
        {
            capturingItem.CancelCapture();
        }
        capturingItem = this;
        GameInputs.BeginCapture();

        if (keyText != null)
        {
            keyText.text = "按任意键...";
        }
    }

    private void CancelCapture()
    {
        FinishCapture();
    }

    private void FinishCapture()
    {
        capturingItem = null;
        GameInputs.EndCapture();
        Refresh();
    }

    #endregion

    /// <summary>
    /// 刷新当前按键显示
    /// </summary>
    public void Refresh()
    {
        if (keyText != null)
        {
            keyText.text = GameInputs.GetKeyName(action);
        }
    }

    /// <summary>
    /// 动作的中文显示名
    /// </summary>
    public static string GetActionDisplayName(GameInputs.Action action)
    {
        switch (action)
        {
            case GameInputs.Action.Attack: return "攻击";
            case GameInputs.Action.CancelAttack: return "取消攻击";
            case GameInputs.Action.Defend: return "防御";
            case GameInputs.Action.Skill0: return "技能 1";
            case GameInputs.Action.Skill1: return "技能 2";
            case GameInputs.Action.Skill2: return "技能 3";
            case GameInputs.Action.Skill3: return "技能 4";
            default: return action.ToString();
        }
    }
}
