using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    public abstract class FactionGoodwill : ScenPart
    {
        public FactionDef FactionToAffect;

        public int Goodwill = 0;

        protected virtual int MinGoodwill => -100;
        protected virtual int MaxGoodwill => 100;

        protected string FactionToAffectLabelText => FactionToAffect != null ? FactionToAffect.LabelCap.ToString() : "-";

        protected virtual IEnumerable<FactionDef> GetAllowedFactions()
        {
            return FactionUtils.GetAllNpcFactions()
                .Where(def => !def.permanentEnemy);
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 5);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 5);

            // Faction selector
            if (Widgets.ButtonText(helper.NextRect(), FactionToAffectLabelText))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                var allowedFactions = GetAllowedFactions();
                foreach (FactionDef factionDef in allowedFactions)
                {
                    floatMenuOptions.Add(new FloatMenuOption(factionDef.LabelCap, () => FactionToAffect = factionDef, factionDef.FactionIcon, factionDef.DefaultColor));
                }
                PlaywrightUtils.OpenFloatMenu(floatMenuOptions);
            }

            helper.Skip(1);

            // Goodwill slider
            Goodwill = Mathf.FloorToInt(Widgets.HorizontalSlider(helper.NextRect(), Goodwill, MinGoodwill, MaxGoodwill, true, "Playwright.ScenParts.FactionGoodwill.Goodwill".Translate(), null, null, 1f));
            Widgets.Label(helper.NextRect(), Goodwill.ToString());

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                DoHelpButton();
            }
        }

        // Modify faction starting goodwill on world gen
        public override void PostWorldGenerate()
        {
            int targetGoodwill = Math.Clamp(Goodwill, -100, 100);

            List<Faction> factions = Find.FactionManager.AllFactionsListForReading
                .Where(f => f.def == FactionToAffect)
                .ToList();

            foreach (var faction in factions)
            {
                int currentPlayerGoodwill = faction.GoodwillWith(Faction.OfPlayer);
                int differenceToReachTarget = Goodwill - currentPlayerGoodwill;
                faction.TryAffectGoodwillWith(Faction.OfPlayer, differenceToReachTarget, false, false);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref FactionToAffect, nameof(FactionToAffect));
            Scribe_Values.Look(ref Goodwill, nameof(Goodwill), 0, false);
        }

        public abstract override string Summary(Scenario scen);
        protected abstract void DoHelpButton();
    }
}
