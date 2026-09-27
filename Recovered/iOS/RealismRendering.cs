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
    public static bool LampEnabled {
        get => NSUserDefaults.StandardUserDefaults["realism.lamp"] == null || NSUserDefaults.StandardUserDefaults.BoolForKey("realism.lamp");
        set => NSUserDefaults.StandardUserDefaults.SetBool(value,"realism.lamp");
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
            if(node.Light is {} light) light.Intensity *= light.LightType == SCNLightType.Ambient ? .25f : RealismRendering.LampEnabled ? .08f : .14f;
            foreach(var child in node.ChildNodes) Balance(child);
        }
        Balance(Scene.RootNode);
        Scene.LightingEnvironment.Contents = environment;
        Scene.LightingEnvironment.Intensity = RealismRendering.LampEnabled ? .45f : .8f;
        using var props=System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(NSBundle.MainBundle.ResourcePath!,"Realism","prop-materials.json")));
        var definitions = assets.Read("scene.json").GetProperty("materials");
        foreach (var (key, material) in materials)
        {
            string name = definitions.GetProperty(key).GetProperty("name").GetString() ?? "";
            switch (name)
            {
                case "quarter00":
                    Physical(material, .85f, .36f);
                    material.Diffuse.Contents = Texture("quarter-albedo");
                    material.Diffuse.Intensity = .8f;
                    material.Normal.Contents = Texture("quarter-normal");
                    material.Normal.Intensity = .25f;
                    material.Roughness.Contents = Texture("quarter-roughness");
                    break;
                case "table_00":
                    Physical(material, 0, .6f);
                    material.Diffuse.Contents = Texture("table-albedo");
                    material.AmbientOcclusion.Contents = UIColor.White;
                    material.Normal.Contents = Texture("table-normal");
                    material.Normal.Intensity = .3f;
                    material.Roughness.Contents = Texture("table-roughness");
                    break;
                default:
                    if(props.RootElement.TryGetProperty(name,out var profile)) ApplyPropMaterial(material,profile,Texture);
                    break;
            }
        }
        ConfigureTableCoordinates();
        AddRealismLamp();
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
