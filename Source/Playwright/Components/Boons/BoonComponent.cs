using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public abstract class BoonComponent : PlaywrightComponent
    {
        /// <summary>
        /// If your component has settings, how high the settings rect is.
        /// Needs to be known in advance to reserve space for it in the UI.
        /// </summary>
        public virtual float SettingsHeight => 0f;

        public virtual void DoBoonContents(Listing_AutoFitVertical boonContentListing)
        {
            Text.Font = GameFont.Medium;
            boonContentListing.Label(NameTranslated);
            Text.Font = GameFont.Small;
            boonContentListing.Gap();
            boonContentListing.Label(DescriptionTranslated);
            DoSettingsContents(boonContentListing);
        }

        public virtual void DoSettingsContents(Listing_AutoFitVertical boonContentListing)
        {

        }

        public virtual void DoSettingsContents(Rect inRect)
        {

        }

        public static List<BoonComponent> GetAvailableBoons()
        {
            List<BoonComponent> boons = new List<BoonComponent>()
            {
                new BionicArmBoon()
            };

            BoonComponent shuttleBoon = new ShuttleBoon();
            if (shuttleBoon.IsAvailable)
            {
                boons.Add(shuttleBoon);
            }

            foreach (BoonComponent boon in ComponentRegistration.Boons)
            {
                if (boon.IsAvailable)
                {
                    boons.Add(boon);
                }
            }

            return boons;
        }
    }
}
