using RimWorld;
using Rokk.Playwright.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public abstract class OriginComponent : PlaywrightComponent
    {
        /// <summary>
        /// How many colonists you can select at the start screen to take with you
        /// </summary>
        public virtual int StartingColonistsSelectable => 3;
        /// <summary>
        /// How many colonist choices are available to you total
        /// </summary>
        public virtual int StartingColonistsTotal => 8;
        /// <summary>
        /// How your colonists arrive on the planet
        /// </summary>
        public virtual PlayerPawnsArriveMethod ArrivalMethod => PlayerPawnsArriveMethod.DropPods;

        public virtual string Summary
        {
            get
            {
                return "Playwright.Components." + this.Id + ".Summary";
            }
        }
        public virtual string SummaryTranslated
        {
            get
            {
                return ("Playwright.Components." + this.Id + ".Summary").Translate();
            }
        }

        public static List<OriginComponent> GetAvailableOrigins()
        { 
            var origins = new List<OriginComponent>();
            origins.Add(new CrashlandedOrigin());
            origins.Add(new TribalOrigin());
            origins.Add(new RichExplorerOrigin());

            var empire = new EmpireOrigin();
            if (empire.IsAvailable)
            {
                origins.Add(empire);
            }

            foreach (OriginComponent origin in ComponentRegistration.Origins)
            {
                if (origin.IsAvailable)
                {
                    origins.Add(origin);
                }
            }

            return origins;
        }
    }
}
