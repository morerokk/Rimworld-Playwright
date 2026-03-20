using RimWorld;
using Rokk.Playwright.Addons;
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
        public virtual float ContentHeight => 100f;
        /// <summary>
        /// If your component has settings, how high the settings rect is.
        /// Needs to be known in advance to reserve space for it in the UI.
        /// </summary>
        public virtual float SettingsHeight => 0f;

        public virtual void DoBoonContents(Rect inRect)
        {
            Listing_Standard boonContentListing = new Listing_Standard();
            boonContentListing.Begin(inRect);
            boonContentListing.Label(NameTranslated);
            boonContentListing.Gap();
            boonContentListing.Label(DescriptionTranslated);
            if (SettingsHeight > 0)
            {
                Rect boonSettingsRect = boonContentListing.GetRect(SettingsHeight);
                DoSettingsContents(boonSettingsRect);
            }
            boonContentListing.End();
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
