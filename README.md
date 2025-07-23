# Bootstrapping

Bootstrapping is a modular initialization framework for Unity that provides full control over game and scene startup. It supports asynchronous boot modules, scene-level initialization, intermediate scene handling, and optional loading screens — all designed to be MonoBehaviour-free and editor-integrated.

---

## Features

- Game Boot Modules (sync or async), with optional execution order via attribute
- Scene Boot with completion signal
- Intermediate Scene between transitions to ensure memory cleanup
- Optional Loading Screen integration
- Boot module registry is generated at edit-time to avoid runtime reflection
- Editor menu for toggling and manual registry update

---

## Installing

To install via UPM, add the following Git URL to your `manifest.json`:

```json
"https://github.com/CliffCalist/Bootstrapping.git"
```

---

## Usage

### Game Boot Modules

Game boot modules are executed before any scene is fully loaded. A module can implement either interface depending on whether asynchronous logic is required:

```csharp
public interface IBootModule
{
    void Run();
}

public interface IAsyncBootModule
{
    Task RunAsync();
}
```

Example:

```csharp
[GameBootOrder(10)]
public class MyBootModule : IAsyncBootModule
{
    public async Task RunAsync()
    {
        await Task.Delay(500);
    }
}
```

> The initial scene (Build Index 0) is loaded automatically and reloaded after all boot modules finish.

---

### Scene Boot

Each scene must contain exactly one class derived from `SceneBoot`. This class runs immediately after scene load and must call `OnFinished()` to signal completion.

```csharp
public class MySceneBoot : SceneBoot
{
    protected override void Run()
    {
        InitializeStuff();
        OnFinished();
    }
}
```

---

### Intermediate Scene

To ensure full unloading of the previous scene before loading a new one, the system uses a lightweight intermediate scene.

- You must manually create an empty scene named:
  ```
  Intermediate
  ```

- This scene must be included in **Build Settings**

---

### Loading Screen

Loading screens can be shown automatically during scene transitions by implementing:

```csharp
public interface ILoadingScreen
{
    bool IsShowed { get; }

    void MarkAsDontDestroyOnLoad();

    void Show(bool skipAnimations, Action callback);
    void Hide();
}
```

Register your screen from a Game Boot Module:

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

### Loading a Scene

Use the built-in scene loader to transition between scenes:

```csharp
StartCoroutine(SceneLoader.LoadScene("Level01"));
```

To skip loading screen animations:

```csharp
StartCoroutine(SceneLoader.LoadScene("Level01", skipShowLoadingScreenAnimations: true));
```

This handles:
- optional loading screen
- unloading via intermediate scene
- scene boot execution

---

### Editor Integration

- Enable or disable the system via:

  ```
  Tools → WhiteArrow → Bootstrapping → Enable / Disable
  ```

- Manually update the registry:

  ```
  Tools → WhiteArrow → Bootstrapping → Update Registry
  ```

---

## Profiling Support

Bootstrapping uses [StackedProfiling](https://github.com/CliffCalist/stacked-profiling.git) for custom profiling of module execution.

- Each Game Boot Module is wrapped in a profiler sample with its type name
- Scene Boot classes are also profiled
- A global "GameBoot" sample wraps the entire game boot process

This enables better insight into boot performance and helps identify slow modules independently of Unity’s built-in timeline profiler.

---

## Roadmap

- [ ] Auto-create and configure the Intermediate scene if missing
- [ ] Use Intermediate as Build Index 0 to avoid initial scene reloading
- [ ] Support parallel execution of Game Boot Modules with controlled dependencies
- [ ] Framework settings window:
  - [ ] Toggle intermediate scene usage
  - [ ] Assign loading screen prefab via editor instead of code
  - [ ] Define module execution order visually
- [ ] Refactor `SceneLoader.LoadScene` into a non-coroutine async method
