using RimWorld;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Composer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace Rokk.Playwright.UI
{
    public class PlaywrightWindow : Window
    {
        private PlaywrightStructure PlaywrightStructure = PlaywrightStructure.CreateDefault();
        private Tabs ActiveTab = Tabs.Intro;

        private int PanelOutlineWidth => 1;
        private Color PanelOutlineColor => Widgets.SeparatorLineColor;
        private Color PanelBGColor => Widgets.MenuSectionBGFillColor;
        private float PanelContentMargin => 5f;

        private float OptionHeight => 50f;
        private float OptionContentMargin => 5f;

        public override Vector2 InitialSize => new Vector2(1200, 800);

        public PlaywrightWindow() : base()
        {
            this.closeOnClickedOutside = false;
            this.resizeable = false;
            this.optionalTitle = "Playwright.WindowTitle".Translate();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect tabPanelRect = new Rect(inRect);
            tabPanelRect.width /= 4;

            DrawTabPanel(tabPanelRect);

            Rect tabContentRect = new Rect(inRect);
            tabContentRect.width -= tabPanelRect.width + Margin;
            tabContentRect.x += tabPanelRect.width + Margin;

            // Content panel background
            Widgets.DrawBoxSolidWithOutline(tabContentRect, PanelBGColor, PanelOutlineColor, PanelOutlineWidth);
            tabContentRect = PlaywrightDrawHelper.RectWithMargin(tabContentRect, PanelContentMargin);

            switch (ActiveTab)
            {
                case Tabs.Intro:
                    DrawIntro(tabContentRect);
                    break;
                case Tabs.Origin:
                    DrawOrigin(tabContentRect);
                    break;
                case Tabs.Boons:
                    DrawBoons(tabContentRect);
                    break;
            }
        }

        private void DrawTabPanel(Rect tabPanelRect)
        {
            float currentY = tabPanelRect.y;
            foreach (Tabs value in Enum.GetValues(typeof(Tabs)))
            {
                var buttonRect = new Rect(tabPanelRect);
                buttonRect.height = OptionHeight;
                buttonRect.y = currentY;
                Widgets.DrawOptionBackground(buttonRect, ActiveTab == value);
                if (Widgets.ButtonInvisible(buttonRect))
                {
                    ActiveTab = value;
                    ButtonSound();
                }
                buttonRect = PlaywrightDrawHelper.RectWithMargin(buttonRect, OptionContentMargin);
                Widgets.LabelFit(buttonRect, ("Playwright.Tabs." + value.ToString()).Translate());
                currentY += OptionHeight + Margin;
            }
        }

        private void DrawIntro(Rect contentRect)
        {
            PlaywrightDrawHelper.NextLabel(contentRect, "Playwright.Tabs.Intro.Welcome");
        }

        private void DrawOrigin(Rect contentRect)
        {
            Rect welcomeRect = new Rect(contentRect);
            Rect currentWelcomeRect = PlaywrightDrawHelper.NextLabel(welcomeRect, "Playwright.Tabs.Origin.Welcome");
            currentWelcomeRect.y += Margin * 1.5f;

            // TODO: Scrollable list probably
            // Left-side rect: render option for each origin
            var buttonRect = new Rect(currentWelcomeRect);
            buttonRect.width *= 0.25f;
            buttonRect.height = OptionHeight;
            buttonRect = PlaywrightDrawHelper.RectWithMargin(buttonRect, OptionContentMargin);
            var origins = OriginComponent.GetAvailableOrigins();
            foreach (OriginComponent origin in origins)
            {
                Widgets.DrawOptionBackground(buttonRect, this.PlaywrightStructure.Origin?.Id == origin.Id);
                if (Widgets.ButtonInvisible(buttonRect))
                {
                    this.PlaywrightStructure.Origin = origin;
                    ButtonSound();
                }
                Rect buttonContentRect = PlaywrightDrawHelper.RectWithMargin(buttonRect, OptionContentMargin);
                Widgets.LabelFit(buttonContentRect, origin.NameTranslated);
                buttonRect.y += OptionHeight + Margin;
            }
            
            if (this.PlaywrightStructure.Origin == null)
            {
                return;
            }
            OriginComponent selectedOrigin = this.PlaywrightStructure.Origin;

            // Right-side rect: render origin description
            Rect originRect = new Rect(currentWelcomeRect);
            originRect.width *= 0.75f;
            originRect.width -= Margin;
            originRect.x += buttonRect.width + Margin;
            Widgets.DrawBoxSolidWithOutline(originRect, PanelBGColor, PanelOutlineColor, PanelOutlineWidth);

            Rect originContentRect = PlaywrightDrawHelper.RectWithMargin(originRect, Margin);
            Listing_Standard originContentListing = new Listing_Standard();
            originContentListing.Begin(originContentRect);

            originContentListing.Label(selectedOrigin.NameTranslated);
            originContentListing.Gap();
            originContentListing.Label(selectedOrigin.DescriptionTranslated);
            originContentListing.Gap();
            originContentListing.Label(selectedOrigin.SummaryTranslated);
            originContentListing.Gap();
            originContentListing.Label("Playwright.Components.SuggestedIdeo".Translate() + " " + selectedOrigin.SuggestedIdeoTranslated);
            
            originContentListing.Gap();
            selectedOrigin.DrawAdditionalContent(originContentListing);

            originContentListing.End();
            // TODO: Draw funny preview image

        }

        private void DrawBoons(Rect contentRect)
        {
            Rect currentRect = new Rect(contentRect);
            currentRect = PlaywrightDrawHelper.NextLabel(currentRect, "Playwright.Tabs.Boons.Welcome");
            currentRect.y += Margin * 1.5f;
        }

        private void ButtonSound()
        {
            SoundDefOf.Click.PlayOneShotOnCamera();
        }

        private enum Tabs
        {
            Intro,
            Origin,
            Boons,
            Allies,
            Enemies,
            OtherFactions,
            WinConditions,
            Special
        }
    }
}
