<div align="center">
  <img src="https://i.postimg.cc/rwxPwfhG/2.jpg" alt="Sober Engine Logo" width="350">

  <h1>SOBER ENGINE</h1>
  
  <p>
    <strong>A high-performance, data-driven 2D game engine built from scratch with C# and OpenGL.</strong>
  </p>

<img src="https://img.shields.io/badge/build-passing-brightgreen?style=flat-square" alt="Build Status">
  <a href="https://opensource.org/licenses/MIT"><img src="https://img.shields.io/badge/license-MIT-informational?style=flat-square" alt="License: MIT"></a>
  <img src="https://img.shields.io/badge/dotnet-9.0-blueviolet?style=flat-square" alt="Target: .NET 9.0">
  <img src="https://img.shields.io/badge/gl-4.0_core-blue?style=flat-square" alt="Graphics: OpenGL 4.0">
  <img src="https://img.shields.io/badge/arch-ecs_contiguous-orange?style=flat-square" alt="Architecture: ECS">
</div>

---

Sober Engine is a custom 2D game framework designed around a strict Entity-Component-System (ECS) architecture. It avoids heavy object-oriented inheritance in favor of contiguous data storage, decoupling game logic from rendering state. The engine provides a complete suite of modular tools including a custom shader pipeline, 2D physics integration, spatial audio, and an embedded runtime editor.

