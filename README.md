# 中国式家长 事件控制前置 Mod

## 对于普通玩家：
这个 Mod 对于普通玩家没有任何可用操作，仅作为前置 Mod 使用。

## 对于开发者：
这个 Mod 提供了添加事件和阻断事件发出的功能，以下是简化版文档，编译后文件附带完整 XML 注释文件：
## namespace：MtC.Mod.ChineseParents.ChatControlLib
### Class：ChatControl
``` C#
public static AnniversaryEventControlParam AddAnniversaryEvent(int id, Func<int, bool> condition, Action<int> after);
添加周年纪念日事件。

public static bool RemoveAddAnniversaryEvent(AnniversaryEventControlParam param);
取消添加周年纪念日事件。

public static AnniversaryEventControlParam BlockAnniversaryEvent(Func<int, bool> condition, Action<int> after);
阻断周年纪念日事件。

public static bool RemoveBlockAnniversaryEvent(AnniversaryEventControlParam param);
取消阻断周年纪念日事件。

public static BoyLovingEventControlParam AddBoyLovingEvent(int boyId, int eventId, Func<int, int, bool> condition, Action<int, int> after);
添加男同学找女儿约会事件。

public static bool RemoveAddBoyLovingEvent(BoyLovingEventControlParam param);
取消添加男同学找女儿约会事件。

public static BoyLovingEventControlParam BlockBoyLovingEvent(Func<int, int, bool> condition, Action<int, int> after);
阻断男同学找女儿约会事件。

public static bool RemoveBlockBoyLovingEvent(BoyLovingEventControlParam param);
取消阻断男同学找女儿约会事件。

public static ChatEventControlParam AddChatEvent(int id, Func<int, bool> condition, Action<int> after);
添加仅对话事件。

public static bool RemoveAddChatEvent(ChatEventControlParam param);
取消添加仅对话事件。

public static ChatEventControlParam BlockChatEvent(Func<int, bool> condition, Action<int> after);
阻断仅对话事件。

public static bool RemoveBlockChatEvent(ChatEventControlParam param);
取消阻断仅对话事件。
```

### Class EventIds
这个类记录了部分事件的 ID

## 使用方式
**[点击此处前往 releases 页面查看这个 Mod 的版本和使用方式](https://github.com/MrTrueChina/Chinese-Parents-Event-Control-Lib/releases)**  

**[点击此处前往中国式家长 Mod 导航库查看中国式家长所有 Mod 的使用方式](https://github.com/MrTrueChina/Chinese-Parents-Mods)**  
