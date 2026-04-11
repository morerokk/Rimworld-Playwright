using Rokk.Playwright.Addons;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public virtual void DoWinConditionContents(Listing_AutoFitVertical winConditionContentListing)
        {
            Text.Font = GameFont.Medium;
            winConditionContentListing.Label(NameTranslated);
            Text.Font = GameFont.Small;
            winConditionContentListing.Gap();
            winConditionContentListing.Label(DescriptionTranslated);
            DoSettingsContents(winConditionContentListing);
        }

        public virtual void DoSettingsContents(Listing_AutoFitVertical winConditionContentListing)
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

            list.Add(new ConquestWinCondition());
            list.Add(new ColonyWinCondition());

            var royalCouncil = new RoyalCouncilWinCondition();
            if (royalCouncil.IsAvailable)
            {
                list.Add(royalCouncil);
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
