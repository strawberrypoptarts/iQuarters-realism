# iQuarters Realism

**An experimental graphics edition of iQuarters for iPhone and iPad, built in C# without Unity.**

This is an independent copy of the [native iQuarters reconstruction](https://github.com/strawberrypoptarts/iQuarters), starting from commit `4798e18`. It explores richer rendering while retaining the recovered gameplay, physics, levels, menus, animations, and audio. The original and web repositories are separate.

## Current graphics pass — 0.2.0

- Rebalanced the recovered lights for the new materials after a desktop preview exposed severe highlight clipping in 0.1.0.
- Added offline-generated wood normal/roughness maps and fine metal roughness variation.
- Improved glass readability under the adjusted lighting.
- Kept the IPA local at the owner's request; source changes are published here.

### Existing features

- Physically based coin, table, and selected prop materials, with individual metalness and roughness.
- An original procedural environment map for metallic highlights and glass reflections.
- Per-pixel glass highlights and view-dependent reflections, preserving the original transparency silhouettes.
- HDR rendering, restrained bloom, and subtle edge shading.
- Screen-space contact shading and 4× antialiasing in Enhanced mode; lighter bloom, 2× antialiasing, and no screen-space occlusion in Efficient mode.
- **GRAPHICS** below **HIGH SCORES**, assembled from the original green button border and letter sprites.
- Persistent **Automatic / Efficient / Enhanced** quality choices. Changes apply on the next game start or resume. Automatic selects Enhanced on devices with at least 4 GiB RAM when Low Power Mode is off; this is a conservative initial heuristic, not a performance benchmark.

HUD rendering remains separate from the gameplay effects. Timestamp-based flick input and the existing frame-rate-independent physics are retained, including the request for up to 120 Hz on compatible displays.

## Install

The experimental IPA is currently distributed locally, not as a public GitHub release. Building and packaging creates `dist/iQuarters-realism-0.2.0-ios15.ipa`.

- iOS/iPadOS 15 or later, arm64.
- App name: **iQuarters Realism**.
- Bundle ID: `com.itsgames.iquarters.realism`.
- Installs beside the original iQuarters and uses its own app storage.
- The IPA is ad-hoc signed; ordinary sideloaders must provision and re-sign it.

**This is a first graphics experiment, not a finished photorealistic remaster.** Device appearance, launch, battery use, and sustained performance still require physical-device testing. The room backdrop and meshes are the recovered low-resolution originals. Glass reflection is an approximation, not volumetric refraction. No new collision geometry or gameplay rules were introduced.

## Build

macOS, .NET SDK 10.0.401, the matching iOS 27 preview workload, and Xcode 27 are used for this source. The minimum deployment version remains iOS 15.

```sh
export DEVELOPER_DIR=/Applications/Xcode.app/Contents/Developer
dotnet workload restore Recovered/iOS/IQuarters.iOS.csproj
dotnet run --project Recovered/Verification -c Release
dotnet build Recovered/iOS/IQuarters.iOS.csproj \
  -c Release -r ios-arm64 \
  -p:EnableCodeSigning=false -p:UseSharedCompilation=false
python3 Recovery/tools/package_ipa.py
```

All converted assets needed to build are included. Neither the original IPA nor Unity is required. `Recovery/tools/generate_realism_environment.py` regenerates the included reflection map using only Python's standard library. `generate_realism_surfaces.py` regenerates the material data maps. These are synthetic microstructure, not recovered height measurements.

## Source

- `Recovered/Core`: original C# gameplay and physics.
- `Recovered/iOS/RealismRendering.cs`: material and camera effects, quality selection.
- `Recovered/iOS/GraphicsMenu.cs`: original-style button and graphics choices.
- `Recovery/realism`: new rendering assets.
- `Recovered/Verification`: gameplay verification suite.

Existing recovery notes describe the baseline reconstruction; the realism edition intentionally changes its rendering. See [Apple's physically based shading documentation](https://developer.apple.com/documentation/scenekit/scnmaterial/lightingmodel-swift.struct/physicallybased) for the material model and [ambient occlusion documentation](https://developer.apple.com/documentation/scenekit/scncamera/screenspaceambientocclusionintensity) for the contact-shading effect.
