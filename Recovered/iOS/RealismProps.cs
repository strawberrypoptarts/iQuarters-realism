using SceneKit;
using System.Text.Json;
namespace IQuarters.iOS;
public sealed partial class LegacyScene
{
    static void ApplyPropMaterial(SCNMaterial material,JsonElement profile,Func<string,UIImage> texture)
    {
        Physical(material,profile.GetProperty("metalness").GetSingle(),profile.GetProperty("roughness").GetSingle());
        material.ClearCoat.Contents=NSNumber.FromFloat(profile.GetProperty("clearCoat").GetSingle());
        material.ClearCoatRoughness.Contents=NSNumber.FromFloat(profile.GetProperty("clearCoatRoughness").GetSingle());
        if(profile.TryGetProperty("albedo",out var albedo))material.Diffuse.Contents=texture(albedo.GetString()!);
        if(profile.TryGetProperty("shader",out var shader))material.ShaderModifiers=new SCNShaderModifiers {EntryPointSurface=shader.GetString()};
        if(profile.TryGetProperty("transparency",out var transparency)){
            material.Transparency=transparency.GetSingle();material.TransparencyMode=SCNTransparencyMode.AOne;
            material.WritesToDepthBuffer=false;material.DoubleSided=true;
        }
    }
}
