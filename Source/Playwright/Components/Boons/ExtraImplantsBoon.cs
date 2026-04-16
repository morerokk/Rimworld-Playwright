using RimWorld;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Rokk.Playwright.Components.Boons
{
    public class ExtraImplantsBoon : BoonComponent
    {
        public override string Id => "Boons.ExtraImplants";

        public List<ExtraImplantEntry> Implants = new List<ExtraImplantEntry>();

        private IEnumerable<HediffDef> PossibleHediffs()
        {
            return DefDatabase<HediffDef>.AllDefs
                .Where(hediffDef => hediffDef.countsAsAddedPartOrImplant);
        }

        private List<BodyPartDef> GetBodyPartsForHediff(HediffDef hediff)
        {
            var list = new List<BodyPartDef>();
            if (hediff == null)
            {
                return list;
            }

            RecipeDef recipe = DefDatabase<RecipeDef>.AllDefsListForReading
                .FirstOrDefault(r => r.addsHediff == hediff);
            if (recipe == null)
            {
                return list;
            }

            return recipe.appliedOnFixedBodyParts;
        }

        private bool HasRightSide(BodyPartDef bodyPart)
        {
            if (bodyPart == null)
            {
                return false;
            }

            if (bodyPart.IsMirroredPart)
            {
                return true;
            }

            if (bodyPart.defName == "Hand" || bodyPart.defName == "Leg" || bodyPart.defName == "Eye" || bodyPart.defName == "Ear" || bodyPart.defName == "Lung" || bodyPart.defName == "Kidney" || bodyPart.defName == "Breast" || bodyPart.defName == "Boob") // Lol
            {
                return true;
            }

            return false;
        }

        public override void DoSettingsContents(Listing_AutoFitVertical boonContentListing)
        {
            if (boonContentListing.ButtonText("Playwright.Components.Boons.ExtraImplants.Add".Translate()))
            {
                Implants.Add(new ExtraImplantEntry());
                boonContentListing.InvalidateGroup();
                AddSound();
            }

            DoImplantsContents(boonContentListing);
        }

        private void DoImplantsContents(Listing_AutoFitVertical boonContentListing)
        {
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

            foreach (ExtraImplantEntry implant in Implants.ToList())
            {
                Rect rect = boonContentListing.GetRect(35f);
                // UI divided into (Hediff) (BodyPart) (Side) each 30% wide, last 10% taken up by (DeleteButton)
                float colWidth = rect.width * 0.3f;

                // Hediff
                Rect hediffRect = new Rect(rect);
                hediffRect.width = colWidth;
                if (Widgets.ButtonText(hediffRect, implant.HediffLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (HediffDef hediffDef in PossibleHediffs())
                    {
                        options.Add(new FloatMenuOption(hediffDef.LabelCap, () => implant.Hediff = hediffDef));
                    }
                    ClickSound();
                    PlaywrightUtils.OpenFloatMenu(options);
                }

                // Bodypart (only if the Hediff supports it)
                var bodyParts = GetBodyPartsForHediff(implant.Hediff);
                if (bodyParts.Count > 1)
                {
                    Rect bodyPartRect = new Rect(rect);
                    bodyPartRect.width = colWidth;
                    bodyPartRect.x += colWidth;
                    if (Widgets.ButtonText(bodyPartRect, implant.BodyPartLabel))
                    {
                        var options = new List<FloatMenuOption>();
                        foreach (BodyPartDef bodyPartDef in GetBodyPartsForHediff(implant.Hediff))
                        {
                            options.Add(new FloatMenuOption(bodyPartDef.LabelCap, () => implant.BodyPart = bodyPartDef));
                        }
                        ClickSound();
                        PlaywrightUtils.OpenFloatMenu(options);
                    }
                }
                else
                {
                    implant.BodyPart = bodyParts.FirstOrDefault();
                }

                // Side (only if the Bodypart is mirrored)
                if (HasRightSide(implant.BodyPart))
                {
                    Rect sideRect = new Rect(rect);
                    sideRect.width = colWidth;
                    sideRect.x += colWidth * 2;
                    if (Widgets.ButtonText(sideRect, implant.SideLabel))
                    {
                        var options = new List<FloatMenuOption>()
                        {
                            new FloatMenuOption("Playwright.Components.Boons.ExtraImplants.Side.Left".Translate(), () => implant.Side = ExtraImplantEntry.ImplantSide.Left),
                            new FloatMenuOption("Playwright.Components.Boons.ExtraImplants.Side.Right".Translate(), () => implant.Side = ExtraImplantEntry.ImplantSide.Right),
                            new FloatMenuOption("Playwright.Components.Boons.ExtraImplants.Side.Both".Translate(), () => implant.Side = ExtraImplantEntry.ImplantSide.Both),
                            new FloatMenuOption("Playwright.Components.Boons.ExtraImplants.Side.Random".Translate(), () => implant.Side = ExtraImplantEntry.ImplantSide.Random)
                        };
                        ClickSound();
                        PlaywrightUtils.OpenFloatMenu(options);
                    }
                }
                else
                {
                    implant.Side = ExtraImplantEntry.ImplantSide.Left;
                }

                // Delete button
                Rect deleteButtonRect = new Rect(rect)
                {
                    width = rect.width * 0.1f,
                    x = rect.x + colWidth * 3
                };
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    Implants.Remove(implant);
                    RemoveSound();
                    boonContentListing.InvalidateGroup();
                }
            }
        }

        private void ClickSound()
        {
            SoundDefOf.Click.PlayOneShotOnCamera();
        }

        private void AddSound()
        {
            SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
        }

        private void RemoveSound()
        {
            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (ExtraImplantEntry implant in Implants)
            {
                if (implant.Hediff == null)
                {
                    Log.WarningOnce("[Playwright] Extra implants boon: hediff was null", 595002);
                    continue;
                }

                switch (implant.Side)
                {
                    case ExtraImplantEntry.ImplantSide.Left:
                        scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(implant.Hediff, 1f, PawnGenerationContext.PlayerStarter, implant.BodyPart, ForcedImplant.ImplantSide.Left));
                        break;
                    case ExtraImplantEntry.ImplantSide.Right:
                        scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(implant.Hediff, 1f, PawnGenerationContext.PlayerStarter, implant.BodyPart, ForcedImplant.ImplantSide.Right));
                        break;
                    case ExtraImplantEntry.ImplantSide.Random:
                        scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(implant.Hediff, 1f, PawnGenerationContext.PlayerStarter, implant.BodyPart, ForcedImplant.ImplantSide.Random));
                        break;
                    case ExtraImplantEntry.ImplantSide.Both:
                        scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(implant.Hediff, 1f, PawnGenerationContext.PlayerStarter, implant.BodyPart, ForcedImplant.ImplantSide.Left));
                        scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(implant.Hediff, 1f, PawnGenerationContext.PlayerStarter, implant.BodyPart, ForcedImplant.ImplantSide.Right));
                        break;
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref Implants, nameof(Implants), LookMode.Deep);
        }

        public class ExtraImplantEntry : IExposable
        {
            public HediffDef Hediff;
            public BodyPartDef BodyPart;
            public ImplantSide Side;

            public string HediffLabel => Hediff != null ? Hediff.LabelCap.ToString() : "Playwright.Components.Boons.ExtraImplants.SelectHediff".Translate().ToString();
            public string BodyPartLabel => BodyPart != null ? BodyPart.LabelCap.ToString() : "Playwright.Components.Boons.ExtraImplants.SelectBodyPart".Translate().ToString();
            public string SideLabel => $"Playwright.Components.Boons.ExtraImplants.Side.{Side}".Translate();

            public void ExposeData()
            {
                Scribe_Defs.Look(ref Hediff, nameof(Hediff));
                Scribe_Defs.Look(ref BodyPart, nameof(BodyPart));
                Scribe_Values.Look(ref Side, nameof(Side));
            }

            public enum ImplantSide
            {
                Left = 0,
                Right = 1,
                Random = 2,
                Both = 3
            }
        }
    }
}
