using ClassicUO.Game;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Game.UI.Gumps;
using System.Linq;

// This requires v3.21 + of the TazUO client

// This is an example of custom sorting/displaying items in a grid container
// You can use -listgrids to list serial numbers for open grid containers
// Then use -sortgrid SERIAL to sort that container with this custom code.
// This is meant as an example to expand on, you could add gumps or ask the user for a target to target a container, etc

namespace TUOScripts
{
    public class CustomGridSorting
    {
        public static void Initialize()
        {
            CommandManager.Register("listgrids", ListGridsCommand);
            CommandManager.Register("sortgrid", SortGridCommand);
        }

        public static void ListGridsCommand(string[] args)
        {
            foreach (GridContainer c in UIManager.Gumps.OfType<GridContainer>())
            {
                GameActions.Print($"{c.LocalSerial}");
            }
        }

        public static void SortGridCommand(string[] args)
        {
            //args[0] is the command itself
            if (args.Length > 1)
            {
                //Arg 1 should be the grid serial from listgrids command
                if (uint.TryParse(args[1], out var gridId))
                {
                    //Try to get a gump with that serial
                    Gump g = UIManager.GetGump(gridId);

                    //If it's not null, there is a gump with that serial. Make sure it's a GridContainer
                    if (g != null && g is GridContainer gridContainer)
                    {
                        //Skip saving this container so you don't override item positions as everything not set here will be reset otherwise.
                        gridContainer.SkipSave = true;

                        //Clear all slots
                        foreach (var slot in gridContainer.GetGridSlotManager.GridSlots)
                        {
                            slot.Value.SetGridItem(null);
                        }

                        //Keep track of what slot we are on
                        int slotCount = 0;

                        var contents = gridContainer.GetGridSlotManager.ContainerContents;
                        contents.OrderBy(i => i.Graphic).ThenBy(i => i.Hue); //Sort by graphic, then hue

                        //For each item in the container
                        foreach (Item item in contents)
                        {
                            if (item != null)
                            {
                                if (World.OPL.Contains(item) && World.OPL.TryGetNameAndData(item, out string name, out string props))
                                {
                                    //Here we have the items name and properties(tooltip)
                                    if ((!string.IsNullOrEmpty(props) && props.ToLower().Contains("ingot")) || name.ToLower().Contains("ingot"))
                                    {
                                        //We are only adding items with ingots back into the grid slots, so these will be the only items to show up
                                        gridContainer.GetGridSlotManager.GridSlots[slotCount].SetGridItem(item);
                                        slotCount++;
                                    }
                                }
                            }
                        }

                        //Apply the new GridSlots list to the visual UI container
                        gridContainer.GetGridSlotManager.SetGridPositions();
                    }
                }
            }
        }
    }
}
