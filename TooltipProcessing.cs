using ClassicUO.Game.Managers;
using static ClassicUO.Game.Managers.ItemPropertiesData;

namespace TUOScripts
{
    public class TooltipExample
    {
        public static void Initialize()
        {
            EventSink.PreProcessTooltip += PreProcessTooltip;
            EventSink.PostProcessTooltip += PostProcess;
        }

        private static void PostProcess(ref string e)
        {
            e += "(END OF ITEM)";
        }

        private static void PreProcessTooltip(ref ItemPropertiesData e)
        {
            e.Name = "~~ITEM~~";
            foreach(SinglePropertyData line in e.singlePropertyData)
            {
                if (line != null)
                {
                    line.OriginalString = "-BLEH-";
                }
            }
        }
    }
}
