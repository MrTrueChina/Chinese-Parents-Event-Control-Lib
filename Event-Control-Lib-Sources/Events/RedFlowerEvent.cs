using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.EventControlLib.Events
{
    /// <summary>
    /// 添加幼儿园的小红花事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_flower")]
    public static class action_manager_add_button_flower
    {
        private static void Postfix(int id)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加小红花事件方法调用完毕");

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加小红花事件方法调用完毕", 1);
            }

            // 这个方法内部完全没用到那个 id 参数
        }
    }
}
