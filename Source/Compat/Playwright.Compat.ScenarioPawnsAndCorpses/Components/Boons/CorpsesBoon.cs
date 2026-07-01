using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Extensions;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using ScenarioPawnsAndCorpses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Compat.ScenarioPawnsAndCorpses.Components.Boons
{
    public class CorpsesBoon : BoonComponent
    {
        public override string Id => "Boons.Compat_ScenarioPawnsAndCorpses_Corpses";

        public List<CorpseEntry> Corpses = new List<CorpseEntry>();

        private IEnumerable<PawnKindDef> PossibleCorpses()
        {
            return DefDatabase<PawnKindDef>.AllDefs;
        }

        protected override void DoSettingsContents(Listing_AutoFitVertical boonContentListing, OriginComponent origin)
        {
            if (boonContentListing.ButtonText("Playwright.Components.Boons.Compat_ScenarioPawnsAndCorpses_Corpses.Add".Translate()))
            {
                Corpses.Add(new CorpseEntry());
                boonContentListing.InvalidateGroup();
                SoundUtils.PlayAdd();
            }

            DoCorpsesContents(boonContentListing);
        }

        private void DoCorpsesContents(Listing_AutoFitVertical boonContentListing)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            foreach (CorpseEntry corpse in Corpses.ToList())
            {
                Rect deleteButtonRect = boonContentListing.GetRect(0f);
                deleteButtonRect.height = 50f;

                Rect rect = boonContentListing.GetRect(35f);
                // UI divided into Count (PawnKind) (ScatterType) each 30% wide, last 10% taken up by (DeleteButton)
                float colWidth = rect.width * 0.3f;

                // Count
                Rect countRect = new Rect(rect);
                countRect.width = colWidth;
                Widgets.TextFieldNumericLabeled(countRect, "Playwright.Count".Translate(), ref corpse.Count, ref corpse.CountBuffer, 1);

                // PawnKind
                Rect pawnKindRect = new Rect(rect);
                pawnKindRect.width = colWidth;
                pawnKindRect.x += colWidth;
                if (Widgets.ButtonText(pawnKindRect, corpse.PawnKindLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (var pawnKindDef in PossibleCorpses())
                    {
                        options.Add(new FloatMenuOption(pawnKindDef.LabelCap, () => corpse.PawnKind = pawnKindDef));
                    }
                    SoundUtils.PlayClick();
                    PlaywrightUtils.OpenFloatMenu(options);
                }

                // ScatterType
                Rect scatterTypeRect = new Rect(rect);
                scatterTypeRect.width = colWidth;
                scatterTypeRect.x += colWidth * 2;
                if (Widgets.ButtonText(scatterTypeRect, corpse.ScatterTypeLabel))
                {
                    var options = new List<FloatMenuOption>()
                    {
                        new FloatMenuOption(ScatterType.Start.Translate(), () => corpse.ScatterType = ScatterType.Start),
                        new FloatMenuOption(ScatterType.NearStart.Translate(), () => corpse.ScatterType = ScatterType.NearStart),
                        new FloatMenuOption(ScatterType.Anywhere.Translate(), () => corpse.ScatterType = ScatterType.Anywhere),
                    };
                    SoundUtils.PlayClick();
                    PlaywrightUtils.OpenFloatMenu(options);
                }

                // Delete button
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    Corpses.Remove(corpse);
                    SoundUtils.PlayRemove();
                    boonContentListing.InvalidateGroup();
                }
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (CorpseEntry corpse in Corpses)
            {
                if (corpse.PawnKind == null)
                {
                    Log.WarningOnce("[Playwright] Corpses boon: item thing was null", 595006);
                    continue;
                }

                ScenPart_CorpseyCorpseyCorpseCorpse part;
                if (corpse.ScatterType == ScatterType.Start)
                {
                    part = (ScenPart_StartingCorpse)ScenarioMaker.MakeScenPart(ScenarioPawnsAndCorpsesDefOf.ScenPart_StartingCorpse);
                }
                else if (corpse.ScatterType == ScatterType.NearStart)
                {
                    part = (ScenPart_ScatteredCorpseNearPlayerStart)ScenarioMaker.MakeScenPart(ScenarioPawnsAndCorpsesDefOf.ScenPart_ScatteredCorpseNearPlayerStart);
                }
                else
                {
                    part = (ScenPart_ScatteredCorpseAnywhere)ScenarioMaker.MakeScenPart(ScenarioPawnsAndCorpsesDefOf.ScenPart_ScatteredCorpseAnywhere);
                }

                FieldInfo pawnKindDefInfo = AccessTools.Field(typeof(ScenPart_CorpseyCorpseyCorpseCorpse), "pawnKindDef");
                pawnKindDefInfo.SetValue(part, corpse.PawnKind);

                FieldInfo countInfo = AccessTools.Field(typeof(ScenPart_CorpseyCorpseyCorpseCorpse), "count");
                countInfo.SetValue(part, corpse.Count);

                scenarioParts.Add(part);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref Corpses, nameof(Corpses), LookMode.Deep);
        }

        public class CorpseEntry : IExposable
        {
            public PawnKindDef PawnKind;
            public int Count = 1;
            public string CountBuffer = "1";
            public ScatterType ScatterType = ScatterType.Start;

            public string PawnKindLabel => PawnKind != null ? PawnKind.LabelCap.ToString() : "Playwright.Components.Boons.Compat_ScenarioPawnsAndCorpses_Corpses.SelectKind".Translate().ToString();
            public string ScatterTypeLabel => ScatterType.Translate();

            public void ExposeData()
            {
                Scribe_Defs.Look(ref PawnKind, nameof(PawnKind));
                Scribe_Values.Look(ref ScatterType, nameof(ScatterType), ScatterType.Start);
                Scribe_Values.Look(ref Count, nameof(Count));
                CountBuffer = Count.ToString();
            }
        }
    }
}
