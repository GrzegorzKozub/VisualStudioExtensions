using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;

namespace GrzegorzKozub.VisualStudioExtensions.ConEmuLauncher
{
    [Guid(Guids.Package)]
    [InstalledProductRegistration("#1", "#2", "1.1.1.0", IconResourceID = 3)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "ConEmu Launcher", "General", 0, 0, false)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class ConEmuLauncherPackage : Package
    {
        private IMenuCommandService _menuCommandService;
        private IVsUIShell _uiShell;

        #region Package Members

        protected override void Initialize()
        {
            base.Initialize();

            _menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            _uiShell = GetService(typeof(SVsUIShell)) as IVsUIShell;

            AddMenuCommand(CommandIds.ConEmu, HandleConEmuMenuCommand, options => true);
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

        private void HandleConEmuMenuCommand(object sender, EventArgs ea)
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
                "Do you want to visit the Options page for ConEmu Launcher now?",
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
