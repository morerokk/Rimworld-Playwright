using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class XenotypeSwapSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.XenotypeSwap";
        public override bool IsAvailable => ModsConfig.BiotechActive;

        public List<XenotypeReplacement> XenotypeReplacements = new List<XenotypeReplacement>();

        private string GetXenotypeLabel(XenotypeDef xenotypeDef)
        {
            return xenotypeDef != null ? xenotypeDef.LabelCap.ToString() : "-";
        }

        private List<XenotypeDef> GetSelectableFromXenotypes(XenotypeReplacement currentReplacement)
        {
            // Only allow a xenotype to be selected if it's not already in use,
            // unless the current row is the one that's using it
            List<XenotypeDef> otherReplacementTakenXenotypeDefs = XenotypeReplacements
                .Where(x => x != currentReplacement && x.From != null)
                .Select(x => x.From)
                .ToList();

            return DefDatabase<XenotypeDef>.AllDefsListForReading
                .Where(def => !otherReplacementTakenXenotypeDefs.Contains(def))
                .ToList();
        }

        private List<XenotypeDef> GetSelectableToXenotypes(XenotypeReplacement currentReplacement)
        {
            // Allow all xenotypes to be selected, except the one already present in From
            if (currentReplacement.From == null)
            {
                return DefDatabase<XenotypeDef>.AllDefsListForReading;
            }
            return DefDatabase<XenotypeDef>.AllDefsListForReading
                .Where(def => def != currentReplacement.From)
                .ToList();
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (XenotypeReplacement replacement in XenotypeReplacements)
            {
                if (replacement.From == null || replacement.To == null)
                {
                    Log.Warning("[Playwright] XenotypeReplacement had a null From/To entry, skipping");
                    continue;
                }
                scenarioParts.Add(ScenPartUtility.MakeReplaceXenotypePart(replacement.From, replacement.To, 1f, PawnGenerationContext.All));
            }
        }

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

            if (specialConditionContentListing.ButtonText("Playwright.Components.SpecialConditions.XenotypeSwap.Add".Translate()))
            {
                XenotypeReplacements.Add(new XenotypeReplacement());
                specialConditionContentListing.Invalidate();
                AddSound();
            }

            foreach(var replacement in XenotypeReplacements.ToList())
            {
                Rect rect = specialConditionContentListing.GetRect(35f);
                // UI divided into (Button) (Text) (Button) each 30% wide, last 10% taken up by (DeleteButton)
                float colWidth = rect.width * 0.3f;
                // From
                Rect fromButtonRect = new Rect(rect)
                {
                    width = colWidth
                };
                if (Widgets.ButtonText(fromButtonRect, GetXenotypeLabel(replacement.From)))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (var xenotypeDef in GetSelectableFromXenotypes(replacement))
                    {
                        options.Add(new FloatMenuOption(xenotypeDef.LabelCap, () => { replacement.From = xenotypeDef; specialConditionContentListing.Invalidate(); }));
                    }
                    Find.WindowStack.Add(new FloatMenu(options));
                    ClickSound();
                }
                // Replacement text
                Rect replacesRect = new Rect(rect)
                {
                    width = colWidth,
                    x = rect.x + colWidth
                };
                string replacesText = "Playwright.Components.SpecialConditions.XenotypeSwap.ReplacedBy".Translate();
                Rect replacesRectCentered = PlaywrightDrawHelper.GetCenteredLabelRect(replacesRect, replacesText);
                Widgets.Label(replacesRectCentered, replacesText);
                // To
                Rect toButtonRect = new Rect(rect)
                {
                    width = colWidth,
                    x = rect.x + colWidth * 2
                };
                if (Widgets.ButtonText(toButtonRect, GetXenotypeLabel(replacement.To)))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (var xenotypeDef in GetSelectableToXenotypes(replacement))
                    {
                        options.Add(new FloatMenuOption(xenotypeDef.LabelCap, () => { replacement.To = xenotypeDef; specialConditionContentListing.Invalidate(); }));
                    }
                    Find.WindowStack.Add(new FloatMenu(options));
                    ClickSound();
                }
                // Delete button
                Rect deleteButtonRect = new Rect(rect)
                {
                    width = rect.width * 0.1f,
                    x = rect.x + colWidth * 3
                };
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    XenotypeReplacements.Remove(replacement);
                    RemoveSound();
                }

                if (replacement.From == replacement.To)
                {
                    replacement.From = null;
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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref XenotypeReplacements, nameof(XenotypeReplacements), LookMode.Deep);
        }

        // Read-write tuple that's also IExposable
        public class XenotypeReplacement : IExposable
        {
            public XenotypeDef From;
            public XenotypeDef To;

            public void ExposeData()
            {
                Scribe_Defs.Look(ref From, nameof(From));
                Scribe_Defs.Look(ref To, nameof(To));
            }
        }
    }
}
