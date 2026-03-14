using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
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
        public Tabs ActiveTab { get; private set; } = Tabs.Intro;

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
            this.doCloseX = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            HookRegistration.CallPlaywrightWindowPreWindowContents(this, PlaywrightStructure, inRect);

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

            HookRegistration.CallPlaywrightWindowPostWindowContents(this, PlaywrightStructure, inRect);
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

            if (ModsConfig.IdeologyActive)
            {
                originContentListing.Gap();
                originContentListing.Label("Playwright.Components.SuggestedIdeo".Translate() + " " + selectedOrigin.SuggestedIdeoTranslated);
            }

            originContentListing.Gap();
            selectedOrigin.DoAdditionalContents(originContentListing, originContentRect);

            originContentListing.End();
        }

        private void DrawBoons(Rect contentRect)
        {
            List<BoonComponent> allBoons = BoonComponent.GetAvailableBoons();
            List<BoonComponent> selectedBoons = PlaywrightStructure.Boons;
            List<BoonComponent> availableBoons = allBoons.Where(b => !selectedBoons.Any(sb => sb.Id == b.Id)).ToList();
            Texture2D plusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

            Rect nextRect = PlaywrightDrawHelper.NextLabel(contentRect, "Playwright.Tabs.Boons.Welcome");
            nextRect.y += Margin;
            nextRect.height -= Margin;
            Widgets.DrawBoxSolidWithOutline(nextRect, PanelBGColor, PanelOutlineColor, PanelOutlineWidth);

            Rect availableBoonsRect = new Rect(nextRect);
            availableBoonsRect.width *= 0.25f;
            Rect selectedBoonsRect = new Rect(nextRect);
            selectedBoonsRect.width *= 0.75f;
            selectedBoonsRect.width -= Margin;
            selectedBoonsRect.x += availableBoonsRect.width;
            selectedBoonsRect.x += Margin;

            availableBoonsRect = PlaywrightDrawHelper.RectWithMargin(availableBoonsRect, PanelContentMargin);
            selectedBoonsRect = PlaywrightDrawHelper.RectWithMargin(selectedBoonsRect, PanelContentMargin);

            Listing_Standard availableBoonsListing = new Listing_Standard();
            availableBoonsListing.Begin(availableBoonsRect);

            foreach (BoonComponent boon in availableBoons)
            {
                Rect boonRect = availableBoonsListing.GetRect(OptionHeight);
                Widgets.DrawOptionBackground(boonRect, false);
                Widgets.Label(PlaywrightDrawHelper.RectWithMargin(boonRect, OptionContentMargin), boon.NameTranslated);
                PlaywrightDrawHelper.DrawInTopRight(boonRect, plusTex, 2f, 0.4f);
                if (Widgets.ButtonInvisible(boonRect))
                {
                    selectedBoons.Add(boon);
                    this.AddSound();
                }
                if (Mouse.IsOver(boonRect))
                {
                    TooltipHandler.TipRegion(boonRect, boon.DescriptionTranslated);
                }
            }

            availableBoonsListing.End();

            Widgets.DrawBoxSolidWithOutline(selectedBoonsRect, PanelBGColor, PanelOutlineColor, PanelOutlineWidth);
            Listing_Standard selectedBoonsListing = new Listing_Standard();
            selectedBoonsListing.Begin(selectedBoonsRect);

            foreach (BoonComponent boon in selectedBoons.ToList())
            {
                Rect boonContentRect = selectedBoonsListing.GetRect(boon.ContentHeight + boon.SettingsHeight);
                boonContentRect = PlaywrightDrawHelper.RectWithMargin(boonContentRect, 5f);
                boon.DoBoonContents(boonContentRect);
                Rect deleteButton = PlaywrightDrawHelper.DrawInTopRight(boonContentRect, deleteTex, 2f, 0.4f);
                if (Widgets.ButtonInvisible(deleteButton))
                {
                    selectedBoons.Remove(boon);
                    RemoveSound();
                }
            }

            selectedBoonsListing.End();
        }

        private void DrawFactions(Rect contentRect)
        {
            List<FactionComponent> availableFactions = FactionComponent.GetAvailableFactions();
            List<FactionComponent> selectedAllies = PlaywrightStructure.AllyFactions;
            List<FactionComponent> selectedNeutrals = PlaywrightStructure.NeutralFactions;
            List<FactionComponent> selectedEnemies = PlaywrightStructure.EnemyFactions;

            Rect nextRect = PlaywrightDrawHelper.NextLabel(contentRect, "Playwright.Tabs.Factions.Welcome");
            nextRect.y += Margin;
            nextRect.height -= Margin;

            // Fuuck how do I RectDivider?
            Rect alliesRect = new Rect(nextRect);
            alliesRect.width /= 3;
            Widgets.DrawLine(new Vector2(alliesRect.x + alliesRect.width, alliesRect.y), new Vector2(alliesRect.x + alliesRect.width, alliesRect.y + alliesRect.height), PanelOutlineColor, PanelOutlineWidth);

            Rect neutralsRect = new Rect(nextRect);
            neutralsRect.width /= 3;
            neutralsRect.x += alliesRect.width;
            Widgets.DrawLine(new Vector2(neutralsRect.x + neutralsRect.width, neutralsRect.y), new Vector2(neutralsRect.x + neutralsRect.width, neutralsRect.y + neutralsRect.height), PanelOutlineColor, PanelOutlineWidth);

            Rect enemiesRect = new Rect(nextRect);
            enemiesRect.width /= 3;
            enemiesRect.x += alliesRect.width + neutralsRect.width;

            alliesRect = PlaywrightDrawHelper.RectWithMargin(alliesRect, PanelContentMargin);
            neutralsRect = PlaywrightDrawHelper.RectWithMargin(neutralsRect, PanelContentMargin);
            enemiesRect = PlaywrightDrawHelper.RectWithMargin(enemiesRect, PanelContentMargin);

            // Allies
            Listing_Standard alliesListing = new Listing_Standard();
            alliesListing.Begin(alliesRect);
            alliesListing.Label("Playwright.Tabs.Factions.Allies".Translate());
            
            alliesListing.End();

            // Neutrals
            Listing_Standard neutralsListing = new Listing_Standard();
            neutralsListing.Begin(neutralsRect);
            neutralsListing.Label("Playwright.Tabs.Factions.Neutrals".Translate());
            neutralsListing.End();

            // Enemies
            Listing_Standard enemiesListing = new Listing_Standard();
            enemiesListing.Begin(enemiesRect);
            enemiesListing.Label("Playwright.Tabs.Factions.Enemies".Translate());
            if (enemiesListing.ButtonText("Playwright.Tabs.Factions.Add".Translate()))
            {
                DoFactionFloatMenu(availableFactions, FactionRelationKind.Hostile);
            }
            DrawSelectedFactions(enemiesListing, selectedEnemies, FactionRelationKind.Hostile);
            enemiesListing.End();
        }

        private void DoFactionFloatMenu(List<FactionComponent> availableFactions, FactionRelationKind relationKind)
        {
            List<FactionComponent> selectedFactions;
            switch (relationKind)
            {
                case FactionRelationKind.Ally:
                    selectedFactions = PlaywrightStructure.AllyFactions;
                    break;
                case FactionRelationKind.Hostile:
                    selectedFactions = PlaywrightStructure.EnemyFactions;
                    break;
                default:
                    selectedFactions = PlaywrightStructure.NeutralFactions;
                    break;
            }
            List<FactionComponent> allSelectedFactions = PlaywrightStructure.AllyFactions
                .Union(PlaywrightStructure.NeutralFactions)
                .Union(PlaywrightStructure.EnemyFactions)
                .ToList();

            List<FactionComponent> selectableFactions = availableFactions
                .Where(f => f.AllowedDispositions.Contains(relationKind))
                .Where(f => selectedFactions.Count(ef => ef.Id == f.Id) < f.MaxInGroup)
                .Where(f => allSelectedFactions.Count(ef => ef.Id == f.Id) < f.MaxTotal)
                .ToList();

            List<FloatMenuOption> floatMenuOptions = new List<FloatMenuOption>();
            foreach (FactionComponent faction in selectableFactions)
            {
                floatMenuOptions.Add(new FloatMenuOption(faction.NameTranslated, () => selectedFactions.Add(faction)));
            }
            Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
        }

        private void DrawSelectedFactions(Listing_Standard factionListing, List<FactionComponent> selectedFactions, FactionRelationKind relationKind)
        {
            foreach (FactionComponent faction in selectedFactions.ToList())
            {
                if (factionListing.ButtonTextLabeledPct(faction.NameTranslated, "-", 0.8f))
                {
                    selectedFactions.Remove(faction);
                    RemoveSound();
                }
            }
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

        private void AddSound()
        {
            SoundDefOf.TabOpen.PlayOneShotOnCamera();
        }

        private void RemoveSound()
        {
            SoundDefOf.TabClose.PlayOneShotOnCamera();
        }

        public enum Tabs
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
