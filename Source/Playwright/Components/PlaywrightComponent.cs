using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components
{
    /// <summary>
    /// The base class for any Playwright component.
    /// When extending this, ensure the constructor is safe, as a new instance must be created for <see cref="IsAvailable"/> to be callable.
    /// </summary>
    public abstract class PlaywrightComponent : IExposable
    {
        /// <summary>
        /// The ID for this component. Must be unique and should be prefixed with the type of component it is.
        /// Example: "Origins.CoolDude"
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Whether this component is currently available.
        /// Can be available/unavailable for any reason, such as depending on whether a mod or DLC is loaded.
        /// If false, this component will not be shown in the Playwright designer, and can't be loaded from a saved Playwright.
        /// </summary>
        public virtual bool IsAvailable => true;

        public virtual string Name
        {
            get
            {
                return ("Playwright.Components." + this.Id);
            }
        }
        public virtual string Description
        {
            get
            {
                return ("Playwright.Components." + this.Id + ".Description");
            }
        }
        public virtual string NameTranslated
        {
            get
            {
                return ("Playwright.Components." + this.Id).Translate();
            }
        }
        public virtual string DescriptionTranslated
        {
            get
            {
                return ("Playwright.Components." + this.Id + ".Description").Translate();
            }
        }

        /// <summary>
        /// Mutate the scenario and its parts based on what this component does (such as adding a ScenPart for forced goodwill or removing a pre-existing part)
        /// </summary>
        public abstract void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts);

        /// <summary>
        /// Render the UI for the settings in your component.
        /// If your component doesn't have any extra settings, you don't have to override this.
        /// </summary>
        /// <param name="inRect">The <see cref="Rect"/> that your settings will be rendered inside of.</param>
        public virtual void DoSettingsContents(Rect inRect)
        {

        }

        /// <summary>
        /// <see cref="IExposable"/>, needed to serialize the Playwright structure with Scribe.
        /// If your component doesn't have any extra settings, you don't have to override this.
        /// </summary>
        public virtual void ExposeData()
        {

        }
    }
}
