﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.EventControlLib.Events
{
    /// <summary>
    /// 添加面子对决事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_facefight")]
    public static class action_manager_add_button_facefight
    {
        private static void Postfix(int facefight_id)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加面子对决事件的方法调用完毕，facefight_id = " + facefight_id);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加面子对决事件的方法调用完毕，facefight_id = " + facefight_id, 1);
            }

            // 这个 facefight_id 在方法里立刻就被拿去随机查另一个 id 了，暂时看不到输出的价值
        }
    }
}
