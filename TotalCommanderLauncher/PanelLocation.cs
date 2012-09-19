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
        [Description("Solution Explorer Directory")]
        SolutionExplorerDirectory,

        [Description("Specific Directory Set Below")]
        SpecificDirectory,

        [Description("Unchanged")]
        Unchanged
    }
}
