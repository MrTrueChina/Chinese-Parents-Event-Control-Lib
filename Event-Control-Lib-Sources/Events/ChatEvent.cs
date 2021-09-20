using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.EventControlLib.Events
{
    /// <summary>
    /// 添加仅发出对话的事件的方法。好感度事件、学习技能后的事件、转学事件都是从这里发出的
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_comedy")]
    public static class action_manager_add_button_comedy
    {
        private static void Postfix(int id)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("action_manager.add_button_comedy(" + id + ") 方法调用完毕");

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("action_manager.add_button_comedy(" + id + ") 方法调用完毕", 1);
            }

            // 如果开启了设置则输出事件数据
            if (Main.settings.printEventDataToLog)
            {
                Main.ModEntry.Logger.Log("输出 id = " + id + " 的 add_button_comedy 事件数据");

                try
                {
                    XmlData data = ReadXml.GetData("comedy", id);

                    data.Log();
                }
                catch (KeyNotFoundException)
                {
                    Main.ModEntry.Logger.Log("没有 id = " + id + "的 add_button_comedy 事件数据");
                }
            }
        }
    }
}
