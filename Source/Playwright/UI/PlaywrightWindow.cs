using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Components.WinConditions;
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
        private Color PanelSelectionsBGColor => Widgets.WindowBGFillColor;
        private float PanelContentMargin => 5f;

        private float OptionHeight => 50f;
        private float OptionContentMargin => 5f;

        private float ListMargin => 5f;

        private float FactionContentHeight => 30f;

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
                case Tabs.WinConditions:
                    DrawWinConditions(tabContentRect);
                    break;
                case Tabs.SpecialConditions:
                    DrawSpecialConditions(tabContentRect);
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

        private Vector2 OriginsScrollPos = Vector2.zero;
        private Vector2 OriginContentScrollPos = Vector2.zero;
        private Listing_AutoFitVertical OriginContentListing = new Listing_AutoFitVertical();
        private void DrawOrigin(Rect contentRect)
        {
            List<OriginComponent> origins = OriginComponent.GetAvailableOrigins();

            Rect welcomeRect = new Rect(contentRect);
            Rect currentWelcomeRect = PlaywrightDrawHelper.NextLabel(welcomeRect, "Playwright.Tabs.Origin.Welcome");
            currentWelcomeRect.y += Margin * 1.5f;
            currentWelcomeRect.height -= Margin * 1.5f;

            // Left-side rect: render option for each origin
            Rect originList = new Rect(currentWelcomeRect);
            originList.width *= 0.25f;
            Rect originListInner = new Rect(originList);
            originListInner.height = (OptionHeight + Margin) * origins.Count;
            originListInner.width -= Margin;
            Widgets.BeginScrollView(originList, ref OriginsScrollPos, originListInner);
            var buttonRect = new Rect(originListInner);
            buttonRect.height = OptionHeight;
            buttonRect = PlaywrightDrawHelper.RectWithMargin(buttonRect, OptionContentMargin);

            foreach (OriginComponent origin in origins)
            {
                Widgets.DrawOptionBackground(buttonRect, this.PlaywrightStructure.Origin?.Id == origin.Id);
                if (Widgets.ButtonInvisible(buttonRect))
                {
                    this.PlaywrightStructure.Origin = origin;
                    OriginContentListing.Invalidate();
                    ButtonSound();
                }
                Rect buttonContentRect = PlaywrightDrawHelper.RectWithMargin(buttonRect, OptionContentMargin);
                Widgets.LabelFit(buttonContentRect, origin.NameTranslated);
                buttonRect.y += OptionHeight + Margin * 0.25f;
            }

            Widgets.EndScrollView();

            if (this.PlaywrightStructure.Origin == null)
            {
                return;
            }

            // Right-side rect: render origin description
            Rect originRect = new Rect(currentWelcomeRect);
            originRect.width *= 0.75f;
            originRect.width -= Margin;
            originRect.x += originList.width + Margin;
            Widgets.DrawBoxSolidWithOutline(originRect, PanelSelectionsBGColor, PanelOutlineColor, PanelOutlineWidth);
            originRect = PlaywrightDrawHelper.RectWithMargin(originRect, Margin);
            originRect.width += Margin * 0.5f;

            DrawSelectedOrigin(originRect);
        }

        private void DrawSelectedOrigin(Rect originRect)
        {
            OriginComponent selectedOrigin = this.PlaywrightStructure.Origin;

            Rect originContentRect = new Rect(originRect);
            originContentRect.width -= Margin;
            originContentRect = OriginContentListing.GetScrollViewInnerRect(originContentRect);

            Widgets.BeginScrollView(originRect, ref OriginContentScrollPos, originContentRect);
            OriginContentListing.Begin(originContentRect);

            Text.Font = GameFont.Medium;
            OriginContentListing.Label(selectedOrigin.NameTranslated);
            Text.Font = GameFont.Small;
            OriginContentListing.Gap();
            OriginContentListing.Label(selectedOrigin.DescriptionTranslated);
            OriginContentListing.Gap();
            OriginContentListing.Label(selectedOrigin.SummaryTranslated);

            if (ModsConfig.IdeologyActive)
            {
                OriginContentListing.Gap();
                OriginContentListing.Label("Playwright.Components.SuggestedIdeo".Translate() + " " + selectedOrigin.SuggestedIdeoTranslated);
            }
            OriginContentListing.Gap();
            selectedOrigin.DoAdditionalContents(OriginContentListing, originContentRect);

            if (selectedOrigin.SettingsHeight > 0f)
            {
                selectedOrigin.DoSettingsContents(OriginContentListing.GetRect(selectedOrigin.SettingsHeight));
            }

            OriginContentListing.End();

            Widgets.EndScrollView();
        }

        private Vector2 AvailableBoonsScrollPosition = Vector2.zero;
        private Listing_AutoFitVertical AvailableBoonsListing = new Listing_AutoFitVertical();
        private Vector2 SelectedBoonsScrollPosition = Vector2.zero;
        private Listing_AutoFitVertical SelectedBoonsListing = new Listing_AutoFitVertical();
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

            // Available boons
            Rect availableBoonsRect = new Rect(nextRect);
            availableBoonsRect.width *= 0.25f;
            availableBoonsRect.width += Margin;
            availableBoonsRect = PlaywrightDrawHelper.RectWithMargin(availableBoonsRect, PanelContentMargin);
            Rect selectedBoonsRect = new Rect(nextRect);
            selectedBoonsRect.width *= 0.75f;
            selectedBoonsRect.width -= PanelContentMargin;
            selectedBoonsRect.x += availableBoonsRect.width;

            Rect availableBoonsRectInner = PlaywrightDrawHelper.RectWithMargin(availableBoonsRect, PanelContentMargin);
            availableBoonsRectInner.width -= Margin;
            availableBoonsRectInner = AvailableBoonsListing.GetScrollViewInnerRect(availableBoonsRectInner);

            Widgets.BeginScrollView(availableBoonsRect, ref AvailableBoonsScrollPosition, availableBoonsRectInner);
            AvailableBoonsListing.Begin(availableBoonsRectInner);

            foreach (BoonComponent boon in availableBoons)
            {
                Rect boonRect = AvailableBoonsListing.GetRect(OptionHeight);
                Widgets.DrawOptionBackground(boonRect, false);
                Widgets.Label(PlaywrightDrawHelper.RectWithMargin(boonRect, OptionContentMargin), boon.NameTranslated);
                PlaywrightDrawHelper.DrawInTopRight(boonRect, plusTex, 2f, 0.4f);
                if (Widgets.ButtonInvisible(boonRect))
                {
                    selectedBoons.Add(boon);
                    this.AddSound();
                    AvailableBoonsListing.Invalidate();
                    SelectedBoonsListing.Invalidate();
                }
                if (Mouse.IsOver(boonRect))
                {
                    TooltipHandler.TipRegion(boonRect, boon.DescriptionTranslated);
                }
            }

            AvailableBoonsListing.End();
            Widgets.EndScrollView();

            // Selected boons
            selectedBoonsRect = PlaywrightDrawHelper.RectWithMargin(selectedBoonsRect, PanelContentMargin);
            Widgets.DrawBoxSolidWithOutline(selectedBoonsRect, PanelSelectionsBGColor, PanelOutlineColor, PanelOutlineWidth);
            Rect selectedBoonsRectInner = new Rect(selectedBoonsRect);
            selectedBoonsRectInner.width -= Margin;
            selectedBoonsRectInner = SelectedBoonsListing.GetScrollViewInnerRect(selectedBoonsRectInner);

            Widgets.BeginScrollView(selectedBoonsRect, ref SelectedBoonsScrollPosition, selectedBoonsRectInner);
            SelectedBoonsListing.Begin(selectedBoonsRectInner);

            foreach (BoonComponent boon in selectedBoons.ToList())
            {
                Rect boonContentRect = SelectedBoonsListing.GetRect(boon.ContentHeight + boon.SettingsHeight);
                boonContentRect = PlaywrightDrawHelper.RectWithMargin(boonContentRect, 5f);
                boon.DoBoonContents(boonContentRect);
                if (PlaywrightDrawHelper.DrawButtonInTopRight(boonContentRect, deleteTex, 2f, 0.4f))
                {
                    selectedBoons.Remove(boon);
                    RemoveSound();
                    AvailableBoonsListing.Invalidate();
                    SelectedBoonsListing.Invalidate();
                }
                PlaywrightDrawHelper.DrawBottomLine(boonContentRect, PanelOutlineColor, PanelOutlineWidth);
            }

            SelectedBoonsListing.End();
            Widgets.EndScrollView();
        }

        private Vector2 AlliesScrollPosition = Vector2.zero;
        private Vector2 NeutralsScrollPosition = Vector2.zero;
        private Vector2 EnemiesScrollPosition = Vector2.zero;
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

            Rect alliesRectInner = PlaywrightDrawHelper.RectWithMargin(alliesRect, PanelContentMargin);
            alliesRectInner.width -= Margin;
            alliesRectInner.height = selectedAllies.Sum(f => f.SettingsHeight + FactionContentHeight + ListMargin) + 120f;
            Rect neutralsRectInner = PlaywrightDrawHelper.RectWithMargin(neutralsRect, PanelContentMargin);
            neutralsRectInner.width -= Margin;
            neutralsRectInner.height = selectedNeutrals.Sum(f => f.SettingsHeight + FactionContentHeight + ListMargin) + 120f;
            Rect enemiesRectInner = PlaywrightDrawHelper.RectWithMargin(enemiesRect, PanelContentMargin);
            enemiesRectInner.width -= Margin;
            enemiesRectInner.height = selectedEnemies.Sum(f => f.SettingsHeight + FactionContentHeight + ListMargin) + 120f;

            // Allies
            Widgets.BeginScrollView(alliesRect, ref AlliesScrollPosition, alliesRectInner);
            Listing_Standard alliesListing = new Listing_Standard();
            alliesListing.Begin(alliesRectInner);
            alliesListing.Label("Playwright.Tabs.Factions.Allies".Translate());
            if (alliesListing.ButtonText("Playwright.Tabs.Factions.AddAlly".Translate()))
            {
                DoFactionFloatMenu(availableFactions, selectedAllies, FactionRelationKind.Ally);
                ButtonSound();
            }
            DrawSelectedFactions(alliesListing, selectedAllies, FactionRelationKind.Ally);
            alliesListing.End();
            Widgets.EndScrollView();

            // Neutrals
            Widgets.BeginScrollView(neutralsRect, ref NeutralsScrollPosition, neutralsRectInner);
            Listing_Standard neutralsListing = new Listing_Standard();
            neutralsListing.Begin(neutralsRectInner);
            neutralsListing.Label("Playwright.Tabs.Factions.Neutrals".Translate());
            if (neutralsListing.ButtonText("Playwright.Tabs.Factions.Add".Translate()))
            {
                DoFactionFloatMenu(availableFactions, selectedNeutrals, FactionRelationKind.Neutral);
                ButtonSound();
            }
            DrawSelectedFactions(neutralsListing, selectedNeutrals, FactionRelationKind.Neutral);
            neutralsListing.End();
            Widgets.EndScrollView();

            // Enemies
            Widgets.BeginScrollView(enemiesRect, ref EnemiesScrollPosition, enemiesRectInner);
            Listing_Standard enemiesListing = new Listing_Standard();
            enemiesListing.Begin(enemiesRectInner);
            enemiesListing.Label("Playwright.Tabs.Factions.Enemies".Translate());
            if (enemiesListing.ButtonText("Playwright.Tabs.Factions.AddEnemy".Translate()))
            {
                DoFactionFloatMenu(availableFactions, selectedEnemies, FactionRelationKind.Hostile);
                ButtonSound();
            }
            DrawSelectedFactions(enemiesListing, selectedEnemies, FactionRelationKind.Hostile);
            enemiesListing.End();
            Widgets.EndScrollView();
        }

        private void DoFactionFloatMenu(List<FactionComponent> availableFactions, List<FactionComponent> selectedFactions, FactionRelationKind relationKind)
        {
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
            foreach (FactionComponent faction in selectedFactions.OrderBy(f => f.SortOrder).ToList())
            {
                if (factionListing.ButtonTextLabeledPct(faction.NameTranslated, "-", 0.8f))
                {
                    selectedFactions.Remove(faction);
                    RemoveSound();
                }

                if (faction.SettingsHeight > 0f)
                {
                    faction.DoSettingsContents(factionListing.GetRect(faction.SettingsHeight), relationKind);
                }

                factionListing.GapLine(ListMargin);
            }
        }

        private Vector2 AvailableWinConditionsScrollPos = Vector2.zero;
        private Listing_AutoFitVertical AvailableWinConditionsListing = new Listing_AutoFitVertical();
        private Vector2 SelectedWinConditionsScrollPos = Vector2.zero;
        private Listing_AutoFitVertical SelectedWinConditionsListing = new Listing_AutoFitVertical();
        private void DrawWinConditions(Rect contentRect)
        {
            List<WinConditionComponent> allWinConditions = WinConditionComponent.GetAvailableWinConditions();
            List<WinConditionComponent> selectedWinConditions = PlaywrightStructure.WinConditions;
            List<WinConditionComponent> availableWinConditions = allWinConditions.Where(b => !selectedWinConditions.Any(sb => sb.Id == b.Id)).ToList();
            Texture2D plusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

            Rect nextRect = PlaywrightDrawHelper.NextLabel(contentRect, "Playwright.Tabs.WinConditions.Welcome");
            nextRect.y += Margin;
            nextRect.height -= Margin;
            Widgets.DrawBoxSolidWithOutline(nextRect, PanelBGColor, PanelOutlineColor, PanelOutlineWidth);

            // Available win conditions
            Rect availableWinConditionsRect = new Rect(nextRect);
            availableWinConditionsRect.width *= 0.25f;
            availableWinConditionsRect.width += Margin;
            availableWinConditionsRect = PlaywrightDrawHelper.RectWithMargin(availableWinConditionsRect, PanelContentMargin);
            Rect selectedWinConditionsRect = new Rect(nextRect);
            selectedWinConditionsRect.width *= 0.75f;
            selectedWinConditionsRect.width -= PanelContentMargin;
            selectedWinConditionsRect.x += availableWinConditionsRect.width;

            Rect availableWinConditionsRectInner = PlaywrightDrawHelper.RectWithMargin(availableWinConditionsRect, PanelContentMargin);
            availableWinConditionsRectInner.width -= Margin;
            availableWinConditionsRectInner = AvailableWinConditionsListing.GetScrollViewInnerRect(availableWinConditionsRectInner);

            Widgets.BeginScrollView(availableWinConditionsRect, ref AvailableWinConditionsScrollPos, availableWinConditionsRectInner);
            AvailableWinConditionsListing.Begin(availableWinConditionsRectInner);

            foreach (WinConditionComponent winCondition in availableWinConditions)
            {
                Rect winConditionRect = AvailableWinConditionsListing.GetRect(OptionHeight);
                Widgets.DrawOptionBackground(winConditionRect, false);
                Widgets.Label(PlaywrightDrawHelper.RectWithMargin(winConditionRect, OptionContentMargin), winCondition.NameTranslated);
                PlaywrightDrawHelper.DrawInTopRight(winConditionRect, plusTex, 2f, 0.4f);
                if (Widgets.ButtonInvisible(winConditionRect))
                {
                    selectedWinConditions.Add(winCondition);
                    this.AddSound();
                    AvailableWinConditionsListing.Invalidate();
                    SelectedWinConditionsListing.Invalidate();
                }
                if (Mouse.IsOver(winConditionRect))
                {
                    TooltipHandler.TipRegion(winConditionRect, winCondition.DescriptionTranslated);
                }
            }

            AvailableWinConditionsListing.End();
            Widgets.EndScrollView();

            // Selected win conditions
            selectedWinConditionsRect = PlaywrightDrawHelper.RectWithMargin(selectedWinConditionsRect, PanelContentMargin);
            Widgets.DrawBoxSolidWithOutline(selectedWinConditionsRect, PanelSelectionsBGColor, PanelOutlineColor, PanelOutlineWidth);
            selectedWinConditionsRect.x += Margin;
            selectedWinConditionsRect.width -= Margin;
            Rect selectedWinConditionsRectInner = new Rect(selectedWinConditionsRect);
            selectedWinConditionsRectInner.width -= Margin;
            selectedWinConditionsRectInner = SelectedWinConditionsListing.GetScrollViewInnerRect(selectedWinConditionsRectInner);

            Widgets.BeginScrollView(selectedWinConditionsRect, ref SelectedWinConditionsScrollPos, selectedWinConditionsRectInner);
            SelectedWinConditionsListing.Begin(selectedWinConditionsRectInner);

            foreach (WinConditionComponent winCondition in selectedWinConditions.ToList())
            {
                Rect deleteButtonRect = SelectedWinConditionsListing.GetRect(0f);
                deleteButtonRect.height = 50f;
                deleteButtonRect = PlaywrightDrawHelper.RectWithMargin(deleteButtonRect, 5f);
                winCondition.DoWinConditionContents(SelectedWinConditionsListing, deleteButtonRect);
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 2f, 0.4f))
                {
                    selectedWinConditions.Remove(winCondition);
                    RemoveSound();
                    AvailableWinConditionsListing.Invalidate();
                    SelectedWinConditionsListing.Invalidate();
                }
                Rect lineRect = SelectedWinConditionsListing.GetRect(3f);
                PlaywrightDrawHelper.DrawBottomLine(lineRect, PanelOutlineColor, PanelOutlineWidth);
            }

            SelectedWinConditionsListing.End();
            Widgets.EndScrollView();
        }

        private void DrawSpecialConditions(Rect contentRect)
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
                            Find.WindowStack.Add(new Page_SelectScenario());
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
            SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
        }

        private void RemoveSound()
        {
            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
        }

        public enum Tabs
        {
            Intro,
            Origin,
            Boons,
            Factions,
            WinConditions,
            SpecialConditions
        }
    }
}
