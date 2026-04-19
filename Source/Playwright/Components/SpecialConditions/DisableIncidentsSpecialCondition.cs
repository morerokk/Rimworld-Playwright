using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class DisableIncidentsSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.DisableIncidents";

        public List<IncidentDef> DisabledIncidents = new List<IncidentDef>();

        private IEnumerable<IncidentDef> GetAllowedIncidents()
        {
            return DefDatabase<IncidentDef>.AllDefs
                // Some defs can already be disabled through other win/special conditions specifically, don't need them here.
                // Journey offer can still be disabled specifically, for if the player wants to disable the journey offer but build the ship themselves.
                .Where(def =>
                    def != DefOfs.IncidentDefOf.StrangerInBlackJoin
                    && def != DefOfs.IncidentDefOf.GiveQuest_EndGame_ArchonexusVictory
                    && def != DefOfs.IncidentDefOf.GiveQuest_EndGame_RoyalAscent
                    && def != DefOfs.IncidentDefOf.GiveQuest_MechanoidSignal)
                .Where(def => !DisabledIncidents.Contains(def));
        }

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            if (specialConditionContentListing.ButtonText("Playwright.Components.SpecialConditions.DisableIncidents.Add".Translate()))
            {
                var options = new List<FloatMenuOption>();

                foreach (IncidentDef incidentDef in GetAllowedIncidents())
                {
                    options.Add(new FloatMenuOption(incidentDef.LabelCap, () =>
                    {
                        DisabledIncidents.Add(incidentDef);
                        specialConditionContentListing.InvalidateGroup();
                    }));
                }
                PlaywrightUtils.OpenFloatMenu(options);
                SoundUtils.PlayClick();
            }

            DoIncidentsContents(specialConditionContentListing);
        }

        private void DoIncidentsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            foreach (IncidentDef incidentDef in DisabledIncidents.ToList())
            {
                Rect rowRect = specialConditionContentListing.GetRect(25f);
                Rect incidentLabelRect = new Rect(rowRect);
                incidentLabelRect.width *= 0.9f;
                Widgets.Label(incidentLabelRect, incidentDef.LabelCap);

                Rect deleteButtonRect = new Rect(rowRect);
                deleteButtonRect.width *= 0.1f;
                deleteButtonRect.x += incidentLabelRect.width;
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    DisabledIncidents.Remove(incidentDef);
                    specialConditionContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (var incidentDef in DisabledIncidents)
            {
                if (incidentDef == null)
                {
                    continue;
                }
                scenarioParts.Add(ScenPartUtility.MakeDisableIncidentPart(incidentDef));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref DisabledIncidents, nameof(DisabledIncidents), LookMode.Def);
        }
    }
}
