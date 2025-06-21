# Bootstrapping for Unity

**Bootstrapping** is a modular initialization framework for Unity. It provides full control over both game startup and per-scene initialization, supports asynchronous logic, and intercepts Unity's default scene launching behavior for optimized memory and loading flow.

## ✨ Features

- ⚙️ Game Boot Modules — execute before any scene is loaded.
- 🧩 Scene Boot — per-scene logic triggered automatically after scene load.
- 🔄 Intermediate Scene — optional mechanism to fully unload memory between scenes.
- 🧪 MonoBehaviour-free game boot modules — written as plain C# classes.
- ✅ Execution order — supported via the `[GameBootOrder]` attribute.
- 🖼️ Optional loading screen — shown during scene transitions.
- 🛠️ Editor integration — registry is auto-generated, with manual override.

---

## 🚀 How It Works

### Game Boot Modules

Each boot module can implement one of two interfaces:

- `IBootModule` — for synchronous logic.
- `IAsyncBootModule` — for async logic (e.g. `await`-based initialization).

Here is an example of an async boot module:

```csharp
[GameBootOrder(10)]
public class MyBootModule : IAsyncBootModule
{
    public async Task RunAsync()
    {
        await LoadSomeData();
    }
}
```

These modules are executed **before the first scene**. Specifically, the scene defined at **Build Index 0** is loaded first, then reloaded after all game modules complete.

---

### Scene Boot

Each scene must have exactly **one** scene boot. You define it by inheriting from the base class:

```csharp
public class MySceneBoot : SceneBoot
{
    protected override void Run()
    {
        InitializeStuff();
        OnFinished(); // signal completion
    }
}
```

SceneBoot is executed automatically after the scene loads.  
The system will wait until `OnFinished()` is called.

---

## 🔃 Intermediate Scene

To ensure complete memory release between scenes, the system can insert a lightweight empty scene before loading the actual target scene.

> ✅ You must create this scene manually and name it exactly:
>
> ```
> Intermediate
> ```

> ✅ It must also be included in **Build Settings**.

This technique avoids memory spikes during transitions and allows safe unloading.

---

## 🖼️ Loading Screen (Optional)

You can display a loading screen during scene transitions. To do so, set the screen instance at runtime:

```csharp
LoadingScreenProvider.SetScreen(myScreen);
```

Your screen must implement the following interface:

```csharp
public class MyLoadingScreen : MonoBehaviour, ILoadingScreen
{
    public bool IsShowed { get; private set; }

    public void MarkAsDontDestroyOnLoad()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Show(bool skipAnimations, Action callback)
    {
        IsShowed = true;
        // optionally play animation...
        callback?.Invoke();
    }

    public void Hide()
    {
        IsShowed = false;
        // optionally hide instantly or animate...
    }
}
```

### 📌 When to set the loading screen

You should assign the loading screen **as early as possible**, before any scene transitions occur.  
The recommended way is to create a game boot module that instantiates and sets the screen:

```csharp
[GameBootOrder(-100)]
public class LoadingScreenInstaller : IBootModule
{
    public void Run()
    {
        var prefab = Resources.Load<MyLoadingScreen>("UI/LoadingScreen");
        var instance = Object.Instantiate(prefab);
        LoadingScreenProvider.SetScreen(instance);
    }
}
```

---

## 🎬 How to Load Scenes

To load a scene using the framework (with optional intermediate scene and loading screen), use the static method:

```csharp
StartCoroutine(SceneLoader.LoadScene("MyScene"));
```

Or, if you want to skip loading screen animations:

```csharp
StartCoroutine(SceneLoader.LoadScene("MyScene", skipShowLoadingScreenAnimations: true));
```

This will automatically handle:
- showing the loading screen (if set),
- transitioning through the intermediate scene (if applicable),
- executing scene boot logic.

---

## 🧰 Editor Integration

Boot modules are discovered automatically after each script compilation.

You can also trigger the scan manually via:

```
Tools → WhiteArrow → Bootstrapping → Update Registry
```

To enable or disable the entire boot system:

```
Tools → WhiteArrow → Bootstrapping → Enable / Disable
```

---

## 📦 Installation

This framework is distributed as source code.  
Simply copy it into your project.  
UPM support may be added later.

---

## ✅ TODO (What’s Next)

Here are some ideas we’re considering to make Bootstrapping even more powerful and easy to use:

- 🔧 **Auto-create Intermediate scene** if it doesn’t exist yet.
- 🧩 **Make Intermediate scene the build index 0** — for simpler full control without reloading.
- 🔄 **Optional intermediate scene on scene-to-scene transitions** — not just game start.
- 🖼️ **Assign loading screen via editor or config** — no code needed.
