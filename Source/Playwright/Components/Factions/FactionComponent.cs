using RimWorld;
using Rokk.Playwright.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Components.Factions
{
    public abstract class FactionComponent : PlaywrightComponent
    {
        /// <summary>
        /// This faction's def. Can be null if this doesn't apply, but if your component is for a specific faction, this should be rare.
        /// </summary>
        public virtual FactionDef FactionDef => null;

        /// <summary>
        /// What dispositions this faction is allowed to have during selection.
        /// This should usually be "anything", but insectoids and mechanoids don't really work if they're not always the default of hostile.
        /// </summary>
        public virtual HashSet<FactionDisposition> AllowedDispositions => new HashSet<FactionDisposition>()
        {
            FactionDisposition.Default,
            FactionDisposition.Neutral,
            FactionDisposition.InitiallyHostile,
            FactionDisposition.AlwaysHostile,
            FactionDisposition.InitiallyAllied,
            FactionDisposition.AlwaysAllied,
        };

        /// <summary>
        /// How many of this faction can be added within a single group (Allies, Enemies, Neutral).
        /// This should usually be 1, but some factions are placeholders so they might be added multiple times.
        /// </summary>
        public virtual int MaxInGroup => 1;
        /// <summary>
        /// How many of this faction can be added in total, across all groups.
        /// This should usually be 1, but some factions are placeholders so they might be added multiple times.
        /// </summary>
        public virtual int MaxTotal => 1;

        /// <summary>
        /// What natural or forced goodwill the faction will have.
        /// </summary>
        public enum FactionDisposition
        {
            Default = 0,
            Neutral = 100,
            InitiallyHostile = 99,
            AlwaysHostile = 98,
            InitiallyAllied = 101,
            AlwaysAllied = 102
        }

        public bool IsRelationKindAllowed(FactionRelationKind relationKind)
        {
            if (relationKind == FactionRelationKind.Ally)
            {
                return AllowedDispositions.Contains(FactionDisposition.InitiallyAllied) || AllowedDispositions.Contains(FactionDisposition.AlwaysAllied);
            }
            else if (relationKind == FactionRelationKind.Hostile)
            {
                return AllowedDispositions.Contains(FactionDisposition.InitiallyHostile) || AllowedDispositions.Contains(FactionDisposition.AlwaysHostile);
            }
            else
            {
                return AllowedDispositions.Contains(FactionDisposition.Neutral);
            }
        }

        public static List<FactionComponent> GetAvailableFactions()
        {
            var list = new List<FactionComponent>();
            list.Add(new AllOtherFactions());
            list.Add(new SpecificFaction());
            list.Add(new InsectoidHiveFaction());
            list.Add(new MechanoidHiveFaction());

            foreach (var faction in ComponentRegistration.Factions)
            {
                if (faction.IsAvailable)
                {
                    list.Add(faction);
                }
            }

            return list;
        }
    }
}
