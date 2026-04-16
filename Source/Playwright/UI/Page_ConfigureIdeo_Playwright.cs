using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.UI
{
    /// <summary>
    /// A standalone implementation of the ideoligion editor UI that is slightly more aware of the fact that it's not attached to a real game in progress.
    /// </summary>
    public class Page_ConfigureIdeo_Playwright : Page_ConfigureIdeo
    {
        public Page_ConfigureIdeo_Playwright() : base()
        {
            this.doCloseX = true;
            this.grayOutIfOtherDialogOpen = false;
        }

        public override void DoWindowContents(Rect rect)
        {
            base.DrawPageTitle(rect);
            this.DoIdeos(rect);
            if (this.ideo != null)
            {
                string text = null;
                Pair<Precept, Precept> pair = this.ideo.FirstIncompatiblePreceptPair();
                if (pair != default(Pair<Precept, Precept>))
                {
                    string text2 = pair.First.TipLabel;
                    string text3 = pair.Second.TipLabel;
                    if (text2 == text3)
                    {
                        text2 = pair.First.UIInfoSecondLine;
                        text3 = pair.Second.UIInfoSecondLine;
                    }
                    text = "MessageIdeoIncompatiblePrecepts".Translate(text2.Named("PRECEPT1"), text3.Named("PRECEPT2")).CapitalizeFirst();
                }
                else
                {
                    Tuple<Precept_Ritual, List<string>> tuple = this.ideo.FirstRitualMissingTarget();
                    Precept_Building precept_Building = this.ideo.FirstConsumableBuildingMissingRitual();
                    if (tuple != null)
                    {
                        text = "MessageRitualMissingTarget".Translate(tuple.Item1.LabelCap.Named("PRECEPT")) + ": " + tuple.Item2.ToCommaList(false, false).CapitalizeFirst() + ".";
                    }
                    else if (precept_Building != null)
                    {
                        text = "MessageBuildingMissingRitual".Translate(precept_Building.LabelCap.Named("PRECEPT"));
                    }
                }
                Rect rect2 = rect;
                rect2.xMin = rect2.xMax - Page.BottomButSize.x * 2.75f;
                rect2.width = Page.BottomButSize.x * 1.7f;
                rect2.yMin = rect2.yMax - Page.BottomButSize.y;
                Precept precept = this.ideo.FirstPreceptWithWarning();
                if (text != null)
                {
                    GUI.color = Color.red;
                    Text.Font = GameFont.Tiny;
                    Text.Anchor = TextAnchor.UpperRight;
                    Widgets.Label(rect2, text);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = Color.white;
                }
                else if (precept != null)
                {
                    GUI.color = ColorLibrary.Yellow;
                    Text.Font = GameFont.Tiny;
                    Text.Anchor = TextAnchor.UpperRight;
                    string text4;
                    string description;
                    precept.GetPlayerWarning(out text4, out description);
                    text4 = "Warning".Translate() + ": " + text4.CapitalizeFirst();
                    Widgets.Label(rect2, text4);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = Color.white;
                    Widgets.DrawHighlightIfMouseover(rect2);
                    TooltipHandler.TipRegion(rect2, () => description, 37584575);
                }
                else
                {
                    IdeoUIUtility.DrawImpactInfo(rect2, this.ideo.memes);
                }
            }
            DoButtonBar(rect);
        }

        protected virtual void DoButtonBar(Rect rect)
        {
            Rect buttonBarRect = new Rect(rect);
            buttonBarRect.y = rect.y + rect.height - 38f;
            buttonBarRect.height = 38f;

            Rect addButtonRect = new Rect(buttonBarRect);
            addButtonRect.width = 150f;
            if (Widgets.ButtonText(addButtonRect, "Add".Translate().CapitalizeFirst()))
            {
                SelectOrMakeNewIdeo(null);
            }
        }
    }
}
