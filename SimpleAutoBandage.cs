using ClassicUO.Game;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using System;

namespace TUOScripts
{
    public class PlayerDamaged
    {
        public static void Initialize()
        {
            //Lets call the EventSink_OnPlayerStatChange method when a player stat changes
            EventSink.OnPlayerStatChange += EventSink_OnPlayerStatChange;

            CommandManager.Register("autobandage", (e) =>
            {
                //Set enabled to the opposite of what it was. enabled is a bool(true/false)
                enabled = !enabled;
            });
        }

        private static void EventSink_OnPlayerStatChange(object sender, PlayerStatChangedArgs e)
        {
            //If it's not enabled OR the stat that changed was not hits, don't run the code.
            if (!enabled || e.Stat != PlayerStatChangedArgs.PlayerStat.Hits)
            {
                return;
            }

            //Double check the sender is a player and that player is you (World.Player is you)
            if (sender is PlayerMobile player && player == World.Player && player.Hits < player.HitsMax)
            {

                if (DateTime.Now > nextBandageUse)
                {
                    //Lets try to bandage ourself. The following method will try to find a bandage and use it on yourself.
                    GameActions.BandageSelf();

                    GameActions.MessageOverhead("We just used a bandage!", 16, player);

                    //Wait 12 seconds before we use a bandage again. Without create a new task with a loop, this method will only
                    //  get called again the next time their hits change.
                    nextBandageUse = DateTime.Now + TimeSpan.FromSeconds(12);
                }
            }
        }

        private static DateTime nextBandageUse = DateTime.MinValue;

        private static bool enabled = true;
    }
}
