using Otter;

using OtterTutorial;
using OtterTutorial.Scenes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtterTutorial
{
    public class Program
    {
        static void Main(string[] args)
        {
            Global.TUTORIAL = new Game("OtterTutorial", 640, 480);
            Global.TUTORIAL.SetWindow(640, 480);
            Global.TUTORIAL.Color = new Otter.Color("202020");

            Global.PlayerSession = Global.TUTORIAL.AddSession("Player");
            Global.PlayerSession.Controller.Start.AddKey(Key.Return);
            Global.PlayerSession.Controller.Up.AddKey(Key.W);
            Global.PlayerSession.Controller.Left.AddKey(Key.A);
            Global.PlayerSession.Controller.Down.AddKey(Key.S);
            Global.PlayerSession.Controller.Right.AddKey(Key.D);
            Global.PlayerSession.Controller.X.AddKey(Key.Left);
            Global.PlayerSession.Controller.A.AddKey(Key.Down);
            Global.PlayerSession.Controller.B.AddKey(Key.Right);
            Global.PlayerSession.Controller.Y.AddKey(Key.Up);

            Global.TUTORIAL.FirstScene = new TitleScene();
            Global.TUTORIAL.Start();
        }
    }
}
