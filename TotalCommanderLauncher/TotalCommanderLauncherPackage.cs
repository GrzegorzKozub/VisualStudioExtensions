using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace GrzegorzKozub.VisualStudioExtensions.TotalCommanderLauncher
{
    [Guid(Guids.Package)]
    [InstalledProductRegistration("#1", "#2", Guids.Package, IconResourceID = 3)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Total Commander Launcher", "General", 0, 0, false)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class TotalCommanderLauncherPackage : Microsoft.VisualStudio.Shell.Package
    {
        private IMenuCommandService _menuCommandService;
        private IVsUIShell _uiShell;

        #region Package Members

        protected override void Initialize()
        {
            base.Initialize();

            _menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            _uiShell = GetService(typeof(SVsUIShell)) as IVsUIShell;

            AddMenuCommand(CommandIds.TotalCommander, HandleTotalCommanderMenuCommand, options => true);
        }

        #endregion

        private void AddMenuCommand(uint commandId, EventHandler invokeHandler, Func<Options, bool> visible)
        {
            var commandID = new CommandID(Guids.MenuGroup, (int)commandId);
            var menuCommand = new OleMenuCommand(invokeHandler, commandID);

            menuCommand.BeforeQueryStatus += (s, e) =>
            {
                menuCommand.Visible = visible(GetDialogPage(typeof(Options)) as Options);
            };

            _menuCommandService.AddCommand(menuCommand);
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
