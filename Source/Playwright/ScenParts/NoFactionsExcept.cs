using RimWorld;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    public abstract class NoFactionsExcept : ScenPart
    {
        public List<FactionDef> FactionsToAffect = new List<FactionDef>();
        public abstract FactionRelationKind RelationKind { get; }
        protected abstract string SummaryIntro { get; }
        protected abstract string SummaryTag { get; }
        protected abstract string SummaryNoIntro { get; }
        protected abstract string HelpText { get; }

        protected virtual List<FactionDef> GetAllowedFactions()
        {
            return DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer)
                .ToList();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

            int rows = FactionsToAffect.Count + 4;
            Rect scenPartRect = listing.GetScenPartRect(this, RowHeight * rows);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, rows);

            // Faction selector
            if (Widgets.ButtonText(helper.NextRect(), "Playwright.ScenParts.NoFactionsExcept.Add".Translate()))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<FactionDef> allowedFactions = GetAllowedFactions();
                foreach (FactionDef factionDef in allowedFactions)
                {
                    floatMenuOptions.Add(new FloatMenuOption(factionDef.LabelCap, () => FactionsToAffect.Add(factionDef)));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }

            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.NoFactionsExcept.Factions".Translate());
            foreach (FactionDef factionDef in FactionsToAffect.ToList())
            {
                Rect rect = helper.NextRect();
                if (helper.DrawListItemWithButton(rect, factionDef.LabelCap, deleteTex, SoundDefOf.Checkbox_TurnedOff, 0.4f))
                {
                    FactionsToAffect.Remove(factionDef);
                }
            }

            helper.Skip(1);

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                DoHelpButton();
            }
        }

        public override void PostWorldGenerate()
        {
            // TODO: Remove all factions that aren't allowed
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look<FactionDef>(ref FactionsToAffect, nameof(FactionsToAffect), LookMode.Def);

            base.ExposeData();
        }

        public override string Summary(Scenario scen)
        {
            if (FactionsToAffect.Count == 0)
            {
                return SummaryNoIntro;
            }
            return ScenSummaryList.SummaryWithList(scen, SummaryTag, SummaryIntro);
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == SummaryTag)
            {
                foreach (FactionDef factionDef in FactionsToAffect)
                {
                    yield return factionDef.LabelCap;
                }
            }
            yield break;
        }

        protected virtual void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow(HelpText));
        }
    }
}
