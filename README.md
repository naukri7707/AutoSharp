# Auto Sharp

使用 C# 進行自動化。

## 編寫腳本

在 `AutoSharpDevEnv` 中建立腳本，並參考以下範例進行編寫。

```cs
using AutoSharp;
using AutoSharp.Models;
using AutoSharp.Triggers;
using System.Collections.Generic;

namespace Moudles
{
    public class HelloWorld : Component // AutoSharp 會自動偵測 Component 類別，並且在執行時載入
    {
        HWnd targetWindow; // 目標視窗

        // 初始化
        protected override void Start()
        {
            targetWindow = Window.FindWindow("Notepad", ""); // 尋找 Notepad 視窗
        }

        [TriggerSettings(appendCoroutine = "myCoroutine", priority = 10)]  // 設定協程
        [Hotkey(Keys.F1)] // 註冊觸發方式 (快捷鍵 : F1)
        public IEnumerator<Awaiter> SayHello()
        {
            Input.PostKeyPress(targetWindow, Keys.H);
            Input.PostKeyPress(targetWindow, Keys.E);
            Input.PostKeyPress(targetWindow, Keys.L);
            Input.PostKeyPress(targetWindow, Keys.L);
            Input.PostKeyPress(targetWindow, Keys.O);
            yield break;
        }

        [NotifyMatch(".*Beep!.*")] // 當 AutoSharp 接收到匹配正則 (.*Beep!.*) 的訊息時，觸發
        public IEnumerator<Awaiter> HelloAndBeep()
        {
            yield return Awaiter.Delay(1000); // 等待 1 秒
            yield return Awaiter.Routine(SayHello()); // 呼叫並等候 SayHello 協程
            Output.Beep(); // 發出蜂鳴聲
        }
    }
}

```

## 啟用腳本

### 一般

1. 執行 `AutoSharpDevTester` 專案

- 你可以使用 `send` 關鍵字來模擬訊息輸入，例如 `send hello` 會傳送 `hello` 給 AutoSharp

### 其他專案使用 AutoSharp

如果你希望在自己的專案中使用 AutoSharp。

1. 建置 `AutoSharpDevEnv` 將腳本儲存為一個模組
2. 建置 `AutoSharp` 專案
3. 為目標專案引用 `AutoSharp.dll` 參考
4. 使用 `AutoSharpLifeCycle.Start()` 開啟
5. 使用 `ModuleLoader.Load()` 載入目標模組

- 注意：模組(Module)及組件(Component)預設為關閉你需要使用自行在合適的時機啟動，或者使用 `ModuleLoader.LoadAndEnableAllModule()` 載入模組。

```cs
foreach (var module in Module.GetModules())
{
    foreach (var component in module.GetComponents<Component>())
    {
        component.Enabled = true;
    }

    module.Enabled = true;
}
```

### 在 ACT 中使用 AutoSharp

1. 建置 `AutoSharpDevEnv` 將腳本儲存為一個模組
2. 建置 `AutoSharpActPlugin` 專案
3. 將 `AutoSharpActPlugin.dll` 新增到 ACT Plugin 中
4. 在 `AutoSharpActPlugin.dll` 的目錄建立資料夾 `Modules` 並將目標模組放在這裡
5. 重新啟動 ACT
