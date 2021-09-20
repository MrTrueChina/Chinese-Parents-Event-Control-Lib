using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace MtC.Mod.ChineseParents.EventControlLib
{
    public static partial class EventControl
    {
        /// <summary>
        /// 添加或移除周年纪念日事件使用的参数
        /// </summary>
        public class AnniversaryEventControlParam
        {
            /// <summary>
            /// 对话 id，这个参数只在添加时使用
            /// </summary>
            public int id;
            /// <summary>
            /// 条件
            /// </summary>
            public Func<int, bool> condition;
            /// <summary>
            /// 成功添加或移除事件后调用的方法
            /// </summary>
            public Action<int> after;

            public AnniversaryEventControlParam(int id, Func<int, bool> condition, Action<int> after)
            {
                this.id = id;
                this.condition = condition;
                this.after = after;
            }
        }

        /// <summary>
        /// 添加的周年纪念日事件列表
        /// </summary>
        internal static List<AnniversaryEventControlParam> addAnniversaryEvents = new List<AnniversaryEventControlParam>();
        /// <summary>
        /// 阻断的周年纪念日事件列表
        /// </summary>
        internal static List<AnniversaryEventControlParam> blockAnniversaryEvents = new List<AnniversaryEventControlParam>();

        /// <summary>
        /// 添加周年纪念日事件
        /// </summary>
        /// <param name="id">这个周年纪念日事件的对话的 id</param>
        /// <param name="condition">符合这个条件则添加</param>
        /// <param name="after">这个事件成功添加后执行的方法</param>
        /// <returns></returns>
        public static AnniversaryEventControlParam AddAnniversaryEvent(int id, Func<int, bool> condition, Action<int> after)
        {
            AnniversaryEventControlParam param = new AnniversaryEventControlParam(id, condition, after);

            addAnniversaryEvents.Add(param);

            return param;
        }
        /// <summary>
        /// 取消添加周年纪念事件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool RemoveAddAnniversaryEvent(AnniversaryEventControlParam param)
        {
            return addAnniversaryEvents.Remove(param);
        }
        /// <summary>
        /// 阻断周年纪念日事件
        /// </summary>
        /// <param name="condition">符合这个条件的周年纪念日将会被移除</param>
        /// <param name="after">这个事件成功移除后执行的方法</param>
        public static AnniversaryEventControlParam BlockAnniversaryEvent(Func<int, bool> condition, Action<int> after)
        {
            AnniversaryEventControlParam param = new AnniversaryEventControlParam(0, condition, after);

            blockAnniversaryEvents.Add(param);

            return param;
        }
        /// <summary>
        /// 取消阻断周年纪念日事件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool RemoveBlockAnniversaryEvent(AnniversaryEventControlParam param)
        {
            return blockAnniversaryEvents.Remove(param);
        }
    }

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

            // 遍历阻断条件，如果符合条件则移除事件
            EventControl.blockAnniversaryEvents.ForEach(blockEvent => 
            {
                // 记录是否已经移除，因为无论有多少个移除实际上都是指向同一个目标，只能移除一次
                bool isRemoved = false;

                if (blockEvent.condition.Invoke(id))
                {
                    // 事件还没有被移除则移除事件
                    if (!isRemoved)
                    {
                        // 移除最后一个事件
                        EventControl.RemoveLastEvent();

                        isRemoved = true;
                    }

                    // 调用移除成功后的回调
                    blockEvent.after.Invoke(id);
                }
            });
        }
    }

    /// <summary>
    /// 调用添加周年纪念日事件方法的方法
    /// </summary>
    [HarmonyPatch(typeof(AnniversaryManager), "CheckCondition")]
    public static class AnniversaryManager_CheckCondition
    {
        private static void Postfix(int skillId)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("调用添加周年纪念日事件方法的方法调用完毕，skillId = " + skillId);

            // 遍历新增条件，添加其中所有符合条件的事件
            EventControl.addAnniversaryEvents.ForEach(addEvent => 
            {
                if (addEvent.condition.Invoke(addEvent.id))
                {
                    // 添加事件
                    action_manager.Instance.AddAnniversaryButton(addEvent.id);

                    // 调用添加成功后执行的方法
                    addEvent.after.Invoke(addEvent.id);
                }
            });
        }
    }
}
