using ClassicUO.Game;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using System.Collections.Generic;

namespace TUOScripts
{
    public class ItemSearch
    {
        public static void Initialize()
        {
            CommandManager.Register("isearch", OnISearch);
        }

        private static void OnISearch(string[] args)
        {
            //Could use args here if needed


            var backpack = World.Player.FindItemByLayer(ClassicUO.Game.Data.Layer.Backpack);

            var items = new List<Item>(World.Items.Values);
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];

                if (item != null && (item.Container == backpack || item.RootContainer == backpack)) //Item isn't null and is somewhere in players backpack
                {
                    if (World.OPL.TryGetNameAndData(item, out string itemName, out string itemProperties))
                    {
                        if (itemName.ToLower().Contains("runebook") || item.ItemData.Name.ToLower().Contains("runebook") || item.Name.ToLower().Contains("runebook"))
                        {
                            GameActions.Print("Found a runebook somewhere in your backpack");
                        }
                    }
                }
            }
        }
    }
}
