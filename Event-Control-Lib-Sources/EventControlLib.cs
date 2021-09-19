using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityModManagerNet;

namespace MtC.Mod.ChineseParents.EventControlLib
{
    /// <summary>
    /// 这个 Mod 的设置
    /// </summary>
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("在游戏内显示创建事件按钮方法被调用的提示 - Show Create Event Button Method Run In Game")]
        public bool showCreateEventTip = false;

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

    ////////--------////////--------//////// 对添加各种事件的方法的切入 ////////--------////////--------////////

    /// <summary>
    /// 添加周年纪念日事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "AddAnniversaryButton")]
    public static class action_manager_AddAnniversaryButton
    {
        private static void Postfix(int id)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加周年纪念日事件的方法调用完毕，id = " + id);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加周年纪念日事件的方法调用完毕，id = " + id, 1);
            }
        }
    }

    /// <summary>
    /// 添加男生找女主约会事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "AddBoyEventButton")]
    public static class action_manager_AddBoyEventButton
    {
        private static void Postfix(int boyId, int eventId)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加男生找女主约会事件的方法调用完毕，boyId = " + boyId + ", eventId = " + eventId);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加男生找女主约会事件的方法调用完毕，boyId = " + boyId + ", eventId = " + eventId, 1);
            }

            // 如果开启了设置则输出约会事件数据
            if (Main.settings.printEventDataToLog)
            {
                Main.ModEntry.Logger.Log("输出 id = " + eventId + " 的男生约会事件数据");

                try
                {
                    XmlData data = BoysManager.Instance.eventDataList.Get(eventId);

                    data.Log();
                }
                catch (KeyNotFoundException)
                {
                    Main.ModEntry.Logger.Log("没有 id = " + eventId + "的男生约会事件数据");
                }
            }
        }
    }

    /// <summary>
    /// 添加红包事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "AddButttonRedPacket")]
    public static class action_manager_AddButttonRedPacket
    {
        private static void Postfix()
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加红包事件的方法调用完毕");

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加红包事件的方法调用完毕", 1);
            }
        }
    }

    /// <summary>
    /// 疑似是选班委事件
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_candidate")]
    public static class action_manager_add_button_candidate
    {
        private static void Postfix()
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("action_manager.add_button_candidate 方法调用完毕");

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("action_manager.add_button_candidate 方法调用完毕", 1);
            }
        }
    }

    /// <summary>
    /// 翻译过来是“喜剧”“幽默的剧情”
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

    /// <summary>
    /// 添加作文大赛事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_Composition")]
    public static class action_manager_add_button_Composition
    {
        private static void Postfix(int type_id)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加作文大赛事件的方法调用完毕，type_id = " + type_id);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加作文大赛事件的方法调用完毕，type_id = " + type_id, 1);
            }

            // 这个 type_id 在方法里立刻就被拿去随机查另一个 id 了，暂时看不到输出的价值
        }
    }

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

    /// <summary>
    /// <see cref="XmlData"/> 的扩展方法类
    /// </summary>
    public static class XmlDataExtension
    {
        /// <summary>
        /// 输出所有内容到 Log
        /// </summary>
        /// <param name="data"></param>
        public static void Log(this XmlData data)
        {
            foreach(KeyValuePair<string,string> pair in data.value)
            {
                Main.ModEntry.Logger.Log("key = " + pair.Key + ", value = " + pair.Value);
            }
        }
    }

    /// <summary>
    /// 疑似是基于父母职业的那个期望事件，就是父母是什么就想让你当什么那个
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_fathertask")]
    public static class action_manager_add_button_fathertask
    {
        private static void Postfix()
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("action_manager.add_button_fathertask 方法调用完毕");

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("action_manager.add_button_fathertask 方法调用完毕", 1);
            }
        }
    }

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

    /// <summary>
    /// 疑似是添加某类只输出消息的事件的方法
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

    /// <summary>
    /// 疑似是添加某类包含问答的事件的方法，可能是抉择
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

            Main.ModEntry.Logger.Log("action_manager.add_button_question 调用完毕，type = " + type);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("action_manager.add_button_question 调用完毕，type = " + type, 1);
            }

            // 这个 type 被用在两个连续的随机数中，基本上看不到输出的价值了
        }
    }

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

    /// <summary>
    /// 添加选秀事件的方法
    /// </summary>
    [HarmonyPatch(typeof(action_manager), "add_button_starshow")]
    public static class action_manager_add_button_starshow
    {
        private static void Postfix(int id)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加选秀事件的方法调用完毕，id = " + id);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加选秀事件的方法调用完毕，id = " + id, 1);
            }

            // 这个 id 用在随机数生成里，目前看不到输出的价值
        }
    }

    ////////--------////////--------//////// 测试代码 ////////--------////////--------////////

    /// <summary>
    /// 疑似是进入下一回合的方法
    /// </summary>
    [HarmonyPatch(typeof(NextRound), "nextRound")]
    public static class NextRound_nextRound
    {
        private static void Prefix(NextRound __instance, keyValueUpdate kv)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("NextRound.nextRound 即将调用");

            if (kv != null)
            {
                Main.ModEntry.Logger.Log("key = " + kv.Key + ", value = " + kv.Values);
            }
            else
            {
                Main.ModEntry.Logger.Log("kv = null");
            }
        }
    }

    ///// <summary>
    ///// 疑似是安排确认执行方法
    ///// </summary>
    //[HarmonyPatch(typeof(NextRound), "buttonclick")]
    //public static class NextRound_buttonclick
    //{
    //    private static bool Prefix(NextRound __instance)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("NextRound.buttonclick 即将调用");

    //        return false;
    //    }
    //}
}
