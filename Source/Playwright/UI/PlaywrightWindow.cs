using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Components.SpecialConditions;
using Rokk.Playwright.Components.WinConditions;
using Rokk.Playwright.Composer;
using Rokk.Playwright.Exceptions;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.Sound;

namespace Rokk.Playwright.UI
{
    public class PlaywrightWindow : Window
    {
        private PlaywrightStructure PlaywrightStructure = PlaywrightStructure.CreateDefault();
        public Tabs ActiveTab { get; private set; } = Tabs.Intro;
        public bool FormDirty { get; private set; } = false;

        private int PanelOutlineWidth => 1;
        private Color PanelOutlineColor => Widgets.SeparatorLineColor;
        private Color PanelBGColor => Widgets.MenuSectionBGFillColor;
        private Color PanelSelectionsBGColor => Widgets.WindowBGFillColor;
        private float PanelContentMargin => 5f;

        private float OptionHeight => 50f;
        private float OptionContentMargin => 5f;

        public override Vector2 InitialSize => new Vector2(1200, 800);

        // Auto-listings and scroll positions
        // Can't use field initializers because I cannot use a reference to InvalidateAutoListings() there,
        // have to assign them in the constructor
        private Vector2 OriginsScrollPos = Vector2.zero;
        private Vector2 OriginContentScrollPos = Vector2.zero;
        private Listing_AutoFitVertical OriginsListing;
        private Listing_AutoFitVertical OriginContentListing;

        private Vector2 AvailableBoonsScrollPosition = Vector2.zero;
        private Listing_AutoFitVertical AvailableBoonsListing;
        private Vector2 SelectedBoonsScrollPosition = Vector2.zero;
        private Listing_AutoFitVertical SelectedBoonsListing;

        private Vector2 AlliesScrollPosition = Vector2.zero;
        private Listing_AutoFitVertical AllyFactionsListing;
        private Vector2 NeutralsScrollPosition = Vector2.zero;
        private Listing_AutoFitVertical NeutralFactionsListing;
        private Vector2 EnemiesScrollPosition = Vector2.zero;
        private Listing_AutoFitVertical EnemyFactionsListing;

        private Vector2 AvailableWinConditionsScrollPos = Vector2.zero;
        private Listing_AutoFitVertical AvailableWinConditionsListing;
        private Vector2 SelectedWinConditionsScrollPos = Vector2.zero;
        private Listing_AutoFitVertical SelectedWinConditionsListing;

        private Vector2 AvailableSpecialConditionsScrollPos = Vector2.zero;
        private Listing_AutoFitVertical AvailableSpecialConditionsListing;
        private Vector2 SelectedSpecialConditionsScrollPos = Vector2.zero;
        private Listing_AutoFitVertical SelectedSpecialConditionsListing;

        private Listing_Standard SummaryListing = new Listing_Standard();
        private Listing_Standard SummaryContentListing = new Listing_Standard();

        private Listing_Standard ReviewListing = new Listing_Standard();
        private Listing_AutoFitVertical ReviewContentListing;
        private Vector2 ReviewContentScrollPos = Vector2.zero;
        private Listing_Standard ReviewErrorListing = new Listing_Standard();

        public PlaywrightWindow() : base()
        {
            this.closeOnClickedOutside = false;
            this.resizeable = false;
            this.optionalTitle = "Playwright.WindowTitle".Translate();
            this.doCloseX = true;

            this.OriginsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.OriginContentListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.AvailableBoonsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.SelectedBoonsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.AllyFactionsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.NeutralFactionsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.EnemyFactionsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.AvailableWinConditionsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.SelectedWinConditionsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.AvailableSpecialConditionsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.SelectedSpecialConditionsListing = new Listing_AutoFitVertical(InvalidateAutoListings);
            this.ReviewContentListing = new Listing_AutoFitVertical(InvalidateAutoListings);
        }

