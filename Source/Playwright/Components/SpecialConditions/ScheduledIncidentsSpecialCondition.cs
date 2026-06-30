using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class ScheduledIncidentsSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.ScheduledIncidents";

        public List<ScheduledIncidentEntry> ScheduledIncidents = new List<ScheduledIncidentEntry>();

        private IEnumerable<IncidentDef> GetAllowedIncidents()
        {
            return DefDatabase<IncidentDef>.AllDefs;
        }

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            if (specialConditionContentListing.ButtonText("Playwright.Components.SpecialConditions.ScheduledIncidents.Add".Translate()))
            {
                ScheduledIncidents.Add(new ScheduledIncidentEntry());
                SoundUtils.PlayAdd();
                specialConditionContentListing.InvalidateGroup();
            }

            DoScheduledIncidentsContents(specialConditionContentListing);
        }

        private void DoScheduledIncidentsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            float oldWidth = specialConditionContentListing.ColumnWidth;
            specialConditionContentListing.ColumnWidth *= 0.9f;

            foreach (var scheduledIncidentEntry in ScheduledIncidents.ToList())
            {
                specialConditionContentListing.GapLine();
                // (select incident), minDays, maxDays, repeat, delete
                Rect deleteButtonRect = specialConditionContentListing.GetRect(0f);
                deleteButtonRect.height = 50f;
                Rect rowRect = specialConditionContentListing.GetRect(35f);

                // (select incident)
                Rect incidentButtonRect = new Rect(rowRect);
                incidentButtonRect.width *= 0.24f;
                if (Widgets.ButtonText(incidentButtonRect, scheduledIncidentEntry.IncidentLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (var incidentDef in GetAllowedIncidents())
                    {
                        options.Add(new FloatMenuOption(incidentDef.LabelCap, () =>
                        {
                            scheduledIncidentEntry.Incident = incidentDef;
                            specialConditionContentListing.InvalidateGroup();
                        }));
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }

                // minDays
                Rect minDaysRect = new Rect(rowRect);
                minDaysRect.width *= 0.24f;
                minDaysRect.x += incidentButtonRect.width;
                Widgets.TextFieldNumericLabeled(minDaysRect, "minDays".Translate().CapitalizeFirst(), ref scheduledIncidentEntry.MinDays, ref scheduledIncidentEntry.MinDaysBuffer);

                // maxDays
                Rect maxDaysRect = new Rect(rowRect);
                maxDaysRect.width *= 0.24f;
                maxDaysRect.x += incidentButtonRect.width + minDaysRect.width;
                Widgets.TextFieldNumericLabeled(maxDaysRect, "maxDays".Translate().CapitalizeFirst(), ref scheduledIncidentEntry.MaxDays, ref scheduledIncidentEntry.MaxDaysBuffer);
                if (scheduledIncidentEntry.MaxDays < scheduledIncidentEntry.MinDays)
                {
                    scheduledIncidentEntry.MaxDays = scheduledIncidentEntry.MinDays;
                    scheduledIncidentEntry.MaxDaysBuffer = scheduledIncidentEntry.MaxDays.ToString();
                }

                // repeat
                Rect repeatRect = new Rect(rowRect);
                repeatRect.width *= 0.2f;
                repeatRect.x += incidentButtonRect.width + minDaysRect.width + maxDaysRect.width;
                Widgets.CheckboxLabeled(repeatRect, "repeat".Translate().CapitalizeFirst(), ref scheduledIncidentEntry.Repeat);

                // Delete button
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    ScheduledIncidents.Remove(scheduledIncidentEntry);
                    specialConditionContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }

                // Summary, is necessary IMO because the UI gets a bit crowded and the scenario summary is not very helpful towards the user
                // (It doesn't say the interval or whether it repeats)
                
                if (scheduledIncidentEntry.Incident != null)
                {
                    Rect summaryRect = specialConditionContentListing.GetRect(28f);
                    if (scheduledIncidentEntry.Repeat)
                    {
                        Widgets.Label(summaryRect, "Playwright.Components.SpecialConditions.ScheduledIncidents.SummaryRepeat".Translate(scheduledIncidentEntry.IncidentLabel, scheduledIncidentEntry.MinDays, scheduledIncidentEntry.MaxDays));
                    }
                    else
                    {
                        Widgets.Label(summaryRect, "Playwright.Components.SpecialConditions.ScheduledIncidents.Summary".Translate(scheduledIncidentEntry.IncidentLabel, scheduledIncidentEntry.MinDays, scheduledIncidentEntry.MaxDays));
                    }
                }
                else
                {
                    // We reserve space for it even if nothing was selected yet
                    specialConditionContentListing.Gap(28f);
                }
            }

            specialConditionContentListing.ColumnWidth = oldWidth;
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (var scheduledIncidentEntry in ScheduledIncidents)
            {
                if (scheduledIncidentEntry.Incident == null)
                {
                    continue;
                }
                scenarioParts.Add(ScenPartUtils.MakeCreateIncidentPart(scheduledIncidentEntry.Incident, scheduledIncidentEntry.MinDays, scheduledIncidentEntry.MaxDays, scheduledIncidentEntry.Repeat));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref ScheduledIncidents, nameof(ScheduledIncidents), LookMode.Deep);
        }

        public class ScheduledIncidentEntry : IExposable
        {
            public IncidentDef Incident;
            public float MinDays = 5f;
            public string MinDaysBuffer = "5";
            public float MaxDays = 30;
            public string MaxDaysBuffer = "30";
            public bool Repeat = true;

            public string IncidentLabel => Incident != null ? Incident.LabelCap.ToString() : "Playwright.PleaseSelectFromFloatMenu".Translate().ToString();

            public void ExposeData()
            {
                Scribe_Defs.Look(ref Incident, nameof(Incident));
                Scribe_Values.Look(ref MinDays, nameof(MinDays), 5f);
                Scribe_Values.Look(ref MaxDays, nameof(MaxDays), 30f);
                Scribe_Values.Look(ref Repeat, nameof(Repeat), true);
                MinDaysBuffer = MinDays.ToString();
                MaxDaysBuffer = MaxDays.ToString();
            }
        }
    }
}
