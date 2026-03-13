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
            tabPanelRect.width *= 0.25f;
            tabPanelRect.height *= 0.9f;

            DrawTabPanel(tabPanelRect);

            Rect tabContentRect = new Rect(inRect);
            tabContentRect.width -= tabPanelRect.width + Margin;
            tabContentRect.x += tabPanelRect.width + Margin;
            tabContentRect.height *= 0.9f;

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
                case Tabs.Factions:
                    DrawFactions(tabContentRect);
                    break;
            }

            Rect buttonBarRect = new Rect(inRect);
            buttonBarRect.height *= 0.1f;
            buttonBarRect.height -= Margin;
            buttonBarRect.y += tabContentRect.height + Margin;
            DrawButtonBar(buttonBarRect);
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
                Widgets.Label(buttonRect, ("Playwright.Tabs." + value.ToString()).Translate());
                currentY += OptionHeight + (Margin * 0.75f);
            }
        }

        private void DrawIntro(Rect contentRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(contentRect);
            listing.Label("Playwright.Tabs.Intro.Welcome".Translate());
            listing.Gap();
            listing.Label("Playwright.Tabs.Intro.Welcome.1".Translate());
            listing.Gap();
            listing.Label("Playwright.Tabs.Intro.Welcome.2".Translate());
            listing.End();
        }

        private void DrawOrigin(Rect contentRect)
        {
            Rect welcomeRect = new Rect(contentRect);
            Rect currentWelcomeRect = PlaywrightDrawHelper.NextLabel(welcomeRect, "Playwright.Tabs.Origin.Welcome");
            currentWelcomeRect.y += Margin * 1.5f;
            currentWelcomeRect.height -= Margin * 1.5f;

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
                buttonRect.y += OptionHeight + Margin * 0.25f;
            }
            
            if (this.PlaywrightStructure.Origin == null)
            {
                return;
            }
            OriginComponent selectedOrigin = this.PlaywrightStructure.Origin;

            // Right-side rect: render origin description
            Rect originRect = new Rect(currentWelcomeRect);
            originRect.height -= (currentWelcomeRect.y - welcomeRect.y);
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
        }

        private void DrawBoons(Rect contentRect)
        {
            Rect nextRect = PlaywrightDrawHelper.NextLabel(contentRect, "Playwright.Tabs.Boons.Welcome");
            nextRect.y += Margin;
            nextRect.height -= Margin;
            Widgets.DrawBoxSolidWithOutline(nextRect, PanelBGColor, PanelOutlineColor, PanelOutlineWidth);
            Listing_Standard boonsListing = new Listing_Standard();
            boonsListing.Begin(nextRect);

            // TODO: Draw some sort of nice UI here using either the nextRect Rect or the boonsListing Listing_Standard

            boonsListing.End();
        }

        private void DrawFactions(Rect contentRect)
        {

        }

        private void DrawButtonBar(Rect contentRect)
        {
            const float ButtonHeight = 38f;
            const float ButtonWidth = 140f;
            const float ButtonMargin = 8f;

            float buttonY = contentRect.yMax - ButtonHeight - ButtonMargin;
            float leftX = contentRect.x;

            Rect saveRect = new Rect(leftX, buttonY, ButtonWidth, ButtonHeight);
            if (Widgets.ButtonText(saveRect, "Save".Translate()))
            {
                Find.WindowStack.Add(new Dialog_PlaywrightList_Save(this.PlaywrightStructure));
            }

            Rect loadRect = new Rect(saveRect.xMax + ButtonMargin, buttonY, ButtonWidth, ButtonHeight);
            if (Widgets.ButtonText(loadRect, "Load".Translate()))
            {
                Find.WindowStack.Add(new Dialog_PlaywrightList_Load((PlaywrightStructure loadedStructure) =>
                {
                    this.PlaywrightStructure = loadedStructure;
                }));
            }

            Rect generateRect = new Rect(contentRect.xMax - ButtonWidth - ButtonMargin, buttonY, ButtonWidth, ButtonHeight);
            if (Widgets.ButtonText(generateRect, "Playwright.CreateScenario".Translate()))
            {
                this.CompileScenario();
            }
        }

        private void CompileScenario()
        {
            PlaywrightBuilder builder = new PlaywrightBuilder();
            try
            {
                Scenario scenario = builder.MakeScenario(this.PlaywrightStructure);
                Find.WindowStack.Add(new Dialog_ScenarioList_Save(scenario, () =>
                {
                    Find.WindowStack.Add(new ScenarioSavedPopupWindow((bool shouldGoToNewGame) =>
                    {
                        if (shouldGoToNewGame)
                        {
                            this.Close();
                            // TODO: Auto-open new game window
                        }
                    }));
                }));
            }
            catch (Exception ex)
            {
                Log.Error("[Playwright] Error generating scenario: " + ex.Message);
                Log.Error(ex.StackTrace);
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ErrorSavingScenario".Translate()));
            }
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
            Factions,
            WinConditions,
            Special
        }
    }
}
