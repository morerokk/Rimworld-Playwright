using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Rokk.Playwright.Components.Boons
{
    public class StartingResearchBoon : BoonComponent
    {
        public override string Id => "Boons.StartingResearch";

        public List<ResearchProjectDef> ResearchProjects = new List<ResearchProjectDef>();

        private IEnumerable<ResearchProjectDef> PossibleResearchProjects(OriginComponent origin)
        {
            // The origin's player faction determines the research projects the player starts with,
            // don't show any projects that would already be researched anyway
            FactionDef playerFactionDef;
            if (origin is CustomOrigin) // Custom origin was maybe a mistake
            {
                var customOrigin = origin as CustomOrigin;
                playerFactionDef = customOrigin.Faction;
            }
            else if (origin.PlayerFaction != null)
            {
                playerFactionDef = origin.PlayerFaction;
            }
            else if (origin.BasedOnScenario != null)
            {
                FieldInfo playerFactionInfo = AccessTools.Field(typeof(Scenario), "playerFaction");
                ScenPart_PlayerFaction playerFactionPart = (ScenPart_PlayerFaction)playerFactionInfo.GetValue(origin.BasedOnScenario.scenario);

                FieldInfo factionDefInfo = AccessTools.Field(typeof(ScenPart_PlayerFaction), "factionDef");
                playerFactionDef = (FactionDef)factionDefInfo.GetValue(playerFactionPart);
            }
            else
            {
                // Well, what can you do, really?
                Log.WarningOnce("[Playwright] Starting research boon: unable to find player faction in origin, falling back on PlayerTribe", 595003);
                playerFactionDef = FactionDefOf.PlayerTribe;
            }

            var result = DefDatabase<ResearchProjectDef>.AllDefs;
            if (playerFactionDef.startingResearchTags == null)
            {
                return result;
            }

            return result.Where(def =>
            {
                if (def.tags == null)
                {
                    return true;
                }
                return !def.tags.Any(tag => playerFactionDef.startingResearchTags.Contains(tag));
            });
        }

        protected override void DoSettingsContents(Listing_AutoFitVertical boonContentListing, OriginComponent origin)
        {
            if (boonContentListing.ButtonText("Playwright.Components.Boons.StartingResearch.Add".Translate()))
            {
                var options = new List<FloatMenuOption>();
                foreach (var researchProjectDef in PossibleResearchProjects(origin))
                {
                    options.Add(new FloatMenuOption(researchProjectDef.LabelCap, () =>
                    {
                        ResearchProjects.Add(researchProjectDef);
                        boonContentListing.InvalidateGroup();
                    }));
                }
                PlaywrightUtils.OpenFloatMenu(options);
                SoundUtils.PlayClick();
            }

            DoResearchProjects(boonContentListing, origin);
        }

        private void DoResearchProjects(Listing_AutoFitVertical boonContentListing, OriginComponent origin)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            foreach (var researchProject in ResearchProjects.ToList())
            {
                Rect rect = boonContentListing.GetRect(35f);
                float colWidth = rect.width * 0.9f;

                // Research project
                Rect researchProjectRect = new Rect(rect);
                researchProjectRect.width = colWidth;
                Widgets.Label(researchProjectRect, researchProject.LabelCap);

                // Delete button
                Rect deleteButtonRect = new Rect(rect)
                {
                    width = rect.width * 0.1f,
                    x = rect.x + colWidth
                };
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    ResearchProjects.Remove(researchProject);
                    SoundUtils.PlayRemove();
                    boonContentListing.InvalidateGroup();
                }
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (var researchProject in ResearchProjects)
            {
                if (researchProject == null)
                {
                    Log.WarningOnce("[Playwright] Starting research boon: research project def was null, skipping", 595004);
                    continue;
                }
                scenarioParts.Add(ScenPartUtility.MakeStartingResearchPart(researchProject));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref ResearchProjects, nameof(ResearchProjects), LookMode.Def);
        }
    }
}
