using System.IO;
using UnityEngine;

namespace MiniRPG.Core
{
    /// <summary>
    /// 全局游戏管理器（单例，跨场景存活）。
    /// 职责：
    /// 1. 统一存档读写（SaveData → persistentDataPath/save.json）
    /// 2. 游戏状态管理（进行中 / 暂停），控制 Time.timeScale
    /// 3. 从旧版 PlayerPrefs 存档迁移到新存档
    ///
    /// 使用方式：在 StartMenu 场景建一个空物体挂此脚本。
    /// 若场景中未放置，会通过 Instance 懒加载自动创建。
    /// 各业务系统通过 GameManager.Instance.SaveData 读写进度。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
                return instance;
            }
            private set => instance = value;
        }

        /// <summary>当前存档数据，所有系统读写此对象</summary>
        public SaveData SaveData { get; private set; }

        /// <summary>游戏状态</summary>
        public enum GameState { Playing, Paused }
        public GameState State { get; private set; } = GameState.Playing;

        private string savePath;

        private void Awake()
        {
            // 单例
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, "save.json");
            Load();
        }

        #region ===== 存档 =====

        /// <summary>
        /// 读取存档。若存档文件不存在则创建空存档，并尝试从旧版 PlayerPrefs 迁移数据。
        /// </summary>
        public void Load()
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                SaveData = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            }
            else
            {
                SaveData = new SaveData();
                MigrateFromPlayerPrefs();
            }
        }

        /// <summary>
        /// 把 SaveData 序列化为 JSON 写入磁盘。
        /// 各系统修改完 SaveData 字段后调用此方法持久化。
        /// </summary>
        public void Save()
        {
            if (SaveData == null) SaveData = new SaveData();
            string json = JsonUtility.ToJson(SaveData, true);
            File.WriteAllText(savePath, json);
        }

        /// <summary>删除存档（回到初始状态）</summary>
        public void DeleteSave()
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            SaveData = new SaveData();
        }

        /// <summary>
        /// 从旧版 PlayerPrefs 迁移数据到新存档（仅在新存档首次创建时执行一次）。
        /// 迁移后不删除旧 PlayerPrefs，作为兜底。
        /// </summary>
        private void MigrateFromPlayerPrefs()
        {
            // 技能
            if (PlayerPrefs.HasKey("PlayerSkills"))
            {
                string json = PlayerPrefs.GetString("PlayerSkills");
                var wrapper = JsonUtility.FromJson<SkillSaveWrapper>(json);
                if (wrapper?.skills != null)
                {
                    foreach (int s in wrapper.skills)
                    {
                        if (!SaveData.unlockedSkills.Contains(s))
                            SaveData.unlockedSkills.Add(s);
                    }
                }
            }

            // 按键绑定
            if (PlayerPrefs.HasKey("GameKeyBindings_v2"))
            {
                string json = PlayerPrefs.GetString("GameKeyBindings_v2");
                var data = JsonUtility.FromJson<BindingSave>(json);
                if (data?.actions != null && data.keys != null)
                {
                    for (int i = 0; i < data.actions.Length && i < data.keys.Length; i++)
                    {
                        SaveData.keyActions.Add(data.actions[i]);
                        SaveData.keyCodes.Add(data.keys[i]);
                    }
                }
            }

            // 音量
            if (PlayerPrefs.HasKey("BGMVolume"))
                SaveData.bgmVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("BGMVolume"));
            if (PlayerPrefs.HasKey("SFXVolume"))
                SaveData.sfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("SFXVolume"));

            Save();
        }

        // 旧版 PlayerPrefs 用的包装类（仅用于迁移反序列化）
        [System.Serializable]
        private class SkillSaveWrapper { public System.Collections.Generic.List<int> skills; }
        [System.Serializable]
        private class BindingSave { public int[] actions; public int[] keys; }

        #endregion

        #region ===== 游戏状态 / 暂停 =====

        /// <summary>暂停游戏：Time.timeScale 置 0，状态切到 Paused</summary>
        public void PauseGame()
        {
            if (State == GameState.Paused) return;
            State = GameState.Paused;
            Time.timeScale = 0f;
        }

        /// <summary>恢复游戏：Time.timeScale 置 1，状态切到 Playing</summary>
        public void ResumeGame()
        {
            if (State == GameState.Playing) return;
            State = GameState.Playing;
            Time.timeScale = 1f;
        }

        /// <summary>切换暂停/恢复</summary>
        public void TogglePause()
        {
            if (State == GameState.Paused) ResumeGame();
            else PauseGame();
        }

        #endregion
    }
}
