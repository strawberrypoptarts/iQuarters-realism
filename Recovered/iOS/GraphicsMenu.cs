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
    void ShowGraphics()
    {
        if(screen!="main")return;
        Sound("sharedassets0.assets-131.wav");
        var panel=UIAlertController.Create("Graphics",$"Current: {RealismRendering.Quality}\nOverhead lamp: {(RealismRendering.LampEnabled ? "On" : "Off")}\nChanges apply when you start or resume a game.",UIAlertControllerStyle.Alert);
        void Choice(string title,string value)=>panel.AddAction(UIAlertAction.Create(title,UIAlertActionStyle.Default,_=>RealismRendering.Quality=value));
        Choice("Automatic — recommended","auto");
        Choice("Efficient — lighter effects","efficient");
        Choice("Enhanced — full effects","enhanced");
        panel.AddAction(UIAlertAction.Create(RealismRendering.LampEnabled ? "Turn lamp off" : "Turn lamp on",UIAlertActionStyle.Default,_=>RealismRendering.LampEnabled=!RealismRendering.LampEnabled));
        panel.AddAction(UIAlertAction.Create("Done",UIAlertActionStyle.Cancel,null));
        PresentViewController(panel,true,null);
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
