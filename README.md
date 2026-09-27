# iQuarters Realism

**An experimental graphics edition of iQuarters for iPhone and iPad, built in C# without Unity.**

This is an independent copy of the [native iQuarters reconstruction](https://github.com/strawberrypoptarts/iQuarters), starting from commit `4798e18`. It explores richer rendering while retaining the recovered gameplay, physics, levels, menus, animations, and audio. The original and web repositories are separate.

## Game-style Graphics menu — 0.5.0

Graphics now uses the recovered four-row SceneKit menu, original green button artwork, and original `npin`, `npout`, and button-press animation clips. Tap Quality, Antialias, or Lamp to cycle the saved setting; Done or the original back arrow returns home. Changes apply to the next game. New labels use outlined bold lettering; the original artwork and animations are reused, but the new label font is an approximation. Player-selection materials are restored before returning home.

## Performance and antialiasing — 0.4.0

- Efficient removes bloom and vignette passes and halves shadow filtering from 8 to 4 samples. Native resolution, HDR tone mapping, all PBR maps, glass, and the lamp remain. Shadows may look slightly less soft.
- Leaving a screen explicitly stops its SceneKit render loops.
- Graphics now offers Automatic, Off, 2×, and 4× antialiasing. The selected mode covers gameplay, HUD geometry, and menus when created. Automatic remains 2× for Efficient and 4× for Enhanced.
- Trilinear mip filtering and 2× anisotropic filtering reduce texture shimmer at oblique angles.
- These changes target older devices including iPhone 6s; no on-device frame-rate benchmark has been measured. Automatic or Efficient with Automatic antialiasing is the recommended starting point on 6s.

## Existing graphics

- Custom 1024px oak table albedo, normal, and roughness maps with natural grain, knots, and worn satin-varnish detail. These AI-assisted artist-style maps replace the rejected uniform procedural material. Table UVs use a planar layout; collision geometry is unchanged.
- Custom quarter color, normal, and roughness maps add face detail, simulated engraving relief, and circulated-metal wear while retaining the original mesh and face layout.
- A modeled overhead pendant lamp, enabled by default. Its warm spotlight casts filtered shadow-map shadows from the quarter and opaque obstacles.
- **Graphics → Turn lamp off/on** saves the choice. It applies on the next game start/resume.
- Enhanced uses a 2048px shadow map with 16 samples; Efficient uses 1024px with 4 samples. These are initial budgets, not device benchmark results.
- Transparent glass keeps approximate contact shadows rather than casting opaque silhouettes. This is not ray-traced glass, transmission, or caustics.
- A green-and-yellow version of the original-style silver-quarter icon, installed in every iPhone/iPad icon size.
- Complete prop material profiles: painted-resin bobblehead/hula figures, metal/plastic/glass phone with lit screens, brass lighters, glass bottles and drinking bird, painted aircraft, wood-and-metal pendulum/ballista, leather check holder, and wood rotating stand.
- New detailed phone and bobblehead atlases. Other props retain their authored color textures with differentiated PBR roughness, metalness, and clear coat. UI and background photographs retain their original rendering.
- Glass now uses alpha-blended PBR shading. It remains an approximation without true transmission, refraction, or caustics.

### Previous lighting improvements

- Rebalanced the recovered lights for the new materials after a desktop preview exposed severe highlight clipping in 0.1.0.
- Added offline-generated wood normal/roughness maps and fine metal roughness variation.
- Improved glass readability under the adjusted lighting.
- Kept the IPA local at the owner's request; source changes are published here.

### Existing features

- Physically based coin, table, and selected prop materials, with individual metalness and roughness.
- An original procedural environment map for metallic highlights and glass reflections.
- Per-pixel glass highlights and view-dependent reflections, preserving the original transparency silhouettes.
- HDR rendering, restrained bloom, and subtle edge shading.
- Screen-space contact shading and 4× antialiasing in Enhanced mode; no bloom/vignette, 2× antialiasing, and no screen-space occlusion in Efficient mode.
- **GRAPHICS** below **HIGH SCORES**, assembled from the original green button border and letter sprites.
- Persistent **Automatic / Efficient / Enhanced** quality choices. Changes apply on the next game start or resume. Automatic selects Enhanced on devices with at least 4 GiB RAM when Low Power Mode is off; this is a conservative initial heuristic, not a performance benchmark.

HUD rendering remains separate from the gameplay effects. Timestamp-based flick input and the existing frame-rate-independent physics are retained, including the request for up to 120 Hz on compatible displays.

## Install

The experimental IPA is currently distributed locally, not as a public GitHub release. Building and packaging creates `dist/iQuarters-realism-0.5.0-ios15.ipa`.

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

All converted assets needed to build are included. Neither the original IPA nor Unity is required. `Recovery/tools/generate_realism_environment.py` regenerates the included reflection map using only Python's standard library. `generate_realism_surfaces.py` regenerates the microstructure maps; the table map originals and material notes are in `Artwork/Table`. These are synthetic microstructure, not recovered height measurements.

## Source

- `Recovered/Core`: original C# gameplay and physics.
- `Recovered/iOS/RealismRendering.cs`: material and camera effects, quality selection.
- `Recovered/iOS/GraphicsMenu.cs`: original-style button and graphics choices.
- `Recovery/realism`: new rendering assets.
- `Recovered/Verification`: gameplay verification suite.

Existing recovery notes describe the baseline reconstruction; the realism edition intentionally changes its rendering. See [Apple's physically based shading documentation](https://developer.apple.com/documentation/scenekit/scnmaterial/lightingmodel-swift.struct/physicallybased) for the material model and [ambient occlusion documentation](https://developer.apple.com/documentation/scenekit/scncamera/screenspaceambientocclusionintensity) for the contact-shading effect.
