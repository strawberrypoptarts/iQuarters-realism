using CoreGraphics;
using SceneKit;

namespace IQuarters.iOS;

public sealed partial class MainMenuController
{
    UIButton graphicsButton=null!;
    void CreateGraphicsButton()
    {
        graphicsButton=new UIButton(UIButtonType.Custom){Hidden=true,AccessibilityLabel="Graphics"};
        graphicsButton.SetBackgroundImage(GraphicsButtonImage(),UIControlState.Normal);
        graphicsButton.TouchUpInside+=(_,_)=>ShowGraphics();
        View!.AddSubview(graphicsButton);
    }
    void PlaceMainButtons()
    {
        // Make room for the third row within the original portrait composition.
        // Intro animation re-establishes authored positions before each home visit.
        foreach(int id in new[]{222,225}){
            var node=legacy.Nodes[id];var p=node.WorldPosition;p.Y+=20;node.WorldPosition=p;
        }
        LayoutGraphicsButton();
    }
    void LayoutGraphicsButton()
    {
        if(graphicsButton==null||display==null)return;
        graphicsButton.Hidden=screen!="main";
        if(graphicsButton.Hidden)return;
        var n=legacy.Nodes[225];SCNVector3 min=default,max=default;
        n.GetBoundingBox(ref min,ref max);
        var points=new List<SCNVector3>();
        foreach(float x in new[]{min.X,max.X})foreach(float y in new[]{min.Y,max.Y})foreach(float z in new[]{min.Z,max.Z})
            points.Add(display.ProjectPoint(n.ConvertPositionToNode(new(x,y,z),null)));
        float left=points.Min(p=>p.X),right=points.Max(p=>p.X),top=points.Min(p=>p.Y),bottom=points.Max(p=>p.Y);
        right=Math.Min(right,(float)View!.Bounds.Width-(float)View.SafeAreaInsets.Right);
        float height=bottom-top;
        graphicsButton.Frame=new CGRect(left,bottom+height*.12f,right-left,height);
    }
    readonly Dictionary<int,SCNGeometry> graphicsOriginals=new();
    readonly Dictionary<string,UIImage> graphicsImages=new();
    void ShowGraphics()
    {
        if(screen!="main")return;
        Sound("sharedassets0.assets-131.wav");
        legacy.Find("ui_about")!.Hidden=true;
        Sequence(["hiscoreclick","playnowout"],()=>{
            foreach(int id in new[]{210,213,216,219}) {
                var node=legacy.Nodes[id];graphicsOriginals[id]=node.Geometry!;
                node.Geometry=(SCNGeometry)node.Geometry!.Copy();
                node.Geometry.Materials=node.Geometry.Materials.Select(m=>(SCNMaterial)m.Copy()).ToArray();
            }
            RefreshGraphicsLabels();
            Sequence(["npin"],()=>screen="graphics");
        });
    }
    void RefreshGraphicsLabels()
    {
        string quality=RealismRendering.Quality switch {"efficient"=>"EFFICIENT","enhanced"=>"ENHANCED",_=>"AUTO"};
        string aa=RealismRendering.Antialiasing.ToUpperInvariant();
        foreach(var (id,label) in new[]{(210,"QUALITY\n"+quality),(213,"ANTIALIAS\n"+aa),(216,"LAMP\n"+(RealismRendering.LampEnabled?"ON":"OFF")),(219,"DONE")}) {
            if(!graphicsImages.TryGetValue(label,out var image))graphicsImages[label]=image=GraphicsSettingImage(label);
            legacy.Nodes[id].Geometry!.FirstMaterial!.Diffuse.Contents=image;
        }
    }
    void GraphicsTap(string name)
    {
        if(name is "button_button_left" or "button_right_bk" or "button_player_04") {
            Sequence([name=="button_player_04"?"npfourplayer":"npbackclick","npout"],()=>{
                foreach(var (id,geometry) in graphicsOriginals)legacy.Nodes[id].Geometry=geometry;
                graphicsOriginals.Clear();RealismRendering.ConfigureAntialiasing(display);Home();
            });return;
        }
        string clip;
        if(name=="button_player_01") {
            RealismRendering.Quality=RealismRendering.Quality switch {"auto"=>"efficient","efficient"=>"enhanced",_=>"auto"};clip="nponeplayer";
        }else if(name=="button_player_02") {
            RealismRendering.Antialiasing=RealismRendering.Antialiasing switch {"auto"=>"off","off"=>"2x","2x"=>"4x",_=>"auto"};clip="nptwoplayer";
        }else if(name=="button_player_03") {RealismRendering.LampEnabled=!RealismRendering.LampEnabled;clip="npthreeplayer";}
        else return;
        RefreshGraphicsLabels();Sequence([clip],()=>screen="graphics");
    }
    static UIImage GraphicsSettingImage(string label)
    {
        using var original=UIImage.FromFile(LegacyScene.Resource("textures/sharedassets0.assets-8.png"))!;
        using var renderer=new UIGraphicsImageRenderer(new CGSize(256,64));
        return renderer.CreateImage(_=>{
            original.Draw(new CGRect(0,0,256,64));
            // Extend the original blank green fill over the player label; preserve its bevel.
            using var crop=original.CGImage!.WithImageInRect(new CGRect(20,18,2,29));
            using var fill=UIImage.FromImage(crop!);fill.Draw(new CGRect(18,18,218,29));
            using var text=new NSString(label);
            var style=new NSMutableParagraphStyle {Alignment=UITextAlignment.Right};
            var attrs=new UIStringAttributes {Font=UIFont.FromName("Arial-BoldMT",label.Contains('\n')?13:19)!,ForegroundColor=UIColor.White,
                StrokeColor=UIColor.Black,StrokeWidth=-5,ParagraphStyle=style};
            text.DrawString(new CGRect(120,label.Contains('\n')?17:21,110,34),attrs);
        });
    }
    static UIImage GraphicsButtonImage()
    {
        // Assemble the UI from the original button's bevel and original letter
        // sprites, so the new label uses the game's actual type, not a substitute font.
        using var high=UIImage.FromFile(LegacyScene.Resource("textures/sharedassets0.assets-26.png"))!;
        using var play=UIImage.FromFile(LegacyScene.Resource("textures/sharedassets0.assets-15.png"))!;
        using var renderer=new UIGraphicsImageRenderer(new CGSize(256,64));
        return renderer.CreateImage(_=>{
            void Piece(UIImage image,CGRect source,CGRect target){
                using var cg=image.CGImage!.WithImageInRect(source);
                using var part=UIImage.FromImage(cg!);part.Draw(target);
            }
            high.Draw(new CGRect(0,0,256,64));
            Piece(high,new(210,18,2,24),new(19,18,186,24));
            // G R A P H I C S. PLAY NOW glyphs are slightly taller, so normalize them.
            var letters=new (UIImage image,CGRect rect)[]{
                (high,new(48,21,18,18)),(high,new(151,21,17,18)),
                (play,new(98,19,20,22)),(play,new(57,19,21,22)),
                (high,new(21,21,17,18)),(high,new(40,21,7,18)),
                (high,new(113,21,16,18)),(high,new(95,21,17,18))};
            double width=letters.Sum(l=>(double)l.rect.Width*18/(double)l.rect.Height)+7*2;
            double x=(256-width)/2;
            foreach(var letter in letters){double w=(double)letter.rect.Width*18/(double)letter.rect.Height;Piece(letter.image,letter.rect,new(x,21,w,18));x+=w+2;}
        });
    }
}
