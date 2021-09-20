using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.EventControlLib
{
    public static partial class EventControl
    {
        /// <summary>
        /// 添加或移除男生找女儿约会事件使用的参数
        /// </summary>
        public class BoyLovingEventControlParam
        {
            /// <summary>
            /// 男同学 ID
            /// </summary>
            public int boyId;
            /// <summary>
            /// 事件 ID
            /// </summary>
            public int eventId;
            /// <summary>
            /// 条件
            /// </summary>
            public Func<int, int, bool> condition;
            /// <summary>
            /// 成功添加或移除事件后调用的方法
            /// </summary>
            public Action<int, int> after;

            public BoyLovingEventControlParam(int boyId, int eventId, Func<int, int, bool> condition, Action<int, int> after)
            {
                this.boyId = boyId;
                this.eventId = eventId;
                this.condition = condition;
                this.after = after;
            }
        }

        /// <summary>
        /// 添加的男生找女儿约会事件列表
        /// </summary>
        internal static List<BoyLovingEventControlParam> addBoyLovingEvents = new List<BoyLovingEventControlParam>();
        /// <summary>
        /// 阻断的男生找女儿约会事件列表
        /// </summary>
        internal static List<BoyLovingEventControlParam> blockBoyLovingEvents = new List<BoyLovingEventControlParam>();

        /// <summary>
        /// 添加男生找女儿约会事件
        /// </summary>
        /// <param name="boyId">男同学 ID</param>
        /// <param name="eventId">事件数据 ID</param>
        /// <param name="condition"><boyId, eventId>符合这个条件则添加</param>
        /// <param name="after">这个事件成功添加后执行的方法</param>
        /// <returns></returns>
        public static BoyLovingEventControlParam AddBoyLovingEvent(int boyId, int eventId, Func<int, int, bool> condition, Action<int, int> after)
        {
            BoyLovingEventControlParam param = new BoyLovingEventControlParam(boyId, eventId, condition, after);

            addBoyLovingEvents.Add(param);

            return param;
        }
        /// <summary>
        /// 取消添加男生找女儿约会事件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool RemoveAddBoyLovingEvent(BoyLovingEventControlParam param)
        {
            return addBoyLovingEvents.Remove(param);
        }
        /// <summary>
        /// 阻断男生找女儿约会事件
        /// </summary>
        /// <param name="condition">符合这个条件的男生找女儿约会将会被移除</param>
        /// <param name="after">这个事件成功移除后执行的方法</param>
        public static BoyLovingEventControlParam BlockBoyLovingEvent(Func<int, int, bool> condition, Action<int, int> after)
        {
            BoyLovingEventControlParam param = new BoyLovingEventControlParam(0, 0, condition, after);

            blockBoyLovingEvents.Add(param);

            return param;
        }
        /// <summary>
        /// 取消阻断男生找女儿约会事件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool RemoveBlockBoyLovingEvent(BoyLovingEventControlParam param)
        {
            return blockBoyLovingEvents.Remove(param);
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

            // 遍历阻断条件，如果符合条件则移除事件
            EventControl.blockBoyLovingEvents.ForEach(blockEvent =>
            {
                // 记录是否已经移除，因为无论有多少个移除实际上都是指向同一个目标，只能移除一次
                bool isRemoved = false;

                if (blockEvent.condition.Invoke(boyId, eventId))
                {
                    // 事件还没有被移除则移除事件
                    if (!isRemoved)
                    {
                        // 移除最后一个事件
                        EventControl.RemoveLastEvent();

                        isRemoved = true;
                    }

                    // 调用移除成功后的回调
                    blockEvent.after.Invoke(boyId, eventId);
                }
            });
        }
    }

    /// <summary>
    /// 调用添加男生找女主约会事件方法的方法
    /// </summary>
    [HarmonyPatch(typeof(BoysManager), "CreateEvent")]
    public static class BoysManager_OnEndRound
    {
        private static void Postfix(BoysManager __instance)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("调用添加男生找女主约会事件方法的方法即将调用");

            // 遍历新增条件，添加其中所有符合条件的事件
            EventControl.addBoyLovingEvents.ForEach(addEvent =>
            {
                if (addEvent.condition.Invoke(addEvent.boyId, addEvent.eventId))
                {
                    // 添加事件
                    action_manager.Instance.AddBoyEventButton(addEvent.boyId, addEvent.eventId);

                    // 调用添加成功后执行的方法
                    addEvent.after.Invoke(addEvent.boyId, addEvent.eventId);
                }
            });
        }
    }
}
