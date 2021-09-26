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
        /// 添加或移除仅进行对话事件使用的参数
        /// </summary>
        public class ChatEventControlParam
        {
            /// <summary>
            /// 仅对话事件 ID，一般这个 ID 和对应的对话的 ID 相同
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

            public ChatEventControlParam(int id, Func<int, bool> condition, Action<int> after)
            {
                this.id = id;
                this.condition = condition;
                this.after = after;
            }
        }

        /// <summary>
        /// 添加的仅进行对话事件列表
        /// </summary>
        internal static List<ChatEventControlParam> addChatEvents = new List<ChatEventControlParam>();
        /// <summary>
        /// 阻断的仅进行对话事件列表
        /// </summary>
        internal static List<ChatEventControlParam> blockChatEvents = new List<ChatEventControlParam>();
        
        /// <summary>
        /// 添加仅进行对话事件
        /// </summary>
        /// <param name="id">对话 ID</param>
        /// <param name="condition">传入事件 id，如果返回 true 则添加对话事件，否则不添加</param>
        /// <param name="after">这个事件成功添加后执行的方法</param>
        /// <returns></returns>
        public static ChatEventControlParam AddChatEvent(int id, Func<int, bool> condition, Action<int> after)
        {
            ChatEventControlParam param = new ChatEventControlParam(id, condition, after);

            addChatEvents.Add(param);

            return param;
        }
        /// <summary>
        /// 取消添加仅进行对话事件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool RemoveAddChatEvent(ChatEventControlParam param)
        {
            return addChatEvents.Remove(param);
        }
        /// <summary>
        /// 阻断仅进行对话事件
        /// </summary>
        /// <param name="condition">符合这个条件的仅进行对话事件将会被移除</param>
        /// <param name="after">这个事件成功移除后执行的方法</param>
        public static ChatEventControlParam BlockChatEvent(Func<int, bool> condition, Action<int> after)
        {
            ChatEventControlParam param = new ChatEventControlParam(0, condition, after);

            blockChatEvents.Add(param);

            return param;
        }
        /// <summary>
        /// 取消阻断仅进行对话事件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool RemoveBlockChatEvent(ChatEventControlParam param)
        {
            return blockChatEvents.Remove(param);
        }
    }

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

            Main.ModEntry.Logger.Log("添加对话事件方法调用完毕，id = " + id);

            // 如果开启了设置则在游戏内提示
            if (Main.settings.showCreateEventTip)
            {
                TipsManager.instance.AddTips("添加对话事件方法调用完毕，id = " + id, 1);
            }

            // 如果开启了设置则输出事件数据
            if (Main.settings.printEventDataToLog)
            {
                Main.ModEntry.Logger.Log("输出 id = " + id + " 的对话事件数据");

                try
                {
                    XmlData data = ReadXml.GetData("comedy", id);

                    data.Log();
                }
                catch (KeyNotFoundException)
                {
                    Main.ModEntry.Logger.Log("没有 id = " + id + "的对话事件数据");
                }
            }

            // 遍历阻断条件，如果符合条件则移除事件
            EventControl.blockChatEvents.ForEach(blockEvent =>
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
    /// 添加一般的仅对话事件的方法
    /// </summary>
    [HarmonyPatch(typeof(comedy_manager), "week_end")]
    public static class comedy_manager_week_end
    {
        private static void Postfix(comedy_manager __instance)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("添加一般的仅对话事件的方法调用完毕");

            // 遍历新增条件，添加其中所有符合条件的事件
            EventControl.addChatEvents.ForEach(addEvent =>
            {
                if (addEvent.condition.Invoke(addEvent.id))
                {
                    // 添加事件
                    action_manager.Instance.add_button_comedy(addEvent.id);

                    // 调用添加成功后执行的方法
                    addEvent.after.Invoke(addEvent.id);
                }
            });
        }
    }

    /// <summary>
    /// 读取数据方法。仅对话事件的数据从这个方法获取，考虑到可能的新添加事件需要在这里对原版没有的事件进行处理
    /// </summary>
    [HarmonyPatch(typeof(ReadXml), "GetData")]
    public static class ReadXml_GetData
    {
        /// <summary>
        /// 在前后缀之间传递 GetData 方法参数的类
        /// </summary>
        private class GetDataParam
        {
            public string fileName;
            public int id;

            public GetDataParam(string fileName, int id)
            {
                this.fileName = fileName;
                this.id = id;
            }
        }

        private static void Prefix(out GetDataParam __state, string fileName, ref int id)
        {
            // 将参数传给后缀，这是因为这个方法有可能在内部改变参数
            __state = new GetDataParam(fileName, id);

            // 如果 Mod 未启动则不作处理
            if (!Main.enabled)
            {
                return;
            }

            // 如果要获取的是仅对话事件数据 并且 没有要获取的 id 的仅对话事件，那么这个事件就是新添加的事件，将 id 替换为默认事件 id
            if ("comedy".Equals(__state.fileName) && !ReadXml.HaveData(__state.fileName, __state.id))
            {
                id = 8100107;
                return;
            }
        }

        private static void Postfix(GetDataParam __state, ref XmlData __result)
        {
            // 如果 Mod 未启动则不作处理
            if (!Main.enabled)
            {
                return;
            }

            // 如果要获取的不是仅对话事件数据则不处理
            if (!"comedy".Equals(__state.fileName))
            {
                return;
            }

            // 如果本来的参数 id 是获取不到数据的，那么这个事件很可能是原版没有的、由 Mod 添加的事件，用参数 id 替换掉获取到的 id
            if(!ReadXml.HaveData(__state.fileName, __state.id))
            {
                __result.value["chat_id"] = __state.id + "";
            }
        }
    }
}
