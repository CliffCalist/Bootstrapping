# Bootstrapping

Bootstrapping is a modular initialization framework for Unity that provides full control over game and scene startup. It supports asynchronous boot modules, scene-level initialization, preload scene handling, and optional loading screens — all designed to be MonoBehaviour-free and editor-integrated.

---

## Features

- Game Boot Modules (sync or async), with optional execution order via attribute
- Scene Boot with completion signal
- Preload Scene between transitions to ensure memory cleanup
- Optional Loading Screen integration with minimum display time
- Boot module registry is generated at edit-time to avoid runtime reflection
- Editor menu for toggling and manual registry update

---

## Installing

To install via UPM, use "Install package from git URL" and add the following:

- https://github.com/CliffCalist/grouped-performance.git
- https://github.com/CliffCalist/Unity-Tools.git
- https://github.com/CliffCalist/Bootstrapping.git

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

### Preload Scene (required)

Bootstrapping requires a scene named `Preload` to act as an intermediate scene between unloading the current scene and loading the next one.  
Its purpose is to reduce memory spikes by ensuring the previous scene is fully unloaded before the next one begins loading.

If the `Preload` scene is missing or not added to Build Settings, the editor will automatically show a dialog and generate it.

You can also manually trigger its generation from the Unity menu:  
`Tools → WhiteArrow → Bootstrapping → Generate Preload Scene`
(Only available when the scene is missing or misconfigured)

---

### Loading Screen

Loading screens can be shown automatically during scene transitions by implementing:

```csharp
public interface ILoadingScreen
{
    bool IsShowed { get; }
    void Show(bool skipAnimations, Action callback);
    void Hide();
}
```

Assign your loading screen prefab in `Assets/Resources/BootSettings`.  
This is enough for the system to automatically instantiate and manage the screen.

You can also configure `_minLoadingScreenTime` in the same asset to ensure the screen is visible long enough for a smooth UX (especially when loading is very fast).

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
- unloading via preload scene
- scene boot execution

---

### Editor Integration

This package includes several helpful editor tools:

- **`Tools → WhiteArrow → Bootstrapping → Enable / Disable`** — allows to toggle the bootstrapping system on or off in the current project.
- **`Tools → WhiteArrow → Bootstrapping → Fix Preload Scene`** — validates and fixes the required Preload Scene (creates it if missing and ensures it's at build index 0).
- **`Tools → WhiteArrow → Bootstrapping → Update Registry`** — manually triggers registry update for game boot modules.

---

## Profiling Support

Bootstrapping uses [StackedProfiling](https://github.com/CliffCalist/stacked-profiling.git) for custom profiling of module execution.

- Each Game Boot Module is wrapped in a profiler sample with its type name
- Scene Boot classes are also profiled
- A global "GameBoot" sample wraps the entire game boot process

This enables better insight into boot performance and helps identify slow modules independently of Unity’s built-in timeline profiler.

---

## Roadmap

- [x] Auto-create and configure the Preload scene if missing
- [x] Use Preload as Build Index 0 to avoid initial scene reloading
- [ ] Support parallel execution of Game Boot Modules with controlled dependencies
- [ ] Framework settings window:
  - [ ] Toggle Preload scene usage
  - [x] Assign loading screen prefab via editor instead of code
  - [ ] Define module execution order visually
- [ ] Refactor `SceneLoader.LoadScene` into a non-coroutine async method
