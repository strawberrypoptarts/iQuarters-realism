# iQuarters Realism

### 1.0

The classic quarter-tossing game, rebuilt in C# for iPhone and iPad with richer materials, lighting, and shadows. No Unity required.

Based on the [iQuarters reconstruction](https://github.com/strawberrypoptarts/iQuarters), this standalone edition keeps the recovered gameplay, physics, levels, animations, and audio while giving the playing field a new look. It installs alongside the original iQuarters.

## Features

- **Detailed surfaces:** custom wood and quarter textures with normal and roughness maps for grain, wear, and engraved detail.
- **Physically based props:** material finishes for the bobblehead, flip phone, bottles, aircraft, lighters, stands, and other obstacles.
- **Reflective glass:** transparent glass with highlights and environment reflections.
- **Overhead lighting:** a default-on pendant lamp that casts shadows across the playing field, with a switch in Graphics.
- **Scalable effects:** HDR lighting, with bloom and ambient occlusion in Enhanced mode and lighter rendering in Efficient mode.
- **Smoother edges:** selectable antialiasing and texture filtering to reduce shimmer.
- **Game-style settings:** original green menu artwork, background, slide transitions, and button-press animations.
- **Modern displays:** native-resolution rendering and support for up to 120 Hz on compatible devices, with frame-rate-independent flick input and physics.
- **Distinct identity:** a green-and-yellow app icon and separate app storage.

## Graphics settings

Open **GRAPHICS** below **HIGH SCORES**. Tap a setting to cycle its value, then select **DONE** or **BACK**. Gameplay changes apply when starting or resuming a game.

| Setting | Choices |
| --- | --- |
| Quality | Automatic, Efficient, Enhanced |
| Antialiasing | Automatic, Off, 2×, 4× |
| Lamp | On, Off |

Automatic quality selects Enhanced on devices with at least 4 GiB of memory when Low Power Mode is off; otherwise it uses Efficient. Automatic antialiasing uses 2× in Efficient and 4× in Enhanced.

For older devices such as iPhone 6s, start with **Efficient** and **Automatic antialiasing**. Efficient retains the PBR textures, native resolution, and lamp shadows while reducing shadow filtering and disabling the extra screen effects. Performance varies by device; a sustained frame rate is not guaranteed.

## Release and installation

[**View the 1.0 source release**](https://github.com/strawberrypoptarts/iQuarters-realism/releases/tag/v1.0)

- Requires **iOS or iPadOS 15 or later**, on an arm64 device.
- App name: **iQuarters Realism**.
- Bundle identifier: `com.itsgames.iquarters.realism`.
- IPA files are kept local and are not attached to the public release.
- Building and packaging produces an IPA in `dist/`. The package is ad-hoc signed; conventional sideloaders must provision and re-sign it.

The 1.0 release identifies the current source snapshot. App build metadata is retained from that snapshot.

## Build from source

The project uses macOS, **.NET SDK 10.0.401**, the matching **iOS 27 preview workload**, and **Xcode 27**. The deployment target remains iOS 15.

```sh
export DEVELOPER_DIR=/Applications/Xcode.app/Contents/Developer
dotnet workload restore Recovered/iOS/IQuarters.iOS.csproj
dotnet run --project Recovered/Verification -c Release
dotnet build Recovered/iOS/IQuarters.iOS.csproj \
  -c Release -r ios-arm64 \
  -p:EnableCodeSigning=false -p:UseSharedCompilation=false
python3 Recovery/tools/package_ipa.py
```

The converted assets needed to build are included. You do not need the original IPA or a Unity installation.

## Project layout

| Directory | Contents |
| --- | --- |
| `Recovered/Core` | C# gameplay and physics |
| `Recovered/iOS` | iOS app, rendering, menus, and controls |
| `Recovered/Verification` | Gameplay verification suite |
| `Recovery/converted` | Recovered game assets and animation data |
| `Recovery/realism` | Material textures and rendering profiles |
| `Artwork` | Artwork sources and material notes |
| `Recovery/tools` | Packaging, conversion, and preview utilities |

## About the visuals

The new textures include AI-assisted artwork and synthetic surface maps. The original meshes and room backgrounds remain part of the game's appearance. Glass uses an approximation of transparency and reflections rather than volumetric refraction or caustics. New graphics-menu lettering approximates the original font; its button artwork and animation clips come from the recovered game.

This repository is independent of both the [original iOS edition](https://github.com/strawberrypoptarts/iQuarters) and the [web edition](https://github.com/strawberrypoptarts/iQuarters-web).