        public override void DoWindowContents(Rect inRect)
        {
            HookRegistration.CallPlaywrightWindowPreWindowContents(this, PlaywrightStructure, inRect);

            Rect tabPanelRect = new Rect(inRect);
            tabPanelRect.width *= 0.15f;
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
                    DrawIntroTab(tabContentRect);
                    break;
                case Tabs.Origin:
                    DrawOriginTab(tabContentRect);
                    break;
                case Tabs.Boons:
                    DrawBoonsTab(tabContentRect);
                    break;
                case Tabs.Factions:
                    DrawFactionsTab(tabContentRect);
                    break;
                case Tabs.WinConditions:
                    DrawWinConditionsTab(tabContentRect);
                    break;
                case Tabs.SpecialConditions:
                    DrawSpecialConditionsTab(tabContentRect);
                    break;
                case Tabs.Summary:
                    DrawSummaryTab(tabContentRect);
                    // We can't easily detect changes in a text input, just assume that someone visiting the Summary tab may change something
                    FormDirty = true;
                    InvalidateAutoListings();
                    break;
                case Tabs.ReviewAndSave:
                    DrawReviewAndSaveTab(tabContentRect);
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
                    ClickSound();
                }
                buttonRect = PlaywrightDrawHelper.RectWithMargin(buttonRect, OptionContentMargin);
                Widgets.Label(buttonRect, ("Playwright.Tabs." + value.ToString()).Translate());
                currentY += OptionHeight + (Margin * 0.75f);
            }
        }

        private void DrawIntroTab(Rect contentRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(contentRect);
            listing.Label("Playwright.Tabs.Intro.Welcome".Translate());
            listing.Gap();
            listing.Label("Playwright.Tabs.Intro.Welcome.1".Translate());
            listing.Gap();
            listing.Label("Playwright.Tabs.Intro.Welcome.2".Translate());

            if (ModsConfig.BiotechActive)
            {
                listing.Gap();
                if (listing.ButtonText("Playwright.OpenXenotypeEditor".Translate(), widthPct: 0.2f))
                {
                    PlaywrightUtils.OpenXenotypeEditorWindow();
                }
            }

            if (ModsConfig.IdeologyActive)
            {
                listing.Gap();
                if (listing.ButtonText("Playwright.OpenIdeoligionEditor".Translate(), widthPct: 0.2f))
                {
                    PlaywrightUtils.OpenIdeoligionEditor();
                }
            }

            listing.End();
        }

        private void DrawOriginTab(Rect contentRect)
        {
            List<OriginComponent> origins = OriginComponent.GetAvailableOrigins();

            Rect welcomeRect = new Rect(contentRect);
            Rect currentWelcomeRect = PlaywrightDrawHelper.NextLabelTranslated(welcomeRect, "Playwright.Tabs.Origin.Welcome");
            currentWelcomeRect.y += Margin * 1.5f;
            currentWelcomeRect.height -= Margin * 1.5f;

            // Left-side rect: render option for each origin
            Rect originList = new Rect(currentWelcomeRect);
            originList.width *= 0.3f;
            originList.width += Margin;
            Rect originListInner = new Rect(originList);
            originListInner.width -= Margin;
            originListInner = OriginsListing.GetScrollViewInnerRect(originListInner);

            Widgets.BeginScrollView(originList, ref OriginsScrollPos, originListInner);
            OriginsListing.Begin(originListInner);

            foreach (OriginComponent origin in origins)
            {
                Rect buttonRect = OriginsListing.GetRect(75f);
                buttonRect = PlaywrightDrawHelper.RectWithMargin(buttonRect, 4f);
                Widgets.DrawOptionBackground(buttonRect, this.PlaywrightStructure.Origin?.Id == origin.Id);
                Rect buttonContentRect = PlaywrightDrawHelper.RectWithMargin(buttonRect, OptionContentMargin);
                Rect tagline = PlaywrightDrawHelper.NextLabel(buttonContentRect, origin.NameTranslated);
                Text.Font = GameFont.Tiny;
                Widgets.Label(tagline, origin.DescriptionShortTranslated);
                Text.Font = GameFont.Small;
                if (Widgets.ButtonInvisible(buttonRect))
                {
                    ConfirmReset(() =>
                    {
                        ChangeOrigin(origin);
                    });
                    ClickSound();
                }
                OriginsListing.Gap(4f);
            }

            OriginsListing.End();
            Widgets.EndScrollView();

            if (this.PlaywrightStructure.Origin == null)
            {
                return;
            }

            // Right-side rect: render origin description
            Rect originRect = new Rect(currentWelcomeRect);
            originRect.width *= 0.7f;
            originRect.width -= Margin;
            originRect.x += originList.width;
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

            selectedOrigin.DoPreContents(OriginContentListing);
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

            selectedOrigin.DoAdditionalContents(OriginContentListing);
            selectedOrigin.DoSettingsContents(OriginContentListing);

            OriginContentListing.End();

            Widgets.EndScrollView();
        }

        private void ChangeOrigin(OriginComponent origin)
        {
            this.PlaywrightStructure = PlaywrightStructure.CreateDefault();
            this.PlaywrightStructure.Origin = origin;

            // Set defaults
            this.PlaywrightStructure.Boons.AddRange(origin.DefaultBoons.Union(OriginDefaultsRegistration.GetDefaultBoons(origin.Id)));
            this.PlaywrightStructure.AllyFactions.AddRange(origin.DefaultAllies.Union(OriginDefaultsRegistration.GetDefaultAllies(origin.Id)));
            this.PlaywrightStructure.NeutralFactions.AddRange(origin.DefaultNeutrals.Union(OriginDefaultsRegistration.GetDefaultNeutrals(origin.Id)));
            this.PlaywrightStructure.EnemyFactions.AddRange(origin.DefaultEnemies.Union(OriginDefaultsRegistration.GetDefaultEnemies(origin.Id)));
            this.PlaywrightStructure.WinConditions.AddRange(origin.DefaultWinConditions.Union(OriginDefaultsRegistration.GetDefaultWinConditions(origin.Id)));
            this.PlaywrightStructure.SpecialConditions.AddRange(origin.DefaultSpecialConditions.Union(OriginDefaultsRegistration.GetDefaultSpecialConditions(origin.Id)));

            // Set name, description and summary
            ResetSummaryToOriginDefaults();

            // Call origin changed hook, to let submods remove default components
            HookRegistration.CallPlaywrightOriginChanged(this.PlaywrightStructure, origin);

            InvalidateAutoListings();
        }

        private void DrawBoonsTab(Rect contentRect)
        {
            List<BoonComponent> allBoons = BoonComponent.GetAvailableBoons();
            List<BoonComponent> selectedBoons = PlaywrightStructure.Boons;
            List<BoonComponent> availableBoons = allBoons.Where(b => !selectedBoons.Any(sb => sb.Id == b.Id)).ToList();
            Texture2D plusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
            Texture2D infoTex = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);

            Rect nextRect = PlaywrightDrawHelper.NextLabelTranslated(contentRect, "Playwright.Tabs.Boons.Welcome");
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
                    InvalidateAutoListings();
                    FormDirty = true;
                }
                if (Mouse.IsOver(boonRect))
                {
                    TooltipHandler.TipRegion(boonRect, boon.DescriptionShortTranslated);
                }
            }

            AvailableBoonsListing.End();
            Widgets.EndScrollView();

            // Selected boons
            selectedBoonsRect = PlaywrightDrawHelper.RectWithMargin(selectedBoonsRect, PanelContentMargin);
            Widgets.DrawBoxSolidWithOutline(selectedBoonsRect, PanelSelectionsBGColor, PanelOutlineColor, PanelOutlineWidth);
            selectedBoonsRect.x += Margin;
            selectedBoonsRect.width -= Margin;
            Rect selectedBoonsRectInner = new Rect(selectedBoonsRect);
            selectedBoonsRectInner.width -= Margin;
            selectedBoonsRectInner = SelectedBoonsListing.GetScrollViewInnerRect(selectedBoonsRectInner);

            Widgets.BeginScrollView(selectedBoonsRect, ref SelectedBoonsScrollPosition, selectedBoonsRectInner);
            SelectedBoonsListing.Begin(selectedBoonsRectInner);

            foreach (BoonComponent boon in selectedBoons.ToList())
            {
                Rect deleteButtonRect = SelectedBoonsListing.GetRect(0f);
                deleteButtonRect.height = 50f;
                deleteButtonRect = PlaywrightDrawHelper.RectWithMargin(deleteButtonRect, 5f);
                boon.DoBoonContents(SelectedBoonsListing);
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 2f, 0.4f))
                {
                    selectedBoons.Remove(boon);
                    RemoveSound();
                    InvalidateAutoListings();
                    FormDirty = true;
                }

                if (boon.HasHelp)
                {
                    Rect helpButtonRect = new Rect(deleteButtonRect);
                    helpButtonRect.width -= 32f;
                    if (PlaywrightDrawHelper.DrawButtonInTopRight(helpButtonRect, infoTex, 2f, 0.4f))
                    {
                        Find.WindowStack.Add(new InfoPopupWindow($"Playwright.Components.{boon.Id}.Help".Translate()));
                    }
                }

                Rect lineRect = SelectedBoonsListing.GetRect(PanelOutlineWidth);
                PlaywrightDrawHelper.DrawBottomLine(lineRect, PanelOutlineColor, PanelOutlineWidth);
            }

            SelectedBoonsListing.End();
            Widgets.EndScrollView();
        }

        private void DrawFactionsTab(Rect contentRect)
        {
            List<FactionComponent> availableFactions = FactionComponent.GetAvailableFactions();
            List<FactionComponent> selectedAllies = PlaywrightStructure.AllyFactions;
            List<FactionComponent> selectedNeutrals = PlaywrightStructure.NeutralFactions;
            List<FactionComponent> selectedEnemies = PlaywrightStructure.EnemyFactions;

            Rect nextRect = PlaywrightDrawHelper.NextLabelTranslated(contentRect, "Playwright.Tabs.Factions.Welcome");
            nextRect.y += Margin;
            nextRect.height -= Margin;

            // RectDivider? I hardly know 'er
            Rect factionHeaderRect = new Rect(nextRect);
            factionHeaderRect.height = 60f;

            Rect alliesHeaderRect = new Rect(factionHeaderRect);
            alliesHeaderRect.width /= 3f;
            Rect neutralsHeaderRect = new Rect(factionHeaderRect);
            neutralsHeaderRect.width /= 3f;
            neutralsHeaderRect.x += alliesHeaderRect.width;
            Rect enemiesHeaderRect = new Rect(factionHeaderRect);
            enemiesHeaderRect.width /= 3f;
            enemiesHeaderRect.x += alliesHeaderRect.width + neutralsHeaderRect.width;
            alliesHeaderRect = PlaywrightDrawHelper.RectWithMargin(alliesHeaderRect, 5f);
            neutralsHeaderRect = PlaywrightDrawHelper.RectWithMargin(neutralsHeaderRect, 5f);
            enemiesHeaderRect = PlaywrightDrawHelper.RectWithMargin(enemiesHeaderRect, 5f);

            alliesHeaderRect = PlaywrightDrawHelper.NextLabelTranslated(alliesHeaderRect, "Playwright.Tabs.Factions.Allies");
            if (Widgets.ButtonText(alliesHeaderRect, "Playwright.Tabs.Factions.AddAlly".Translate()))
            {
                DoFactionFloatMenu(availableFactions, selectedAllies, FactionRelationKind.Ally);
                ClickSound();
            }


            neutralsHeaderRect = PlaywrightDrawHelper.NextLabelTranslated(neutralsHeaderRect, "Playwright.Tabs.Factions.Neutrals");
            if (Widgets.ButtonText(neutralsHeaderRect, "Playwright.Tabs.Factions.AddNeutral".Translate()))
            {
                DoFactionFloatMenu(availableFactions, selectedNeutrals, FactionRelationKind.Neutral);
                ClickSound();
            }

            enemiesHeaderRect = PlaywrightDrawHelper.NextLabelTranslated(enemiesHeaderRect, "Playwright.Tabs.Factions.Enemies");
            if (Widgets.ButtonText(enemiesHeaderRect, "Playwright.Tabs.Factions.AddEnemy".Translate()))
            {
                DoFactionFloatMenu(availableFactions, selectedEnemies, FactionRelationKind.Hostile);
                ClickSound();
            }

            Rect factionContentRect = new Rect(nextRect);
            factionContentRect.y += factionHeaderRect.height;
            factionContentRect.height -= factionHeaderRect.height;
            Widgets.DrawBoxSolidWithOutline(factionContentRect, PanelSelectionsBGColor, PanelOutlineColor, PanelOutlineWidth);

            Rect alliesContentRect = new Rect(factionContentRect);
            alliesContentRect.width /= 3f;
            Rect neutralsContentRect = new Rect(factionContentRect);
            neutralsContentRect.width /= 3f;
            neutralsContentRect.x += alliesContentRect.width;
            Rect enemiesContentRect = new Rect(factionContentRect);
            enemiesContentRect.width /= 3f;
            enemiesContentRect.x += alliesContentRect.width + neutralsContentRect.width;

            alliesContentRect = PlaywrightDrawHelper.RectWithMargin(alliesContentRect, PanelContentMargin);
            DrawSelectedFactions(alliesContentRect, AllyFactionsListing, ref AlliesScrollPosition, selectedAllies, FactionRelationKind.Ally);

            neutralsContentRect = PlaywrightDrawHelper.RectWithMargin(neutralsContentRect, PanelContentMargin);
            DrawSelectedFactions(neutralsContentRect, NeutralFactionsListing, ref NeutralsScrollPosition, selectedNeutrals, FactionRelationKind.Neutral);

            enemiesContentRect = PlaywrightDrawHelper.RectWithMargin(enemiesContentRect, PanelContentMargin);
            DrawSelectedFactions(enemiesContentRect, EnemyFactionsListing, ref EnemiesScrollPosition, selectedEnemies, FactionRelationKind.Hostile);

            // Draw divider lines
            // DrawLineVertical is too wide with no way to change it
            float dividerX = factionContentRect.x + (factionContentRect.width / 3f);
            float dividerY1 = factionContentRect.y;
            float dividerY2 = factionContentRect.y + factionContentRect.height;
            Widgets.DrawLine(new Vector2(dividerX, dividerY1), new Vector2(dividerX, dividerY2), Color.white, PanelOutlineWidth);
            dividerX = factionContentRect.x + (factionContentRect.width / 1.5f);
            Widgets.DrawLine(new Vector2(dividerX, dividerY1), new Vector2(dividerX, dividerY2), Color.white, PanelOutlineWidth);
        }

        private void DoFactionFloatMenu(List<FactionComponent> availableFactions, List<FactionComponent> selectedFactions, FactionRelationKind relationKind)
        {
            List<FactionComponent> allSelectedFactions = PlaywrightStructure.AllyFactions
                .Union(PlaywrightStructure.NeutralFactions)
                .Union(PlaywrightStructure.EnemyFactions)
                .ToList();

            var selectableFactions = availableFactions
                .Where(f => f.AllowedDispositions.Contains(relationKind))
                .Where(f => selectedFactions.Count(ef => ef.Id == f.Id) < f.MaxInGroup)
                .Where(f => allSelectedFactions.Count(ef => ef.Id == f.Id) < f.MaxTotal);

            List<FloatMenuOption> floatMenuOptions = new List<FloatMenuOption>();
            foreach (FactionComponent faction in selectableFactions)
            {
                floatMenuOptions.Add(new FloatMenuOption(faction.NameTranslated, () =>
                {
                    selectedFactions.Add(faction);
                    FormDirty = true;
                    InvalidateAutoListings();
                }));
            }
            PlaywrightUtils.OpenFloatMenu(floatMenuOptions);
        }

        private void DrawSelectedFactions(Rect contentRect, Listing_AutoFitVertical factionListing, ref Vector2 scrollPos, List<FactionComponent> selectedFactions, FactionRelationKind relationKind)
        {
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

            Rect contentInnerRect = new Rect(contentRect);
            contentInnerRect.width -= Margin;
            contentInnerRect = factionListing.GetScrollViewInnerRect(contentInnerRect);

            Widgets.BeginScrollView(contentRect, ref scrollPos, contentInnerRect);
            factionListing.Begin(contentInnerRect);
            foreach (FactionComponent faction in selectedFactions.OrderBy(f => f.SortOrder).ToList())
            {
                Rect deleteButtonRect = factionListing.GetRect(0f);
                deleteButtonRect.height = 50f;
                faction.DoFactionContents(factionListing, relationKind);
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0, 0.4f))
                {
                    selectedFactions.Remove(faction);
                    RemoveSound();
                    InvalidateAutoListings();
                    FormDirty = true;
                }
                factionListing.GapLine(10f);
            }
            factionListing.End();
            Widgets.EndScrollView();
        }

        private void DrawWinConditionsTab(Rect contentRect)
        {
            List<WinConditionComponent> allWinConditions = WinConditionComponent.GetAvailableWinConditions();
            List<WinConditionComponent> selectedWinConditions = PlaywrightStructure.WinConditions;
            List<WinConditionComponent> availableWinConditions = allWinConditions.Where(b => !selectedWinConditions.Any(sb => sb.Id == b.Id)).ToList();
            Texture2D plusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
            Texture2D infoTex = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);

            Rect nextRect = PlaywrightDrawHelper.NextLabelTranslated(contentRect, "Playwright.Tabs.WinConditions.Welcome");
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
                    InvalidateAutoListings();
                    FormDirty = true;
                }
                if (Mouse.IsOver(winConditionRect))
                {
                    TooltipHandler.TipRegion(winConditionRect, winCondition.DescriptionShortTranslated);
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
                winCondition.DoWinConditionContents(SelectedWinConditionsListing);
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 2f, 0.4f))
                {
                    selectedWinConditions.Remove(winCondition);
                    RemoveSound();
                    InvalidateAutoListings();
                    FormDirty = true;
                }

                if (winCondition.HasHelp)
                {
                    Rect helpButtonRect = new Rect(deleteButtonRect);
                    helpButtonRect.width -= 32f;
                    if (PlaywrightDrawHelper.DrawButtonInTopRight(helpButtonRect, infoTex, 2f, 0.4f))
                    {
                        Find.WindowStack.Add(new InfoPopupWindow($"Playwright.Components.{winCondition.Id}.Help".Translate()));
                    }
                }

                Rect lineRect = SelectedWinConditionsListing.GetRect(PanelOutlineWidth);
                PlaywrightDrawHelper.DrawBottomLine(lineRect, PanelOutlineColor, PanelOutlineWidth);
            }

            SelectedWinConditionsListing.End();
            Widgets.EndScrollView();
        }

        private void DrawSpecialConditionsTab(Rect contentRect)
        {
            List<SpecialConditionComponent> allSpecialConditions = SpecialConditionComponent.GetAvailableSpecialConditions();
            List<SpecialConditionComponent> selectedSpecialConditions = PlaywrightStructure.SpecialConditions;
            List<SpecialConditionComponent> availableSpecialConditions = allSpecialConditions.Where(b => !selectedSpecialConditions.Any(sb => sb.Id == b.Id)).ToList();
            Texture2D plusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
            Texture2D infoTex = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);

            Rect nextRect = PlaywrightDrawHelper.NextLabelTranslated(contentRect, "Playwright.Tabs.SpecialConditions.Welcome");
            nextRect.y += Margin;
            nextRect.height -= Margin;
            Widgets.DrawBoxSolidWithOutline(nextRect, PanelBGColor, PanelOutlineColor, PanelOutlineWidth);

            // Available special conditions
            Rect availableSpecialConditionsRect = new Rect(nextRect);
            availableSpecialConditionsRect.width *= 0.25f;
            availableSpecialConditionsRect.width += Margin;
            availableSpecialConditionsRect = PlaywrightDrawHelper.RectWithMargin(availableSpecialConditionsRect, PanelContentMargin);
            Rect selectedSpecialConditionsRect = new Rect(nextRect);
            selectedSpecialConditionsRect.width *= 0.75f;
            selectedSpecialConditionsRect.width -= PanelContentMargin;
            selectedSpecialConditionsRect.x += availableSpecialConditionsRect.width;

            Rect availableSpecialConditionsRectInner = PlaywrightDrawHelper.RectWithMargin(availableSpecialConditionsRect, PanelContentMargin);
            availableSpecialConditionsRectInner.width -= Margin;
            availableSpecialConditionsRectInner = AvailableSpecialConditionsListing.GetScrollViewInnerRect(availableSpecialConditionsRectInner);

            Widgets.BeginScrollView(availableSpecialConditionsRect, ref AvailableSpecialConditionsScrollPos, availableSpecialConditionsRectInner);
            AvailableSpecialConditionsListing.Begin(availableSpecialConditionsRectInner);

            foreach (SpecialConditionComponent specialCondition in availableSpecialConditions)
            {
                Rect specialConditionRect = AvailableSpecialConditionsListing.GetRect(OptionHeight);
                Widgets.DrawOptionBackground(specialConditionRect, false);
                Widgets.Label(PlaywrightDrawHelper.RectWithMargin(specialConditionRect, OptionContentMargin), specialCondition.NameTranslated);
                PlaywrightDrawHelper.DrawInTopRight(specialConditionRect, plusTex, 2f, 0.4f);
                if (Widgets.ButtonInvisible(specialConditionRect))
                {
                    selectedSpecialConditions.Add(specialCondition);
                    this.AddSound();
                    InvalidateAutoListings();
                    FormDirty = true;
                }
                if (Mouse.IsOver(specialConditionRect))
                {
                    TooltipHandler.TipRegion(specialConditionRect, specialCondition.DescriptionShortTranslated);
                }
            }

            AvailableSpecialConditionsListing.End();
            Widgets.EndScrollView();

            // Selected special conditions
            selectedSpecialConditionsRect = PlaywrightDrawHelper.RectWithMargin(selectedSpecialConditionsRect, PanelContentMargin);
            Widgets.DrawBoxSolidWithOutline(selectedSpecialConditionsRect, PanelSelectionsBGColor, PanelOutlineColor, PanelOutlineWidth);
            selectedSpecialConditionsRect.x += Margin;
            selectedSpecialConditionsRect.width -= Margin;
            Rect selectedSpecialConditionsRectInner = new Rect(selectedSpecialConditionsRect);
            selectedSpecialConditionsRectInner.width -= Margin;
            selectedSpecialConditionsRectInner = SelectedSpecialConditionsListing.GetScrollViewInnerRect(selectedSpecialConditionsRectInner);

            Widgets.BeginScrollView(selectedSpecialConditionsRect, ref SelectedSpecialConditionsScrollPos, selectedSpecialConditionsRectInner);
            SelectedSpecialConditionsListing.Begin(selectedSpecialConditionsRectInner);

            foreach (SpecialConditionComponent specialCondition in selectedSpecialConditions.ToList())
            {
                Rect deleteButtonRect = SelectedSpecialConditionsListing.GetRect(0f);
                deleteButtonRect.height = 50f;
                deleteButtonRect = PlaywrightDrawHelper.RectWithMargin(deleteButtonRect, 5f);
                specialCondition.DoSpecialConditionContents(SelectedSpecialConditionsListing);
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 2f, 0.4f))
                {
                    selectedSpecialConditions.Remove(specialCondition);
                    RemoveSound();
                    InvalidateAutoListings();
                    FormDirty = true;
                }

                if (specialCondition.HasHelp)
                {
                    Rect helpButtonRect = new Rect(deleteButtonRect);
                    helpButtonRect.width -= 32f;
                    if (PlaywrightDrawHelper.DrawButtonInTopRight(helpButtonRect, infoTex, 2f, 0.4f))
                    {
                        Find.WindowStack.Add(new InfoPopupWindow($"Playwright.Components.{specialCondition.Id}.Help".Translate()));
                    }
                }

                Rect lineRect = SelectedSpecialConditionsListing.GetRect(PanelOutlineWidth);
                PlaywrightDrawHelper.DrawBottomLine(lineRect, PanelOutlineColor, PanelOutlineWidth);
            }

            SelectedSpecialConditionsListing.End();
            Widgets.EndScrollView();
        }

        private void DrawSummaryTab(Rect contentRect)
        {
            // Summary heading
            Rect summaryIntroRect = new Rect(contentRect);
            summaryIntroRect.height *= 0.1f;
            SummaryListing.Begin(summaryIntroRect);
            SummaryListing.Label("Playwright.Tabs.Summary.Intro".Translate());
            SummaryListing.End();

            Rect summaryRect = new Rect(contentRect);
            summaryRect.height *= 0.9f;
            summaryRect.y += summaryIntroRect.height;
            Widgets.DrawBoxSolidWithOutline(summaryRect, PanelSelectionsBGColor, PanelOutlineColor, PanelOutlineWidth);

            Rect summaryContentRect = PlaywrightDrawHelper.RectWithMargin(summaryRect, 10f);
            SummaryContentListing.Begin(summaryContentRect);

            if (SummaryContentListing.ButtonText("Playwright.RandomizeName".Translate(), widthPct: 0.25f))
            {
                ClickSound();
                this.PlaywrightStructure.Name = NameGenerator.GenerateName(RulePackDefOf.NamerScenario, null, false, null, null, null);
            }

            SummaryContentListing.Label("Playwright.Tabs.Summary.Name".Translate());
            this.PlaywrightStructure.Name = SummaryContentListing.TextEntry(this.PlaywrightStructure.Name, 1);
            SummaryContentListing.Label("Playwright.Tabs.Summary.DescriptionShort".Translate());
            this.PlaywrightStructure.DescriptionShort = SummaryContentListing.TextEntry(this.PlaywrightStructure.DescriptionShort, 1);
            SummaryContentListing.Label("Playwright.Tabs.Summary.Description".Translate());
            this.PlaywrightStructure.Description = SummaryContentListing.TextEntry(this.PlaywrightStructure.Description, 8);
            SummaryContentListing.Label("Playwright.Tabs.Summary.GameStartDialog".Translate());
            this.PlaywrightStructure.GameStartDialogText = SummaryContentListing.TextEntry(this.PlaywrightStructure.GameStartDialogText, 6);

            if (SummaryContentListing.ButtonText("Playwright.Reset".Translate(), widthPct: 0.25f))
            {
                Find.WindowStack.Add(new ConfirmResetSummaryWindow(() =>
                {
                    ResetSummaryToOriginDefaults();
                }));
            }

            SummaryContentListing.End();
        }

        private void DrawReviewAndSaveTab(Rect contentRect)
        {
            // Review heading
            Rect reviewIntroRect = new Rect(contentRect);
            reviewIntroRect.height *= 0.1f;
            ReviewListing.Begin(reviewIntroRect);
            ReviewListing.Label("Playwright.Tabs.ReviewAndSave.Intro".Translate());
            ReviewListing.End();

            Rect reviewRect = new Rect(contentRect);
            reviewRect.height *= 0.9f;
            reviewRect.y += reviewIntroRect.height;
            Widgets.DrawBoxSolidWithOutline(reviewRect, PanelSelectionsBGColor, PanelOutlineColor, PanelOutlineWidth);

            Rect reviewContentRect = PlaywrightDrawHelper.RectWithMargin(reviewRect, 10f);

            PlaywrightBuilder builder = new PlaywrightBuilder();
            Scenario previewScenario = null;
            string errorText = null;
            bool hasError = false;
            try
            {
                previewScenario = builder.MakeScenario(this.PlaywrightStructure, true);
            }
            catch (PlaywrightBuilderException ex)
            {
                hasError = true;
                errorText = ex.Message;
            }
            catch (Exception ex)
            {
                Log.Error("[Playwright] An error occurred while previewing the generated scenario:");
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                // Have to draw this here if we want to rethrow the exception (and we DO want to rethrow it!)
                // Since we rethrow, we don't have to set errorText, we just bail
                ReviewErrorListing.Begin(reviewContentRect);
                ReviewErrorListing.Label("Playwright.Tabs.ReviewAndSave.ErrorUnknownException".Translate());
                ReviewErrorListing.End();
                throw;
            }

            if (hasError)
            {
                ReviewErrorListing.Begin(reviewContentRect);
                ReviewErrorListing.Label("Playwright.Tabs.ReviewAndSave.ErrorBuilderException".Translate());
                ReviewErrorListing.Label(errorText);
                ReviewErrorListing.End();
                return;
            }

            // If we got this far, we have a working scenario
            Rect reviewContentRectInner = new Rect(reviewContentRect);
            reviewContentRectInner.width -= Margin;
            reviewContentRectInner = ReviewContentListing.GetScrollViewInnerRect(reviewContentRectInner);
            Widgets.BeginScrollView(reviewContentRect, ref ReviewContentScrollPos, reviewContentRectInner);
            ReviewContentListing.Begin(reviewContentRectInner);

            Text.Font = GameFont.Medium;
            ReviewContentListing.Label(previewScenario.name);
            Text.Font = GameFont.Small;

            ReviewContentListing.Label(previewScenario.GetFullInformationText());

            ReviewContentListing.End();
            Widgets.EndScrollView();
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
                FormDirty = false;
            }

            Rect loadRect = new Rect(saveRect.xMax + ButtonMargin, buttonY, ButtonWidth, ButtonHeight);
            if (Widgets.ButtonText(loadRect, "Load".Translate()))
            {
                Find.WindowStack.Add(new Dialog_PlaywrightList_Load((PlaywrightStructure loadedStructure) =>
                {
                    this.PlaywrightStructure = loadedStructure;
                    InvalidateAutoListings();
                    FormDirty = false;
                }));
            }

            if (ActiveTab == Tabs.ReviewAndSave)
            {
                Rect generateRect = new Rect(contentRect.xMax - ButtonWidth - ButtonMargin, buttonY, ButtonWidth, ButtonHeight);
                if (Widgets.ButtonText(generateRect, "Playwright.CreateScenario".Translate()))
                {
                    this.CompileScenario();
                }
            }
        }

        private void ResetSummaryToOriginDefaults()
        {
            this.PlaywrightStructure.Name = this.PlaywrightStructure.Origin?.NameTranslated;
            this.PlaywrightStructure.DescriptionShort = this.PlaywrightStructure.Origin?.DescriptionShortTranslated;
            this.PlaywrightStructure.Description = this.PlaywrightStructure.Origin?.DescriptionTranslated;
        }

        private void CompileScenario()
        {
            PlaywrightBuilder builder = new PlaywrightBuilder();
            try
            {
                Scenario scenario = builder.MakeScenario(this.PlaywrightStructure, false);
                Find.WindowStack.Add(new Dialog_ScenarioList_Save(scenario, () =>
                {
                    FormDirty = false;
                    Find.WindowStack.Add(new ScenarioSavedPopupWindow((bool shouldGoToNewGame) =>
                    {
                        if (shouldGoToNewGame)
                        {
                            // Call base close class to bypass our FormDirty checks
                            base.Close(true);
                            Find.WindowStack.Add(new Page_SelectScenario());
                            // I have attempted to pre-select the scenario that was just saved,
                            // but no matter what I do, I can't get the selection to work properly.
                            // It seems to pre-select a different copy of the just saved scenario instead, even if I first reload the scenario from the lister,
                            // and even if the lister is marked dirty. I give up.
                        }
                    }));
                }));
            }
            catch (PlaywrightBuilderException ex)
            {
                Log.Warning("[Playwright] Builder error while generating scenario: " + ex.Message);
                Log.Warning(ex.StackTrace);
                Find.WindowStack.Add(new InfoPopupWindow(ex.Message));
            }
            catch (Exception ex)
            {
                Log.Error("[Playwright] Error generating scenario: " + ex.Message);
                Log.Error(ex.StackTrace);
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ErrorSavingScenario".Translate()));
                throw;
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

        /// <summary>
        /// Runs <paramref name="onConfirmed"/>, but asks with a confirm reset popup first if the form is dirty.
        /// Also sets <see cref="FormDirty"/> back to false if reset was confirmed.
        /// Used to ask the user if they want to discard their unsaved changes before doing something.
        /// </summary>
        public void ConfirmReset(Action onConfirmed)
        {
            if (!FormDirty)
            {
                onConfirmed();
                return;
            }

            Find.WindowStack.Add(new ConfirmResetWindow(() =>
            {
                onConfirmed();
                FormDirty = false;
            }));
        }

        /// <summary>
        /// Recalculate the height of all AutoFit listings in the UI, for when the playwright structure is changed externally
        /// (such as when a saved playwright is loaded, or the origin is changed)
        /// </summary>
        public void InvalidateAutoListings()
        {
            this.OriginsListing.Invalidate();
            this.OriginContentListing.Invalidate();
            this.AvailableBoonsListing.Invalidate();
            this.SelectedBoonsListing.Invalidate();
            this.AllyFactionsListing.Invalidate();
            this.NeutralFactionsListing.Invalidate();
            this.EnemyFactionsListing.Invalidate();
            this.AvailableWinConditionsListing.Invalidate();
            this.SelectedWinConditionsListing.Invalidate();
            this.AvailableSpecialConditionsListing.Invalidate();
            this.SelectedSpecialConditionsListing.Invalidate();
            this.ReviewContentListing.Invalidate();
        }

        public override void Close(bool doCloseSound = true)
        {
            if (!FormDirty)
            {
                base.Close(doCloseSound);
                return;
            }

            Find.WindowStack.Add(new ConfirmCloseWindow(() =>
            {
                base.Close(doCloseSound);
            }));
        }

        public override void OnAcceptKeyPressed()
        {
            // This window can have multiline text input fields, we should do absolutely nothing on enter press
        }

        public override void OnCancelKeyPressed()
        {
            // Will ask the player to save first
            this.Close(true);
        }

        public enum Tabs
        {
            Intro,
            Origin,
            Boons,
            Factions,
            WinConditions,
            SpecialConditions,
            Summary,
            ReviewAndSave
        }
    }
}
