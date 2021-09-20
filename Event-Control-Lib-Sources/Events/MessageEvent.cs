﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.EventControlLib.Events
{

    /// <summary>
    /// 看名字是一个发出信息的事件，但是尝试遍历数据后发现这个方法在原逻辑中是调用不到的，怀疑这是个曾经有用或设计预留的废方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_message")]
    public static class action_manager_add_button_message
    {
        private static void Postfix(int id)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("action_manager.add_button_message 调用完毕，id = " + id);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("action_manager.add_button_message 调用完毕，id = " + id, 1);
            }

            // 如果开启了设置则输出事件数据
            if (Main.settings.printEventDataToLog)
            {
                Main.ModEntry.Logger.Log("输出 id = " + id + " 的 add_button_message 事件数据");

                try
                {
                    XmlData data = ReadXml.GetData("message", id);

                    data.Log();
                }
                catch (KeyNotFoundException)
                {
                    Main.ModEntry.Logger.Log("没有 id = " + id + "的 add_button_message 事件数据");
                }
            }
        }
    }


    // 这是遍历随回合数发生的事件的方法，add_button_message 是在 action_open = 2 的时候执行的，但是遍历出的数据里没有 action_open = 2 的
    //[HarmonyPatch(typeof(week_player), "action_week_start")]
    //public static class week_player_action_week_start
    //{
    //    private static void Postfix(int id)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return;
    //        }

    //        Main.ModEntry.Logger.Log("week_player.action_week_start 调用完毕，id = " + id);


    //        for (int i = 0; i < 60; i++)
    //        {
    //            if (ReadXml.HaveData("week_data", i))
    //            {
    //                Main.ModEntry.Logger.Log("查询第 " + i + " 天的事件数据");

    //                XmlData data = ReadXml.GetData("week_data", i);

    //                data.Log();
    //            }
    //            else
    //            {
    //                Main.ModEntry.Logger.Log("第 " + i + " 天没有事件数据");
    //            }
    //        }
    //    }
    //}
}
