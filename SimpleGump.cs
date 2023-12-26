using ClassicUO.Game.Managers;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Game.UI.Gumps;
using System;

namespace TUOScripts
{
    public class SimpleGump
    {
        public static void Initialize()
        {
            //Type -simplegump in game to run this command
            CommandManager.Register("simplegump", OnSimpleGumpCommand);
        }

        private static void OnSimpleGumpCommand(string[] obj)
        {
            Gump g = new Gump(0, 0);

            g.Width = 500;
            g.Height = 500;
            g.X = 150;
            g.Y = 150;
            g.CanCloseWithRightClick = true;

            g.Add(new AlphaBlendControl() { Width = g.Width, Height = g.Height });

            g.Add(new GumpPicExternalUrl(10, 10, "https://picsum.photos/480/480", 0, 480, 480));

            UIManager.Add(g);
        }
    }
}
