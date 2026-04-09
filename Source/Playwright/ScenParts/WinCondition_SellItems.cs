using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    public class WinCondition_SellItems : ScenPart_WinCondition
    {
        public ThingDef Thing;
        public int Amount;
        private string AmountBuffer;

        private string ThingLabel => Thing != null ? Thing.LabelCap.ToString() : "-";

        private IEnumerable<ThingDef> PossibleThingDefs()
        {
            return DefDatabase<ThingDef>.AllDefs
                .Where((ThingDef d) => d.category == ThingCategory.Item || d.category == ThingCategory.Pawn);
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 4);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 4);

            if (Widgets.ButtonText(helper.NextRect(), ThingLabel))
            {
                var options = new List<FloatMenuOption>();
                foreach (ThingDef thing in PossibleThingDefs())
                {
                    options.Add(new FloatMenuOption(thing.LabelCap, () => Thing = thing));
                }
                PlaywrightUtils.OpenFloatMenu(options);
            }

            Widgets.IntEntry(helper.NextRect(), ref Amount, ref AmountBuffer);
            if (Amount < 1)
            {
                Amount = 1;
                AmountBuffer = "1";
            }

            helper.Skip(1);

            if (Widgets.ButtonText(helper.NextRect(), "?"))
            {
                DoHelpButton();
            }
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == SummaryTag)
            {
                yield return "Playwright.ScenParts.WinCondition_SellThing.Summary".Translate();
            }
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.WinCondition_SellThing.Help".Translate()));
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            WinCondition_SellItems part = other as WinCondition_SellItems;
            return part == null || part.Thing != this.Thing;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Thing, nameof(Thing));
            Scribe_Values.Look(ref Amount, nameof(Amount));
        }
    }
}
