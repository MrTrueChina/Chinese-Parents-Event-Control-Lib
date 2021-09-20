using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.EventControlLib.Events
{
    /// <summary>
    /// 添加考试分数事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_score")]
    public static class action_manager_add_button_score
    {
        private static void Postfix(int id)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加考试分数事件的方法调用完毕，id = " + id);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加考试分数事件的方法调用完毕，id = " + id, 1);
            }

            // 这个 id 似乎被当做分数使用，总之这个方法似乎是没有使用这个 id 读取数据，没有输出数据的必要
        }
    }
}
