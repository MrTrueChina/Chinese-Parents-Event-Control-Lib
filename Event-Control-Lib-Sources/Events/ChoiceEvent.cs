using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.EventControlLib.Events
{
    /// <summary>
    /// 添加抉择事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_question")]
    public static class action_manager_add_button_question
    {
        private static void Postfix(int type)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加抉择事件的方法调用完毕，type = " + type);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加抉择事件的方法调用完毕，type = " + type, 1);
            }

            // 这个 type 被用在两个连续的随机数中，基本上看不到输出的价值了
        }
    }
}
