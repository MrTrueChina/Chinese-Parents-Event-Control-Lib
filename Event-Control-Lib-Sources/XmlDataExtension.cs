using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MtC.Mod.ChineseParents.EventControlLib
{

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
            foreach (KeyValuePair<string, string> pair in data.value)
            {
                Main.ModEntry.Logger.Log("key = " + pair.Key + ", value = " + pair.Value);
            }
        }
    }
}
