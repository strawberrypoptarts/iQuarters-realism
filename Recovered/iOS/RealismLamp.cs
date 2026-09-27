using SceneKit;
using CoreGraphics;

namespace IQuarters.iOS;
public sealed partial class LegacyScene
{
    public bool HasRealismLamp {get;private set;}
    void ConfigureTableCoordinates()
    {
        var table=Find("table_00");if(table?.Geometry==null)return;
        var mesh=assets.Read("meshes/sharedassets1.assets-238.json");
        var positions=mesh.GetProperty("vertices").EnumerateArray().Select(v=>new[]{v[0].GetSingle(),v[1].GetSingle()}).ToArray();
        float minX=positions.Min(v=>v[0]),maxX=positions.Max(v=>v[0]),minY=positions.Min(v=>v[1]),maxY=positions.Max(v=>v[1]);
        var uv=positions.Select(v=>new CGPoint((v[0]-minX)/(maxX-minX),1-(v[1]-minY)/(maxY-minY))).ToArray();
        var old=table.Geometry;
        var textureSource=SCNGeometrySource.FromTextureCoordinates(uv);
        var sources=old.GeometrySources.Where(s=>!s.Semantic.Equals(textureSource.Semantic)).Append(textureSource).ToArray();
        var geometry=SCNGeometry.Create(sources,old.GeometryElements);geometry.Materials=old.Materials;table.Geometry=geometry;
    }
    void AddRealismLamp()
    {
        HasRealismLamp=RealismRendering.LampEnabled;
        if(!HasRealismLamp)return;
        // Transparent legacy glass shells must not cast solid black silhouettes.
        // Their authored contact shadows remain; opaque props and the coin use the live shadow map.
        foreach(var node in Nodes.Values){
            var mats=node.Geometry?.Materials;
            node.CastsShadow=mats!=null&&mats.Any(m=>m.LightingModelName==SCNLightingModel.PhysicallyBased&&m.WritesToDepthBuffer);
        }
        var lamp=new SCNNode {Name="Realism overhead lamp",Position=new(-2,5.5f,1)};
        Scene.RootNode.AddChildNode(lamp);
        var metal=new SCNMaterial{LightingModelName=SCNLightingModel.PhysicallyBased,DoubleSided=true};
        metal.Diffuse.Contents=UIColor.FromRGB(.12f,.16f,.12f);metal.Metalness.Contents=NSNumber.FromFloat(.75f);metal.Roughness.Contents=NSNumber.FromFloat(.28f);
        var shade=SCNCone.Create(.16f,.55f,.45f);shade.FirstMaterial=metal;
        lamp.AddChildNode(new SCNNode{Geometry=shade,CastsShadow=false});
        var bulbMaterial=new SCNMaterial{LightingModelName=SCNLightingModel.Constant};bulbMaterial.Diffuse.Contents=UIColor.FromRGB(1f,.86f,.62f);
        var diffuser=SCNCylinder.Create(.46f,.025f);diffuser.FirstMaterial=bulbMaterial;
        lamp.AddChildNode(new SCNNode{Geometry=diffuser,Position=new(0,-.23f,0),CastsShadow=false});
        var cable=SCNCylinder.Create(.015f,3);cable.FirstMaterial=metal;
        lamp.AddChildNode(new SCNNode{Geometry=cable,Position=new(0,1.7f,0),CastsShadow=false});
        bool enhanced=RealismRendering.Enhanced;
        var light=new SCNLight {
            LightType=SCNLightType.Spot,Color=UIColor.FromRGB(1f,.91f,.78f),Intensity=650,
            CategoryBitMask=0xf00,SpotInnerAngle=45,SpotOuterAngle=100,
            AttenuationStartDistance=0,AttenuationEndDistance=18,AttenuationFalloffExponent=2,
            CastsShadow=true,ShadowMode=SCNShadowMode.Forward,ShadowColor=UIColor.FromWhiteAlpha(0,.65f),
            ShadowMapSize=new CGSize(enhanced?2048:1024,enhanced?2048:1024),
            ShadowSampleCount=enhanced?16u:8u,ShadowRadius=enhanced?3:2,ShadowBias=.003f,ZNear=.1f,ZFar=20
        };
        var source=new SCNNode{Name="Realism soft spotlight",Light=light,Position=new(0,-.3f,0)};
        lamp.AddChildNode(source);source.Look(new SCNVector3(0,0,0));
    }
}
