using SceneKit;

namespace IQuarters.iOS;

// Presentation only: no changes to input, collision meshes, animation curves or physics.
static class RealismRendering
{
    // Conservative starting presets, not a claim of measured device performance.
    // Selected once per game to avoid shader/pipeline changes during a throw.
    public static string Quality {
        get => NSUserDefaults.StandardUserDefaults.StringForKey("realism.quality") ?? "auto";
        set => NSUserDefaults.StandardUserDefaults.SetString(value, "realism.quality");
    }
    public static bool Enhanced => Quality == "enhanced" || (Quality == "auto" &&
        NSProcessInfo.ProcessInfo.PhysicalMemory >= 4UL * 1024 * 1024 * 1024 &&
        !NSProcessInfo.ProcessInfo.LowPowerModeEnabled);

    public static void Configure(SCNCamera camera, SCNView view)
    {
        bool enhanced = Enhanced;
        view.AntialiasingMode = enhanced ? SCNAntialiasingMode.Multisampling4X : SCNAntialiasingMode.Multisampling2X;
        camera.WantsHdr = true;
        camera.WantsExposureAdaptation = false; // No brightness pumping as the coin moves.
        camera.ExposureOffset = 0;
        camera.WhitePoint = 4;
        camera.BloomIntensity = enhanced ? .14f : .07f;
        camera.BloomThreshold = 1.1f;
        camera.BloomBlurRadius = enhanced ? 7 : 4;
        camera.ScreenSpaceAmbientOcclusionIntensity = enhanced ? .28f : 0;
        camera.ScreenSpaceAmbientOcclusionRadius = .18f;
        camera.ScreenSpaceAmbientOcclusionBias = .015f;
        camera.VignettingIntensity = .08f;
        camera.VignettingPower = 1.4f;
        // HUD is rendered by its separate, unchanged camera; it remains sharp.
    }
}

public sealed partial class LegacyScene
{
    bool realismMaterialsApplied;
    public void ApplyRealismMaterials()
    {
        if(realismMaterialsApplied)return;
        realismMaterialsApplied=true;
        UIImage Texture(string name) => UIImage.FromFile(Path.Combine(NSBundle.MainBundle.ResourcePath!, "Realism", name+".png"))!;
        var environment = Texture("studio-environment");
        // Recovered fixed-function lights overpower energy-conserving materials.
        void Balance(SCNNode node) {
            if(node.Light is {} light) light.Intensity *= light.LightType == SCNLightType.Ambient ? .25f : .14f;
            foreach(var child in node.ChildNodes) Balance(child);
        }
        Balance(Scene.RootNode);
        Scene.LightingEnvironment.Contents = environment;
        Scene.LightingEnvironment.Intensity = .8f;
        var definitions = assets.Read("scene.json").GetProperty("materials");
        foreach (var (key, material) in materials)
        {
            string name = definitions.GetProperty(key).GetProperty("name").GetString() ?? "";
            switch (name)
            {
                case "quarter00":
                    Physical(material, .85f, .36f);
                    material.Diffuse.Intensity = .7f;
                    material.Roughness.Contents = Texture("metal-roughness");
                    break;
                case "table_00":
                    Physical(material, 0, .6f);
                    material.Normal.Contents = Texture("wood-normal");
                    material.Normal.Intensity = .45f;
                    material.Roughness.Contents = Texture("wood-roughness");
                    break;
                case "defl_cellphone01":
                case "pendulum_00":
                    Physical(material, .35f, .32f);
                    break;
                case "check_holder_00":
                case "bobble_00":
                case "hula_00":
                case "biplane_00":
                case "deflanim_bird01":
                    Physical(material, 0, .62f);
                    break;
                case "shotglass_00":
                    // Keep the authored alpha silhouettes: these legacy meshes are
                    // not closed glass volumes and cannot produce real refraction.
                    material.LightingModelName = SCNLightingModel.Blinn;
                    material.LitPerPixel = true;
                    material.ShaderModifiers = null;
                    material.ShaderModifiers = new SCNShaderModifiers {
                        EntryPointSurface = "_surface.emission.rgb = _surface.diffuse.rgb * 0.4;"
                    };
                    material.Specular.Contents = UIColor.FromWhiteAlpha(.45f, 1);
                    material.Shininess = 90;
                    material.Reflective.Contents = environment;
                    material.Reflective.Intensity = .24f;
                    material.FresnelExponent = 4;
                    material.TransparencyMode = SCNTransparencyMode.AOne;
                    material.WritesToDepthBuffer = false;
                    break;
            }
        }
    }

    static void Physical(SCNMaterial material, float metalness, float roughness)
    {
        material.LightingModelName = SCNLightingModel.PhysicallyBased;
        material.LitPerPixel = true;
        material.ShaderModifiers = null;
        material.Emission.Contents = UIColor.Black;
        material.Metalness.Contents = NSNumber.FromFloat(metalness);
        material.Roughness.Contents = NSNumber.FromFloat(roughness);
    }
}
