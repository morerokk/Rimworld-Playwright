using Rokk.Playwright.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.WinConditions
{
    /// <summary>
    /// Something the player can do to see the credits screen and be rewarded with a big fat "You won!" screen.
    /// This can be a goal like making the ship and getting off the planet.
    /// This could also be a much simpler or more abstract goal, like getting X colonists.
    /// </summary>
    public abstract class WinConditionComponent : PlaywrightComponent
    {
        public virtual float ContentHeight => 100f;
        /// <summary>
        /// If your component has settings, how high the settings rect is.
        /// Needs to be known in advance to reserve space for it in the UI.
        /// </summary>
        public virtual float SettingsHeight => 0f;

        public virtual void DoWinConditionContents(Rect inRect)
        {
            Listing_Standard winConditionContentListing = new Listing_Standard();
            winConditionContentListing.Begin(inRect);
            winConditionContentListing.Label(NameTranslated);
            winConditionContentListing.Gap();
            winConditionContentListing.Label(DescriptionTranslated);
            if (SettingsHeight > 0)
            {
                Rect winConditionSettingsRect = winConditionContentListing.GetRect(SettingsHeight);
                DoSettingsContents(winConditionSettingsRect);
            }
            winConditionContentListing.End();
        }

        public virtual void DoSettingsContents(Rect inRect)
        {

        }

        public static List<WinConditionComponent> GetAvailableWinConditions()
        {
            var list = new List<WinConditionComponent>();
            list.Add(new ShipWinCondition());

            var royalAscent = new RoyalAscentWinCondition();
            if (royalAscent.IsAvailable)
            {
                list.Add(royalAscent);
            }

            var archonexus = new ArchonexusWinCondition();
            if (archonexus.IsAvailable)
            {
                list.Add(archonexus);
            }

            foreach (WinConditionComponent winCondition in ComponentRegistration.WinConditions)
            {
                if (winCondition.IsAvailable)
                {
                    list.Add(winCondition);
                }
            }

            return list;
        }
    }
}
