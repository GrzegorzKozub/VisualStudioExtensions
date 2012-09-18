using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrzegorzKozub.VisualStudioExtensions.TotalCommanderLauncher
{
    [Guid(Guids.Package)]
    [InstalledProductRegistration("#1", "#2", Guids.Package, IconResourceID = 3)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Total Commander Launcher", "General", 0, 0, false)]
    public sealed class Package : Microsoft.VisualStudio.Shell.Package
    {
        private IMenuCommandService _menuCommandService;
        private IVsUIShell _uiShell;

        #region Package Members

        protected override void Initialize()
        {
            base.Initialize();

            _menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            _uiShell = GetService(typeof(SVsUIShell)) as IVsUIShell;

            AddMenuComands();
        }

        #endregion

        private void AddMenuComands()
        {
            var totalCommanderCommandId = new CommandID(Guids.MenuGroup, (int)CommandIds.TotalCommander);
            var totalCommanderMenuCommand = new MenuCommand(HandleTotalCommanderMenuCommand, totalCommanderCommandId);

            _menuCommandService.AddCommand(totalCommanderMenuCommand);
        }

        private void HandleTotalCommanderMenuCommand(object sender, EventArgs eventArgs)
        {
            var dte = GetGlobalService(typeof(DTE)) as DTE;
            var options = GetDialogPage(typeof(Options)) as Options;

            var validationErrors = options.GetValidationErrors();

            if (validationErrors != null)
            {
                DisplayErrorAndSuggestOptions(validationErrors);
                return;
            }

            var launcher = new Launcher(dte, options);

            try
            {
                launcher.Launch();
            }
            catch (Exception e)
            {
                DisplayErrorAndSuggestOptions(e.Message);
            }
        }

        private void DisplayErrorAndSuggestOptions(string errorMessage)
        {
            var comp = Guid.Empty;
            int result;

            _uiShell.ShowMessageBox(
                0,
                ref comp,
                errorMessage,
                "Do you want to visit the Options page for Total Commander Launcher now?",
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_CRITICAL,
                0,
                out result);

            if (result == 6)
            {
                var optionsCommandId = new CommandID(VSConstants.GUID_VSStandardCommandSet97, VSConstants.cmdidToolsOptions);
                ((MenuCommandService)_menuCommandService).GlobalInvoke(optionsCommandId, Guids.Options);
            }
        }
    }
}