## Table of Contents
1. [Engine Capabilities](#engine-capabilities)
2. [Memory & Performance](#memory--performance)
3. [Architecture Overview](#architecture-overview)
4. [Directory Structure](#directory-structure)
5. [Entity Component System (ECS)](#entity-component-system-ecs)
6. [Data-Driven Workflow](#data-driven-workflow)
7. [The Runtime Editor](#the-runtime-editor)
8. [Getting Started](#getting-started)
9. [Troubleshooting](#Troubleshooting)
10. [Dependencies & Credits](#dependencies--credits)
11. [License & Copyright](#license--copyright)

---

## Engine Capabilities

The engine is built to handle modern 2D rendering and gameplay systems with a focus on performance and modularity.

| Subsystem | Core Abilities | Implementation Details |
| :--- | :--- | :--- |
| **ECS Framework** | High-performance data processing | Generics-based `ComponentStore<T>`, integer-based Entity IDs, deterministic system execution ordering. |
| **Rendering** | Hardware-accelerated 2D graphics | Custom OpenGL 4 pipeline supporting batched sprites, indexed quad generation, multi-layered tilemaps, and parallax scrolling. |
| **Physics & Collision** | Predictable rigid-body dynamics | Custom 2D fixed-step physics integration. Supports AABB, Circle, and grid-based Tilemap collisions with layer-masking. |
| **Asset Management** | Optimized memory footprint | Lazy-loading system for textures (`StbImageSharp`), shaders, and audio clips. Assets are instantiated once. |
| **Visual Effects** | Dynamic lighting and particle pooling | 2D dynamic point lights via fragment shaders. Contiguous array particle pooling (8000+ limits) with additive/alpha blending. |
| **Audio** | Spatial and ambient soundscapes | OpenAL context wrapper supporting `.wav` parsing, spatial audio positioning, and source pooling via `AudioSourceComponent`. |
| **Post-Processing** | Screen-space visual enhancements | Framebuffer object (FBO) pipeline supporting screen quads, color grading, vignette, and kernel blurring algorithms. |

---

## Memory & Performance

Sober Engine utilizes a strict **Data-Oriented Design (DOD)**. Unlike traditional game engines that store objects as scattered pointers in memory, Sober packs components into contiguous arrays. 

* **Cache Locality:** By keeping components of the same type side-by-side, we virtually eliminate CPU cache misses during system updates.
* **Zero Allocation Loops:** The core update loop performs zero heap allocations, ensuring that Garbage Collection (GC) spikes never interrupt gameplay.
* **O(1) Access:** Component retrieval is a direct index lookup, providing constant-time performance regardless of scene complexity.

> **Benchmark:** The engine handles **10,000+ active entities** with concurrent lighting, physics, and particle effects while maintaining a stable **60 FPS** on integrated graphics.

---

## Architecture Overview

The engine relies on a highly modular architecture. The core execution loop handles input, time, and window management, while delegating all specific game logic and presentation to the ECS systems and the rendering backend.

<div align="center">
  <img src="https://i.postimg.cc/sg7NgTnJ/1.jpg" alt="Engine Architecture" width="800">
</div>

---

## Directory Structure

The repository maintains a strict separation of concerns. Engine core code does not depend on game-specific logic.

```text
/Sober
├── /Assets         # Raw data (JSON scenes/prefabs, PNG textures, WAV audio, GLSL shaders)
├── /Audio          # OpenAL audio context, WAV parsing, and source management
├── /ECS            # Core architecture (Components, Systems, Events, World storage)
├── /Editor         # Runtime developer tools, inspector rendering, and live serialization
├── /Engine         # High-level engine loop, GameWindow lifecycle, and input polling
├── /Rendering      # OpenGL abstractions (Meshes, Shaders, UI, Particles, Post-Processing, Tilemaps)
├── /Scene          # Scene lifecycle, JSON deserialization, and Prefab merging
└── Program.cs      # Application entry point
```

---

## Entity Component System (ECS)

Sober Engine avoids object-oriented inheritance trees in favor of composition. Entities are strictly integer IDs. Components are plain old data structs (PODs) stored in contiguous generic collections. Systems iterate over component intersections using a custom `Query` structure.

| Core Type | Function | Underlying Structure |
| :--- | :--- | :--- |
| **Entity** | A unique identifier representing a game object. | `int` (32-bit integer). |
| **Component** | Pure data struct defining a specific trait. | `struct` (Blittable, memory-contiguous). |
| **Store** | Holds all instances of a specific component type. | `ComponentStore<T>` utilizing arrays/dictionaries. |
| **System** | Processes logic for entities matching a query. | Classes implementing `ISystem` with `Update()` methods. |

Example system query within the engine:

```csharp
foreach (int id in Query.with<TransformComponent, LightComponent>(_world))
{
    var transform = _world.GetStore<TransformComponent>().Get(id);
    var light = _world.GetStore<LightComponent>().Get(id);
    // Process lighting data based on transform position
}
```

---

## Data-Driven Workflow

Levels and objects are constructed outside the compiler. The `SceneManager` handles loading base scene files and injecting `Prefab` overrides. 

| Asset Type | Primary Format | Handled By | Purpose |
| :--- | :--- | :--- | :--- |
| **Scenes** | `.json` | `SceneManager` | Defines level structure, background elements, and entity spawns. |
| **Prefabs** | `.json` | `Library` | Reusable entity templates with base component configurations. |
| **Tilemaps** | `.json` / `.csv` | `TilemapLoader` | Defines grid-based static geometry and visual terrain. |
| **Scripts** | `.json` | `ScriptSystem` | Defines linear sequences triggered by spatial zones. |

A standard prefab definition (`player_cat.json`):

```json
{
    "name": "player_cat_prefab",
    "entities": [
        {
            "name": "Player",
            "player": true,
            "transform": {
                "position": [ 0.0, -0.2 ],
                "scale": [ 0.6, 0.6 ]
            },
            "velocity": { "speed": 1.3 },
            "sprite": { "textureKey": "cat" }
        }
    ]
}
```

When a scene requests this prefab, the merge utility applies any scene-specific overrides (like local position) over the base prefab data before allocating the ECS components.

---

## The Runtime Editor

Sober Engine includes an embedded developer toolkit for live iteration without needing to recompile the project.

| Hotkey | Tool | Function |
| :---: | :--- | :--- |
| `F4` | **Toggle Inspector** | Halts gameplay input and opens a pixel-perfect property inspector. |
| `[` / `]` | **Entity Cycle** | Cycles selection through active scene entities. |
| `I, J, K, L` | **Live Transform** | Modifies the selected entity's local position in real-time. |
| `F5` | **Safe Serialization**| Patches the current ECS state back into the source JSON safely. |
| `F6` | **Hot Reload** | Instantly reloads the entire scene and all components from disk. |
| `F7` | **Debug Draw** | Toggles rendering layer for AABB bounds, colliders, and trigger zones. |

* **Shader Hot-Reloading:** Modifying a `.frag` or `.vert` file externally triggers an automatic recompilation and binding within the engine, completely bypassing application restarts.

---

## Getting Started

### Prerequisites
* .NET 9.0 SDK
* A GPU supporting OpenGL 4.0 or higher
* OpenAL hardware support (or an OpenAL Soft implementation)

### Build Instructions
1. Clone the repository.
2. Open `Sober.sln` in Visual Studio or your preferred IDE.
3. Ensure the `/Assets` directory properties are set to **"Copy if newer"** in your build configuration.
4. Build and execute `Sober.Program.Main`. The active startup scene is defined inside `/Engine/Core/Engine.cs` during `OnLoad()`.

---

## Troubleshooting

If you encounter issues during the initial setup, check the following common solutions:

* **Audio Crashes:** If the engine crashes immediately on startup, verify that **OpenAL** is properly installed. 
    * **Windows:** Usually bundled with GPU drivers; if missing, install the [OpenAL Windows Installer](https://www.openal.org/downloads/).
    * **Linux:** You may need to run `sudo apt install libopenal-dev`.
* **Missing Assets:** If you see "Pink Squares" or missing shaders, double-check that your `/Assets` folder in the IDE is set to **"Copy to Output Directory: Copy if newer"**.
* **OpenGL Version:** Ensure your hardware supports **OpenGL 4.0 Core Profile**. Older integrated chips (Pre-2015) may require a fallback to a 3.3 compatible shader set.

---

## Dependencies & Credits

This engine relies on the following open-source libraries:

* **[OpenTK](https://github.com/opentk/opentk) & OpenTK.Mathematics:** (v4.9.4) - Provides the low-level C# bindings for OpenGL and OpenAL. Licensed under the MIT License.
* **[StbImageSharp](https://github.com/StbSharp/StbImageSharp):** (v2.30.15) - A C# port of the stb_image.h library used for parsing PNG and JPG texture data. Dual-licensed under Public Domain and the MIT License.

---

## License & Copyright

**Copyright &copy; 2026 Sean Salehin.**

This project is licensed under the **MIT License**.

You are free to use, copy, modify, merge, publish, and distribute, the Software, provided that the above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

The "Sober Engine" name and accompanying logo are trademarks of Sean Salehin. Visual placeholder assets within the `/Assets` folder are provided for demonstration purposes only and do not carry copyright restriction.
