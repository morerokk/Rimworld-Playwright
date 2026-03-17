using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rokk.Playwright.Components.WinConditions
{
    /// <summary>
    /// Something the player can do to see the credits screen and be rewarded with a big fat "You won!" screen.
    /// This can be a goal like making the ship and getting off the planet.
    /// This could also be a much simpler or more abstract goal, like getting X colonists.
    /// </summary>
    public abstract class WinConditionComponent : PlaywrightComponent
    {
        public virtual void DoSettingsContents(Rect inRect)
        {

        }
    }
}
