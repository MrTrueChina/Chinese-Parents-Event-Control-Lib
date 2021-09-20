using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.EventControlLib.Events
{
    /// <summary>
    /// 添加起名字事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_name")]
    public static class action_manager_add_button_name
    {
        private static void Postfix()
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加起名字事件的方法调用完毕");

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加起名字事件的方法调用完毕", 1);
            }
        }
    }
}
