using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Vehicles;
using Verse;

namespace Rokk.Playwright.Compat.VehicleFramework.Components.Boons
{
    public class VehiclesBoon : BoonComponent
    {
        public override string Id => "Boons.Compat_VehicleFramework_Vehicles";

        public List<VehicleDef> Vehicles = new List<VehicleDef>();

        private IEnumerable<VehicleDef> GetAllowedVehicles()
        {
            return DefDatabase<VehicleDef>.AllDefs;
        }

        private string GetVehicleLabel(VehicleDef vehicleDef)
        {
            if (vehicleDef == null)
            {
                return "VF_RandomVehicle".Translate();
            }
            return vehicleDef.LabelCap;
        }

        protected override void DoSettingsContents(Listing_AutoFitVertical boonContentListing, OriginComponent origin)
        {
            if (boonContentListing.ButtonText("Playwright.Components.Boons.Compat_VehicleFramework_Vehicles.Add".Translate()))
            {
                var options = new List<FloatMenuOption>()
                {
                    new FloatMenuOption("VF_RandomVehicle".Translate(), () =>
                    {
                        Vehicles.Add(null);
                        boonContentListing.InvalidateGroup();
                    })
                };

                foreach (VehicleDef vehicleDef in GetAllowedVehicles())
                {
                    options.Add(new FloatMenuOption(vehicleDef.LabelCap, () =>
                    {
                        Vehicles.Add(vehicleDef);
                        boonContentListing.InvalidateGroup();
                    }));
                }
                PlaywrightUtils.OpenFloatMenu(options);
                SoundUtils.PlayClick();
            }

            DoVehiclesContents(boonContentListing);
        }

        private void DoVehiclesContents(Listing_AutoFitVertical boonContentListing)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            foreach (VehicleDef vehicleDef in Vehicles.ToList())
            {
                Rect rowRect = boonContentListing.GetRect(25f);
                Rect vehicleLabelRect = new Rect(rowRect);
                vehicleLabelRect.width *= 0.9f;
                Widgets.Label(vehicleLabelRect, GetVehicleLabel(vehicleDef));

                Rect deleteButtonRect = new Rect(rowRect);
                deleteButtonRect.width *= 0.1f;
                deleteButtonRect.x += vehicleLabelRect.width;
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    Vehicles.Remove(vehicleDef);
                    boonContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            FieldInfo vehicleDefInfo = AccessTools.Field(typeof(ScenPart_StartingVehicle), "vehicleDef");
            foreach (VehicleDef vehicleDef in Vehicles)
            {
                ScenPart_StartingVehicle part = (ScenPart_StartingVehicle)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.StartingVehicle);
                vehicleDefInfo.SetValue(part, vehicleDef);
                scenarioParts.Add(part);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref Vehicles, nameof(Vehicles), LookMode.Def);
        }
    }
}
