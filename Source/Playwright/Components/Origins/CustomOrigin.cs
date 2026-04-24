using HarmonyLib;
using RimWorld;
using Rokk.Playwright.DefOfs;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    /// <summary>
    /// Customizable origin.
    /// </summary>
    public class CustomOrigin : OriginComponent
    {
        public override string Id => "Origins.Custom";
        public override string SummaryTranslated => "";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.Playwright_BlankScenario;

        public FactionDef Faction = RimWorld.FactionDefOf.PlayerColony;
        public PlayerPawnsArriveMethod ArriveMethod = PlayerPawnsArriveMethod.DropPods;
        public string FactionLabel => Faction != null ? Faction.LabelCap.ToString() : "-";

        public int PawnChoiceCount = 8;
        private string PawnChoiceCountBuffer = "8";

        public ConfigPageType ConfigPage = ConfigPageType.Default;
        public string ConfigPageLabel => GetConfigPageTypeLabel(ConfigPage);
        
        // Default config page
        public int PawnCount = 3;
        private string PawnCountBuffer = "3";

        // Xenotypes config page
        public List<XenotypeCount> PawnXenotypeCounts = new List<XenotypeCount>()
        {
            new XenotypeCount()
            {
                xenotype = XenotypeDefOf.Baseliner,
                count = 3,
                countBuffer = "3",
                requiredAtStart = false
            }
        };

        // Pawnkinds config page
        public List<PawnKindCount> PawnKindCounts = new List<PawnKindCount>()
        {
            new PawnKindCount()
            {
                kindDef = RimWorld.PawnKindDefOf.Colonist,
                count = 3,
                countBuffer = "3",
                requiredAtStart = false
            }
        };

        // Mutants config page
        public List<MutantCount> PawnMutantCounts = new List<MutantCount>()
        {
            new MutantCount()
            {
                mutant = null,
                count = 3,
                countBuffer = "3",
                requiredAtStart = false
            }
        };

        // Starting/Scattered Nearby/Scattered Anywhere things
        public List<ExtraItemEntry> StartingThings = new List<ExtraItemEntry>();
        public List<ExtraItemEntry> ScatteredNearbyThings = new List<ExtraItemEntry>();
        public List<ExtraItemEntry> ScatteredAnywhereThings = new List<ExtraItemEntry>();

        private List<ConfigPageType> GetAllowedConfigPages()
        {
            var result = new List<ConfigPageType>()
            {
                ConfigPageType.Default
            };
            if (ModsConfig.BiotechActive)
            {
                result.Add(ConfigPageType.Xenotypes);
                result.Add(ConfigPageType.PawnKinds);
            }
            if (ModsConfig.AnomalyActive)
            {
                result.Add(ConfigPageType.Mutants);
            }
            return result;
        }

        private IEnumerable<ThingDef> PossibleThingDefs()
        {
            return DefDatabase<ThingDef>.AllDefs
                .Where(thingDef => 
                    (thingDef.category == ThingCategory.Item && thingDef.scatterableOnMapGen && !thingDef.destroyOnDrop)
                    || (thingDef.category == ThingCategory.Building && thingDef.Minifiable)
                    || thingDef.defName == "ShipChunk") // Specifically ship chunks are added by the default scenarios but can't be selected normally
                .OrderBy(thingDef => thingDef.label);
        }

        private IEnumerable<ThingDef> PossibleStuffForThing(ThingDef thing)
        {
            return GenStuff.AllowedStuffsFor(thing)
                .OrderBy(thingDef => thingDef.label);
        }

        private IEnumerable<FactionDef> PossiblePlayerFactions()
        {
            return FactionUtils.GetPlayerFactions();
        }

        private IEnumerable<PawnKindDef> PossiblePlayerPawnKinds()
        {
            return DefDatabase<PawnKindDef>.AllDefs
                .Where(kindDef => kindDef.RaceProps.Humanlike && kindDef.defaultFactionDef != null && kindDef.defaultFactionDef.isPlayer);
        }

        private IEnumerable<MutantDef> PossiblePlayerMutants()
        {
            return DefDatabase<MutantDef>.AllDefs
                .Where(mutantDef => mutantDef.showInScenarioEditor);
        }

        private string GetMutantLabel(MutantDef mutantDef)
        {
            if (mutantDef == null)
            {
                return "None".Translate();
            }
            return mutantDef.LabelCap;
        }

        private string GetArrivalMethodLabel()
        {
            return GetArrivalMethodLabel(ArriveMethod);
        }

        private string GetArrivalMethodLabel(PlayerPawnsArriveMethod arriveMethod)
        {
            return ("PlayerPawnsArriveMethod_" + arriveMethod.ToString()).Translate();
        }

        private string GetConfigPageTypeLabel(ConfigPageType configPageType)
        {
            return ("Playwright.Components.Origins.Custom.ConfigPageType." + configPageType.ToString()).Translate();
        }

        public override void DoSettingsContents(Listing_AutoFitVertical originContentListing)
        {
            base.DoSettingsContents(originContentListing);

            // Import from existing scenario button,
            // to make it easier to quickly set up an origin without filling in like 20 starting items manually
            if (originContentListing.ButtonText("Playwright.Components.Origins.Custom.ImportFromScenario".Translate(), widthPct: 0.25f))
            {
                var options = new List<FloatMenuOption>();
                foreach (var scenarioDef in GetImportableScenarios())
                {
                    options.Add(new FloatMenuOption(scenarioDef.LabelCap, () =>
                    {
                        ImportFromScenario(scenarioDef);
                        originContentListing.InvalidateGroup();
                    }));
                }
                PlaywrightUtils.OpenFloatMenu(options);
                SoundUtils.PlayClick();
            }

            // Player faction selector
            if (originContentListing.ButtonTextLabeled("Playwright.Components.Origins.Custom.PlayerFaction".Translate(), FactionLabel))
            {
                var options = new List<FloatMenuOption>();
                foreach (FactionDef playerFaction in PossiblePlayerFactions())
                {
                    options.Add(new FloatMenuOption(playerFaction.LabelCap, () =>
                    {
                        Faction = playerFaction;
                        originContentListing.InvalidateGroup();
                    }));
                }
                PlaywrightUtils.OpenFloatMenu(options);
                SoundUtils.PlayClick();
            }

            // Arrival method selector
            if (originContentListing.ButtonTextLabeled("Playwright.Components.Origins.Custom.ArrivalMethod".Translate(), GetArrivalMethodLabel()))
            {
                var options = new List<FloatMenuOption>();
                foreach (PlayerPawnsArriveMethod arriveMethod in Enum.GetValues(typeof(PlayerPawnsArriveMethod)))
                {
                    options.Add(new FloatMenuOption(GetArrivalMethodLabel(arriveMethod), () =>
                    {
                        ArriveMethod = arriveMethod;
                        originContentListing.InvalidateGroup();
                    }));
                }
                PlaywrightUtils.OpenFloatMenu(options);
                SoundUtils.PlayClick();
            }

            // Configpage selector
            var configPages = GetAllowedConfigPages();
            if (configPages.Count > 1)
            {
                if (originContentListing.ButtonTextLabeled("Playwright.Components.Origins.Custom.ConfigPage".Translate(), ConfigPageLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (ConfigPageType configPageType in configPages)
                    {
                        options.Add(new FloatMenuOption(GetConfigPageTypeLabel(configPageType), () =>
                        {
                            ConfigPage = configPageType;
                            originContentListing.InvalidateGroup();
                        }));
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }
            }
            else
            {
                ConfigPage = configPages.FirstOrDefault();
            }

            originContentListing.GapLine();

            // Render config page type specific settings
            switch (ConfigPage)
            {
                case ConfigPageType.Default:
                    DoDefaultConfigPage(originContentListing);
                    break;
                case ConfigPageType.Xenotypes:
                    DoXenotypesConfigPage(originContentListing);
                    break;
                case ConfigPageType.PawnKinds:
                    DoPawnKindsConfigPage(originContentListing);
                    break;
                case ConfigPageType.Mutants:
                    DoMutantsConfigPage(originContentListing);
                    break;
            }

            // Do lists of things started with/scattered nearby/scattered anywhere
            originContentListing.Label("Playwright.Components.Origins.Custom.StartingThings".Translate());
            DoItems(originContentListing, StartingThings, "Playwright.Components.Origins.Custom.AddStartingThing");
            originContentListing.GapLine();
            originContentListing.Label("Playwright.Components.Origins.Custom.ScatteredNearbyThings".Translate());
            DoItems(originContentListing, ScatteredNearbyThings, "Playwright.Components.Origins.Custom.AddScatteredNearbyThing");
            originContentListing.GapLine();
            originContentListing.Label("Playwright.Components.Origins.Custom.ScatteredAnywhereThings".Translate());
            DoItems(originContentListing, ScatteredAnywhereThings, "Playwright.Components.Origins.Custom.AddScatteredAnywhereThings");
        }

        private void DoDefaultConfigPage(Listing_AutoFitVertical originContentListing)
        {
            // Divided into "Choose, (3), out of, (8)"
            Rect rect = originContentListing.GetRect(35f);
            float colWidth = rect.width * 0.25f;

            // Choose
            Rect chooseRect = new Rect(rect)
            {
                width = colWidth
            };
            string chooseText = "Playwright.Components.Origins.Custom.Choose".Translate();
            Rect chooseRectCentered = PlaywrightDrawHelper.GetCenteredLabelRect(chooseRect, chooseText);
            Widgets.Label(chooseRectCentered, chooseText);
            // 3
            Rect pawnCountRect = new Rect(rect)
            {
                width = colWidth,
                x = rect.x + colWidth
            };
            Widgets.TextFieldNumeric(pawnCountRect, ref PawnCount, ref PawnCountBuffer, 1);
            // Out of
            Rect outOfRect = new Rect(rect)
            {
                width = colWidth,
                x = rect.x + colWidth * 2
            };
            string outOfText = "Playwright.Components.Origins.Custom.XOutOfY".Translate();
            Rect outOfRectCentered = PlaywrightDrawHelper.GetCenteredLabelRect(outOfRect, outOfText);
            Widgets.Label(outOfRectCentered, outOfText);
            // 8
            Rect pawnChoiceCountRect = new Rect(rect)
            {
                width = colWidth,
                x = rect.x + colWidth * 3
            };
            Widgets.TextFieldNumeric(pawnChoiceCountRect, ref PawnChoiceCount, ref PawnChoiceCountBuffer, 1);
        }

        private void DoXenotypesConfigPage(Listing_AutoFitVertical originContentListing)
        {
            if (!ModsConfig.BiotechActive)
            {
                return;
            }

            float oldWidth = originContentListing.ColumnWidth;
            originContentListing.ColumnWidth *= 0.8f;

            originContentListing.TextFieldNumericLabeled("Playwright.Components.Origins.Custom.Choices".Translate(), ref PawnChoiceCount, ref PawnChoiceCountBuffer, 1);
            if (originContentListing.ButtonText("Playwright.Components.Origins.Custom.AddXenotypeCount".Translate()))
            {
                PawnXenotypeCounts.Add(new XenotypeCount()
                {
                    count = 1,
                    countBuffer = "1",
                    requiredAtStart = false,
                    xenotype = XenotypeDefOf.Baseliner
                });
                SoundUtils.PlayAdd();
            }

            foreach (XenotypeCount xenotypeCount in PawnXenotypeCounts.ToList())
            {
                originContentListing.TextFieldNumericLabeled("Playwright.Count".Translate(), ref xenotypeCount.count, ref xenotypeCount.countBuffer, PawnChoiceCount);
                Rect deleteButtonRect = originContentListing.GetRect(0f);
                deleteButtonRect.height = 25f;
                if (originContentListing.ButtonText(xenotypeCount.xenotype.LabelCap, widthPct: 0.75f))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (var xenotypeDef in DefDatabase<XenotypeDef>.AllDefsListForReading)
                    {
                        options.Add(new FloatMenuOption(xenotypeDef.LabelCap, () =>
                        {
                            xenotypeCount.xenotype = xenotypeDef;
                            originContentListing.InvalidateGroup();
                        }, xenotypeDef.Icon, Color.white));
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }
                originContentListing.CheckboxLabeled("Required".Translate(), ref xenotypeCount.requiredAtStart, "Playwright.Components.Origins.Custom.RequiredAtStart.Help".Translate());

                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, TextureUtils.DeleteButtonTex, 0f, 0.4f))
                {
                    PawnXenotypeCounts.Remove(xenotypeCount);
                    originContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }

                originContentListing.GapLine();
            }

            originContentListing.ColumnWidth = oldWidth;
        }

        private void DoPawnKindsConfigPage(Listing_AutoFitVertical originContentListing)
        {
            if (!ModsConfig.BiotechActive)
            {
                return;
            }

            float oldWidth = originContentListing.ColumnWidth;
            originContentListing.ColumnWidth *= 0.8f;

            originContentListing.TextFieldNumericLabeled("Playwright.Components.Origins.Custom.Choices".Translate(), ref PawnChoiceCount, ref PawnChoiceCountBuffer, 1);
            if (originContentListing.ButtonText("Playwright.Components.Origins.Custom.AddPawnKindCount".Translate()))
            {
                PawnKindCounts.Add(new PawnKindCount()
                {
                    count = 1,
                    countBuffer = "1",
                    requiredAtStart = false,
                    kindDef = RimWorld.PawnKindDefOf.Colonist
                });
                SoundUtils.PlayAdd();
            }

            foreach (PawnKindCount pawnKindCount in PawnKindCounts.ToList())
            {
                originContentListing.TextFieldNumericLabeled("Playwright.Count".Translate(), ref pawnKindCount.count, ref pawnKindCount.countBuffer, PawnChoiceCount);
                Rect deleteButtonRect = originContentListing.GetRect(0f);
                deleteButtonRect.height = 25f;
                if (originContentListing.ButtonText(pawnKindCount.kindDef.LabelCap, widthPct: 0.75f))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (var pawnKindDef in PossiblePlayerPawnKinds())
                    {
                        options.Add(new FloatMenuOption(pawnKindDef.LabelCap, () =>
                        {
                            pawnKindCount.kindDef = pawnKindDef;
                            originContentListing.InvalidateGroup();
                        }));
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }
                originContentListing.CheckboxLabeled("Required".Translate(), ref pawnKindCount.requiredAtStart, "Playwright.Components.Origins.Custom.RequiredAtStart.Help".Translate());

                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, TextureUtils.DeleteButtonTex, 0f, 0.4f))
                {
                    PawnKindCounts.Remove(pawnKindCount);
                    originContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }

                originContentListing.GapLine();
            }

            originContentListing.ColumnWidth = oldWidth;
        }

        private void DoMutantsConfigPage(Listing_AutoFitVertical originContentListing)
        {
            if (!ModsConfig.AnomalyActive)
            {
                return;
            }

            float oldWidth = originContentListing.ColumnWidth;
            originContentListing.ColumnWidth *= 0.8f;

            originContentListing.TextFieldNumericLabeled("Playwright.Components.Origins.Custom.Choices".Translate(), ref PawnChoiceCount, ref PawnChoiceCountBuffer, 1);
            if (originContentListing.ButtonText("Playwright.Components.Origins.Custom.AddMutantCount".Translate()))
            {
                PawnMutantCounts.Add(new MutantCount()
                {
                    count = 1,
                    countBuffer = "1",
                    requiredAtStart = false,
                    mutant = null
                });
                SoundUtils.PlayAdd();
            }

            foreach (MutantCount mutantCount in PawnMutantCounts.ToList())
            {
                originContentListing.TextFieldNumericLabeled("Playwright.Count".Translate(), ref mutantCount.count, ref mutantCount.countBuffer, PawnChoiceCount);
                Rect deleteButtonRect = originContentListing.GetRect(0f);
                deleteButtonRect.height = 25f;
                if (originContentListing.ButtonText(GetMutantLabel(mutantCount.mutant), widthPct: 0.75f))
                {
                    var options = new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("None".Translate(), () =>
                        {
                            mutantCount.mutant = null;
                            originContentListing.Invalidate();
                        })
                    };

                    foreach (var mutantDef in PossiblePlayerMutants())
                    {
                        options.Add(new FloatMenuOption(mutantDef.LabelCap, () =>
                        {
                            mutantCount.mutant = mutantDef;
                            originContentListing.InvalidateGroup();
                        }));
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }
                originContentListing.CheckboxLabeled("Required".Translate(), ref mutantCount.requiredAtStart, "Playwright.Components.Origins.Custom.RequiredAtStart.Help".Translate());

                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, TextureUtils.DeleteButtonTex, 0f, 0.4f))
                {
                    PawnMutantCounts.Remove(mutantCount);
                    originContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }

                originContentListing.GapLine();
            }

            originContentListing.ColumnWidth = oldWidth;
        }

        private void DoItems(Listing_AutoFitVertical originContentListing, List<ExtraItemEntry> items, string addKey)
        {
            // Lots of stuff taken from ScenPart_ThingCount
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            if (originContentListing.ButtonText(addKey.Translate()))
            {
                items.Add(new ExtraItemEntry());
                SoundUtils.PlayAdd();
            }

            foreach (ExtraItemEntry item in items.ToList())
            {
                Rect rect = originContentListing.GetRect(35f);
                // UI divided into Count (Thing) (Stuff) (Quality) each 24% wide, last 4% taken up by (DeleteButton)
                float colWidth = rect.width * 0.24f;

                // Count
                Rect countRect = new Rect(rect);
                countRect.width = colWidth;
                Widgets.TextFieldNumericLabeled(countRect, "Playwright.Components.Boons.ExtraItems.Count".Translate(), ref item.Count, ref item.CountBuffer, 1);

                // Thing
                Rect thingRect = new Rect(rect);
                thingRect.width = colWidth;
                thingRect.x += colWidth;
                if (Widgets.ButtonText(thingRect, item.ThingLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (ThingDef thingDef in PossibleThingDefs())
                    {
                        options.Add(new FloatMenuOption(thingDef.LabelCap, () => item.Thing = thingDef, thingDef.uiIcon, thingDef.uiIconColor));
                    }
                    SoundUtils.PlayClick();
                    PlaywrightUtils.OpenFloatMenu(options);
                }

                // Stuff (only if the Thing supports it)
                if (item.Thing?.MadeFromStuff == true)
                {
                    Rect stuffRect = new Rect(rect);
                    stuffRect.width = colWidth;
                    stuffRect.x += colWidth * 2;
                    if (Widgets.ButtonText(stuffRect, item.StuffLabel))
                    {
                        var options = new List<FloatMenuOption>();
                        foreach (ThingDef stuffThingDef in PossibleStuffForThing(item.Thing))
                        {
                            options.Add(new FloatMenuOption(stuffThingDef.LabelCap, () => item.Stuff = stuffThingDef, stuffThingDef.uiIcon, stuffThingDef.uiIconColor));
                        }
                        SoundUtils.PlayClick();
                        PlaywrightUtils.OpenFloatMenu(options);
                    }
                }
                else
                {
                    item.Stuff = null;
                }

                // Quality (only if the Thing supports it)
                if (item.Thing?.HasComp(typeof(CompQuality)) == true)
                {
                    Rect qualityRect = new Rect(rect);
                    qualityRect.width = colWidth;
                    qualityRect.x += colWidth * 3;
                    if (Widgets.ButtonText(qualityRect, item.QualityLabel))
                    {
                        var options = new List<FloatMenuOption>()
                        {
                            new FloatMenuOption("Default".Translate().CapitalizeFirst(), () => item.Quality = null)
                        };
                        foreach (QualityCategory qualityCategory in QualityUtility.AllQualityCategories)
                        {
                            options.Add(new FloatMenuOption(qualityCategory.GetLabel().CapitalizeFirst(), () => item.Quality = qualityCategory));
                        }
                        SoundUtils.PlayClick();
                        PlaywrightUtils.OpenFloatMenu(options);
                    }
                }
                else
                {
                    item.Quality = null;
                }

                // Delete button
                Rect deleteButtonRect = new Rect(rect)
                {
                    width = rect.width * 0.04f,
                    x = rect.x + colWidth * 4
                };
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    items.Remove(item);
                    SoundUtils.PlayRemove();
                    originContentListing.InvalidateGroup();
                }
            }
        }

        public override void DoAdditionalContents(Listing_AutoFitVertical originContentListing)
        {
            // Nothing, since we can show the tech level by the faction instead.
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            // Set config page
            ScenPart_ConfigPage_ConfigureStartingPawnsBase configPagePart;
            if (ConfigPage == ConfigPageType.Xenotypes)
            {
                configPagePart = ScenPartUtility.MakeConfigureStartingPawnsXenotypesPart(PawnXenotypeCounts, PawnChoiceCount);
            }
            else if (ConfigPage == ConfigPageType.PawnKinds)
            {
                configPagePart = ScenPartUtility.MakeConfigureStartingPawnsKindDefsPart(PawnKindCounts, PawnChoiceCount);
            }
            else if (ConfigPage == ConfigPageType.Mutants)
            {
                configPagePart = ScenPartUtility.MakeConfigureStartingPawnsMutantsPart(PawnMutantCounts, PawnChoiceCount);
            }
            else
            {
                configPagePart = ScenPartUtility.MakeConfigureStartingPawnsPart(PawnCount, PawnChoiceCount);
            }
            scenarioParts.Add(configPagePart);

            // Player faction is an internal field, not in the list of parts
            FieldInfo playerFactionInfo = AccessTools.Field(typeof(Scenario), "playerFaction");
            playerFactionInfo.SetValue(scenario, ScenPartUtility.MakePlayerFactionPart(Faction));

            // Set arrival method
            scenarioParts.Add(ScenPartUtility.MakePlayerPawnsArriveMethodPart(ArriveMethod));

            // Add starting things
            foreach (var startingThing in StartingThings)
            {
                scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(startingThing.Thing, startingThing.Stuff, startingThing.Count, startingThing.Quality));
            }
            // Add scattered nearby things
            foreach (var scatteredNearbyThing in ScatteredNearbyThings)
            {
                scenarioParts.Add(ScenPartUtility.MakeScatterThingsNearPlayerPart(scatteredNearbyThing.Thing, scatteredNearbyThing.Stuff, scatteredNearbyThing.Count, scatteredNearbyThing.Quality));
            }
            // Add scattered anywhere things
            foreach (var scatteredAnywhereThing in ScatteredAnywhereThings)
            {
                scenarioParts.Add(ScenPartUtility.MakeScatterThingsAnywherePart(scatteredAnywhereThing.Thing, scatteredAnywhereThing.Stuff, scatteredAnywhereThing.Count, scatteredAnywhereThing.Quality));
            }
        }

        protected virtual IEnumerable<ScenarioDef> GetImportableScenarios()
        {
            return DefDatabase<ScenarioDef>.AllDefsListForReading
                    .Where(def => def.scenario.showInUI);
        }

        protected virtual void ImportFromScenario(ScenarioDef scenarioDef)
        {
            // Configpage
            ScenPart_ConfigPage_ConfigureStartingPawnsBase configPage = ScenPartUtility.GetConfigureStartingPawnsPart(scenarioDef.scenario);
            if (configPage != null)
            {
                PawnChoiceCount = configPage.pawnChoiceCount;
                PawnChoiceCountBuffer = PawnChoiceCount.ToString();
                if (configPage is ScenPart_ConfigPage_ConfigureStartingPawns)
                {
                    var configPagePawns = configPage as ScenPart_ConfigPage_ConfigureStartingPawns;
                    ConfigPage = ConfigPageType.Default;
                    PawnCount = configPagePawns.pawnCount;
                    PawnCountBuffer = PawnCount.ToString();
                }
                else if (configPage is ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes)
                {
                    var configPageXenotypes = configPage as ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes;
                    ConfigPage = ConfigPageType.Xenotypes;
                    PawnXenotypeCounts = configPageXenotypes.xenotypeCounts;
                }
                else if (configPage is ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs)
                {
                    var configPageKindDefs = configPage as ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs;
                    ConfigPage = ConfigPageType.PawnKinds;
                    PawnKindCounts = configPageKindDefs.kindCounts;
                }
                else if (configPage is ScenPart_ConfigPage_ConfigureStartingPawns_Mutants)
                {
                    var configPageMutants = configPage as ScenPart_ConfigPage_ConfigureStartingPawns_Mutants;
                    ConfigPage = ConfigPageType.Mutants;
                    PawnMutantCounts = configPageMutants.mutantCounts;
                }
            }

            // Player faction
            // Is an internal field, not in the list of parts
            FieldInfo playerFactionInfo = AccessTools.Field(typeof(Scenario), "playerFaction");
            ScenPart_PlayerFaction playerFaction = playerFactionInfo.GetValue(scenarioDef.scenario) as ScenPart_PlayerFaction;
            if (playerFaction != null)
            {
                FieldInfo factionDefInfo = AccessTools.Field(typeof(ScenPart_PlayerFaction), "factionDef");
                this.Faction = (FactionDef)factionDefInfo.GetValue(playerFaction);
            }

            // Arrival method
            ScenPart_PlayerPawnsArriveMethod arriveMethod = scenarioDef.scenario.AllParts
                .Where(part => part.def == RimWorld.ScenPartDefOf.PlayerPawnsArriveMethod)
                .Cast<ScenPart_PlayerPawnsArriveMethod>()
                .FirstOrDefault();
            if (arriveMethod != null)
            {
                FieldInfo arrivalMethodInfo = AccessTools.Field(typeof(ScenPart_PlayerPawnsArriveMethod), "method");
                this.ArriveMethod = (PlayerPawnsArriveMethod)arrivalMethodInfo.GetValue(arriveMethod);
            }

            // Starting things
            FieldInfo thingDefInfo = AccessTools.Field(typeof(ScenPart_StartingThing_Defined), "thingDef");
            FieldInfo stuffInfo = AccessTools.Field(typeof(ScenPart_StartingThing_Defined), "stuff");
            FieldInfo qualityInfo = AccessTools.Field(typeof(ScenPart_StartingThing_Defined), "quality");
            FieldInfo countInfo = AccessTools.Field(typeof(ScenPart_StartingThing_Defined), "count");
            List<ScenPart_StartingThing_Defined> startingThings = scenarioDef.scenario.AllParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.StartingThing_Defined)
                .Cast<ScenPart_StartingThing_Defined>()
                .ToList();
            this.StartingThings.Clear();
            foreach (var part in startingThings)
            {
                ThingDef thingDef = (ThingDef)thingDefInfo.GetValue(part);
                ThingDef stuff = (ThingDef)stuffInfo.GetValue(part);
                QualityCategory? quality = (QualityCategory?)qualityInfo.GetValue(part);
                int count = (int)countInfo.GetValue(part);
                this.StartingThings.Add(new ExtraItemEntry()
                {
                    Count = count,
                    CountBuffer = count.ToString(),
                    Thing = thingDef,
                    Stuff = stuff,
                    Quality = quality
                });
            }

            // Scattered nearby things
            thingDefInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsNearPlayerStart), "thingDef");
            stuffInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsNearPlayerStart), "stuff");
            qualityInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsNearPlayerStart), "quality");
            countInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsNearPlayerStart), "count");
            List<ScenPart_ScatterThingsNearPlayerStart> scatteredThingsNearby = scenarioDef.scenario.AllParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.ScatterThingsNearPlayerStart)
                .Cast<ScenPart_ScatterThingsNearPlayerStart>()
                .ToList();
            this.ScatteredNearbyThings.Clear();
            foreach (var part in scatteredThingsNearby)
            {
                ThingDef thingDef = (ThingDef)thingDefInfo.GetValue(part);
                ThingDef stuff = (ThingDef)stuffInfo.GetValue(part);
                QualityCategory? quality = (QualityCategory?)qualityInfo.GetValue(part);
                int count = (int)countInfo.GetValue(part);
                this.ScatteredNearbyThings.Add(new ExtraItemEntry()
                {
                    Count = count,
                    CountBuffer = count.ToString(),
                    Thing = thingDef,
                    Stuff = stuff,
                    Quality = quality
                });
            }

            // Scattered anywhere things
            thingDefInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsAnywhere), "thingDef");
            stuffInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsAnywhere), "stuff");
            qualityInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsAnywhere), "quality");
            countInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsAnywhere), "count");
            List<ScenPart_ScatterThingsAnywhere> scatteredThingsAnywhere = scenarioDef.scenario.AllParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.ScatterThingsAnywhere)
                .Cast<ScenPart_ScatterThingsAnywhere>()
                .ToList();
            this.ScatteredAnywhereThings.Clear();
            foreach (var part in scatteredThingsAnywhere)
            {
                ThingDef thingDef = (ThingDef)thingDefInfo.GetValue(part);
                ThingDef stuff = (ThingDef)stuffInfo.GetValue(part);
                QualityCategory? quality = (QualityCategory?)qualityInfo.GetValue(part);
                int count = (int)countInfo.GetValue(part);
                this.ScatteredAnywhereThings.Add(new ExtraItemEntry()
                {
                    Count = count,
                    CountBuffer = count.ToString(),
                    Thing = thingDef,
                    Stuff = stuff,
                    Quality = quality
                });
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Faction, nameof(Faction));
            Scribe_Values.Look(ref ArriveMethod, nameof(ArriveMethod), PlayerPawnsArriveMethod.DropPods);
            Scribe_Values.Look(ref PawnChoiceCount, nameof(PawnChoiceCount), 8);
            Scribe_Values.Look(ref ConfigPage, nameof(ConfigPage), ConfigPageType.Default);
            Scribe_Values.Look(ref PawnCount, nameof(PawnCount), 3);
            Scribe_Collections.Look(ref PawnXenotypeCounts, nameof(PawnXenotypeCounts), LookMode.Deep);
            Scribe_Collections.Look(ref PawnKindCounts, nameof(PawnKindCounts), LookMode.Deep);
            Scribe_Collections.Look(ref PawnMutantCounts, nameof(PawnMutantCounts), LookMode.Deep);
            Scribe_Collections.Look(ref StartingThings, nameof(StartingThings), LookMode.Deep);
            Scribe_Collections.Look(ref ScatteredNearbyThings, nameof(ScatteredNearbyThings), LookMode.Deep);
            Scribe_Collections.Look(ref ScatteredAnywhereThings, nameof(ScatteredAnywhereThings), LookMode.Deep);

            PawnChoiceCountBuffer = PawnChoiceCount.ToString();
            PawnCountBuffer = PawnCount.ToString();
        }

        public enum ConfigPageType
        {
            Default,
            Xenotypes,
            PawnKinds,
            Mutants
        }

        public class ExtraItemEntry : IExposable
        {
            public ThingDef Thing;
            public ThingDef Stuff;
            public QualityCategory? Quality;
            /// <summary>
            /// If constructing an instance of this manually, set <see cref="CountBuffer"/> to <see cref="Count"/>.ToString() for the editor to work properly.
            /// </summary>
            public int Count = 1;
            /// <summary>
            /// If constructing an instance of this manually, set this to <see cref="Count"/>.ToString() for the editor to work properly.
            /// </summary>
            public string CountBuffer = "1";

            public string ThingLabel => Thing != null ? Thing.LabelCap.ToString() : "Playwright.Components.Boons.ExtraItems.SelectThing".Translate().ToString();
            public string StuffLabel => Stuff != null ? Stuff.LabelCap.ToString() : "Playwright.Components.Boons.ExtraItems.SelectStuff".Translate().ToString();
            public string QualityLabel => Quality != null ? Quality.Value.GetLabel().CapitalizeFirst() : "Default".Translate().CapitalizeFirst().ToString();

            public void ExposeData()
            {
                Scribe_Defs.Look(ref Thing, nameof(Thing));
                Scribe_Defs.Look(ref Stuff, nameof(Stuff));
                Scribe_Values.Look(ref Quality, nameof(Quality));
                Scribe_Values.Look(ref Count, nameof(Count));
                CountBuffer = Count.ToString();
            }
        }
    }
}
