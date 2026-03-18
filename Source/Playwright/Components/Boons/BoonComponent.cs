using RimWorld;
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

            return boons;
        }
    }
}
