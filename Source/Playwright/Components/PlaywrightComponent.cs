using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components
{
    /// <summary>
    /// The base class for any Playwright component.
    /// When extending this, ensure the constructor is safe, as a new instance must be created for <see cref="IsAvailable"/> to be callable.
    /// 
    /// Some components may be constructed on game startup, when <see cref="DefOf"/> are not yet loaded.
    /// If you need a default def value for a setting, consider using null as sentinel value and set the intended default in the settings contents method.
    /// </summary>
    public abstract class PlaywrightComponent : IExposable
    {
        /// <summary>
        /// The ID for this component. Must be unique and should be prefixed with the type of component it is.
        /// Example: "Origins.CoolDude"
        /// If you are making an addon or a compatibility mod, try to use a unique prefix, as ID's must be unique.
        /// Example: "Origins.Compat_foobarmod_CoolDude"
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Whether this component is currently available.
        /// Can be available/unavailable for any reason, such as depending on whether an expansion or another mod is loaded.
        /// If false, this component will not be shown in the Playwright designer, and can't be loaded from a saved Playwright.
        /// </summary>
        public virtual bool IsAvailable => true;

        public virtual string Name => "Playwright.Components." + this.Id;
        public virtual string Description => "Playwright.Components." + this.Id + ".Description";
        public virtual string NameTranslated => Name.Translate();
        public virtual string DescriptionTranslated => Description.Translate();
        public virtual string DescriptionShort => "Playwright.Components." + this.Id + ".DescriptionShort";
        /// <summary>
        /// Short description that is generally used for tooltips.
        /// </summary>
        public virtual string DescriptionShortTranslated => DescriptionShort.Translate();

        /// <summary>
        /// Mutates the scenario and its parts based on what this component does (such as adding a ScenPart for forced goodwill or removing a pre-existing part).
        /// Called when a Playwright is compiled into a scenario.
        /// </summary>
        public virtual void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {

        }

        /// <summary>
        /// <see cref="IExposable"/>, needed to serialize the Playwright structure with Scribe.
        /// If your component doesn't have any extra settings, you don't have to override this.
        /// If your component has settings, ensure you call <see cref="Scribe_Values"/> etc. on them.
        /// </summary>
        public virtual void ExposeData()
        {

        }
    }
}
