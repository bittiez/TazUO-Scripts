using ClassicUO.Game;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using System.Threading.Tasks;

namespace TUOScripts
{
    public class RequestTarget
    {
        public static void Initialize()
        {
            CommandManager.Register("target", CommandCallback);
        }

        private static void CommandCallback(string[] obj)
        {
            if (ShowTarget())
            {
                GameActions.Print("Target something.");
            }
            else
            {
                GameActions.Print("You are already trying to target something.");
            }
        }

        public static bool ShowTarget()
        {
            if (TargetManager.IsTargeting)
            {
                return false;
            }

            TargetManager.SetTargeting(CursorTarget.Object, CursorType.Target, TargetType.Neutral);

            Task.Factory.StartNew(() =>
            {
                while (TargetManager.IsTargeting)
                {
                    Task.Delay(100).Wait();
                }
                uint lastTargetSerial = TargetManager.LastTargetInfo.Serial;

                Entity e = World.Get(lastTargetSerial);

                if (e != null)
                {
                    e.AddMessage(ClassicUO.Game.Data.MessageType.Regular, "You targeted me!", ClassicUO.Game.Data.TextType.CLIENT);
                }
            });

            return true;
        }
    }
}
