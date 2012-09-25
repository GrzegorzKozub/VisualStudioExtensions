using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrzegorzKozub.VisualStudioExtensions.TotalCommanderLauncher
{
    public enum PanelLocation
    {
        [Description("Solution Explorer Path")]
        SolutionExplorerPath,

        [Description("Specific Path (Set Below)")]
        SpecificPath,

        [Description("Unchanged")]
        Unchanged
    }
}
