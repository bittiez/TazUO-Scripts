using ClassicUO.Game;
using ClassicUO.Game.Managers;
using System;
using System.Threading.Tasks;

namespace TUOScripts
{
    /// <summary>
    /// Available commands:
    /// -sethealtype bandage/magery
    /// -autoheal (enables or disables healing)
    /// </summary>
    public class AdvancedHealer
    {
        #region Configuration
        private static TimeSpan delay = TimeSpan.FromSeconds(0.2);     //Time to wait in the loop, do not set to 0
        private static HealType healType = HealType.GreaterHealMagery;                //Bandage healing?
        private static int hitsOffset = 10;                                 //Wait until 10 hits are missing until starting to heal
        private static TimeSpan bandageWait = TimeSpan.FromSeconds(12);     //How long to wait before using another bandage
        private static TimeSpan greaterHealWait = TimeSpan.FromSeconds(8);     //How long to wait before using greater heal again
        private static bool enabled = true;
        #endregion

        public static AdvancedHealer Instance { get; private set; }

        private static DateTime nextHeal = DateTime.MinValue;
        private static Macro greaterHealMacro;

        public static void Initialize()
        {
            greaterHealMacro = new Macro("GreatHeal Dummy Macro");
            greaterHealMacro.MoveToBack(new MacroObject(MacroType.CastSpell, MacroSubType.GreaterHeal));
            greaterHealMacro.MoveToBack(new MacroObject(MacroType.WaitForTarget, MacroSubType.MSC_NONE));
            greaterHealMacro.MoveToBack(new MacroObject(MacroType.TargetSelf, MacroSubType.MSC_NONE));

            //Wait for the player to login
            EventSink.OnConnected += EventSink_OnConnected;

            CommandManager.Register("sethealtype", (e) =>
            {
                if (e[1].Equals("band") || e[1].Equals("bandage"))
                {
                    healType = HealType.Bandage;
                    GameActions.Print("Changed to bandage healing.");
                }
                else if (e[1].Equals("mage") || e[1].Equals("magery"))
                {
                    healType = HealType.GreaterHealMagery;
                    GameActions.Print("Changed to magery healing.");
                }
            });

            CommandManager.Register("autoheal", (e) => { enabled = !enabled; });
        }

        private static void EventSink_OnConnected(object sender, EventArgs e)
        {
            //Set the current instance of this class.
            Instance = new AdvancedHealer();
        }

        public AdvancedHealer()
        {
            Task.Factory.StartNew(() => //Run the healer in a seperate thread so the client doesn't freeze.
            {
                while (World.InGame) //While we are ingame we will run this loop
                {
                    if (!enabled)
                    {
                        Task.Delay(delay + delay).Wait();
                        continue;
                    }

                    if (healType == HealType.Bandage)
                    {
                        if (DateTime.Now > nextHeal && World.Player.Hits < World.Player.HitsMax - hitsOffset)
                        {
                            GameActions.BandageSelf(); //Try to use a bandage!
                            nextHeal = DateTime.Now + bandageWait;
                            Task.Delay(bandageWait + TimeSpan.FromMilliseconds(25)).Wait();
                            continue;
                        }
                    }
                    else if (healType == HealType.GreaterHealMagery)
                    {
                        if (DateTime.Now > nextHeal && World.Player.Hits < World.Player.HitsMax - hitsOffset)
                        {
                            MacroManager.TryGetMacroManager()?.SetMacroToExecute(greaterHealMacro.Items as MacroObject);
                            nextHeal = DateTime.Now + greaterHealWait;
                            Task.Delay(greaterHealWait + TimeSpan.FromMilliseconds(25)).Wait();
                            continue;
                        }
                    }
                    Task.Delay(delay).Wait();
                }
            }, TaskCreationOptions.LongRunning);
        }

        public enum HealType
        {
            Bandage,
            GreaterHealMagery
        }
    }
}
