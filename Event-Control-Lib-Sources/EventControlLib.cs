using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace MtC.Mod.ChineseParents.EventControlLib
{
    /// <summary>
    /// 这个 Mod 的设置
    /// </summary>
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        /// <summary>
        /// 是否在游戏内显示创建事件按钮方法被调用的提示
        /// </summary>
        [Draw("在游戏内显示创建事件按钮方法被调用的提示 - Show Create Event Button Method Run In Game")]
        public bool showCreateEventTip = false;

        /// <summary>
        /// 是否将事件数据输出到 Log
        /// </summary>
        [Draw("将事件数据输出到 Log - Print Event Data To Log")]
        public bool printEventDataToLog = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
        }
    }

    public static class Main
    {
        /// <summary>
        /// Mod 对象
        /// </summary>
        public static UnityModManager.ModEntry ModEntry { get; set; }

        /// <summary>
        /// 这个 Mod 是否启动
        /// </summary>
        public static bool enabled;

        /// <summary>
        /// 这个 Mod 的设置
        /// </summary>
        public static Settings settings;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            // 读取设置
            settings = Settings.Load<Settings>(modEntry);

            // 保存 Mod 对象并绑定事件
            ModEntry = modEntry;
            ModEntry.OnToggle = OnToggle;
            ModEntry.OnGUI = OnGUI;
            ModEntry.OnSaveGUI = OnSaveGUI;

            // 加载 Harmony
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();

            modEntry.Logger.Log("事件控制前置 Mod 加载完成");

            // 返回加载成功
            return true;
        }

        /// <summary>
        /// Mod Manager 对 Mod 进行控制的时候会调用这个方法
        /// </summary>
        /// <param name="modEntry"></param>
        /// <param name="value">这个 Mod 是否激活</param>
        /// <returns></returns>
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            // 将 Mod Manager 切换的状态保存下来
            enabled = value;

            // 返回 true 表示这个 Mod 切换到 Mod Manager 切换的状态，返回 false 表示 Mod 依然保持原来的状态
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
    }

    public static partial class EventControl
    {
        /// <summary>
        /// 移除事件列表中最后一个事件，也就是最新添加的事件
        /// </summary>
        internal static void RemoveLastEvent()
        {
            Main.ModEntry.Logger.Log("移除最后一个事件");

            if(action_manager.Instance == null || action_manager.Instance.ActionGameObjectsliList == null || action_manager.Instance.ActionGameObjectsliList.Count == 0)
            {
                Main.ModEntry.Logger.Log("移除最后一个事件失败，可能是事件控制器不存在，或者并没有已存在的事件");
                return;
            }

            // 从事件列表里获取最后一个事件
            GameObject eventButton = action_manager.Instance.ActionGameObjectsliList.Last();
            // 从列表里移除这个事件
            action_manager.Instance.ActionGameObjectsliList.Remove(eventButton);
            // 销毁事件按钮物体
            GameObject.Destroy(eventButton);
        }
    }
}
