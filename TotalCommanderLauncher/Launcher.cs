using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrzegorzKozub.VisualStudioExtensions.TotalCommanderLauncher
{
    internal class Launcher
    {
        private DTE _dte;
        private Options _options;

        internal Launcher(DTE dte, Options options)
        {
            _dte = dte;
            _options = options;
        }

        internal void Launch()
        {
            var process = System.Diagnostics.Process.Start(new ProcessStartInfo()
            {
                FileName = GetFileName(),
                Arguments = GetArguments()
            });

            System.Threading.Thread.Sleep(250);

            if (process != null && !process.HasExited)
                NativeMethods.SetForegroundWindow(process.MainWindowHandle);
        }

        private string GetWorkingDirectory()
        {
            string path;
            var selectedItem = _dte.SelectedItems.Item(1);

            if (selectedItem.Project != null &&
                selectedItem.Project.Kind != "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}" && // Excludes Solution Folders.
                !string.IsNullOrEmpty(selectedItem.Project.FullName))
            {
                // Project.
                path = selectedItem.Project.FullName;
            }
            else if (selectedItem.ProjectItem != null &&
                (
                    Guid.Parse(selectedItem.ProjectItem.Kind) == VSConstants.GUID_ItemType_PhysicalFile ||
                    Guid.Parse(selectedItem.ProjectItem.Kind) == VSConstants.GUID_ItemType_PhysicalFolder
                ) &&
                selectedItem.ProjectItem.Properties != null &&
                selectedItem.ProjectItem.Properties.Item("FullPath") != null)
            {
                // Project Folder (also Project Properties) or Project Item.
                path = selectedItem.ProjectItem.Properties.Item("FullPath").Value.ToString();
            }
            else
            {
                // Solution, Solution Folder, Solution Folder contents or Project References.
                path = _dte.Solution.FullName;
            }

            if (string.IsNullOrEmpty(path))
            {
                var defaultWorkingDirectory = string.IsNullOrEmpty(_options.DefaultWorkingDirectory) ? "%HOMEDRIVE%%HOMEPATH%" : _options.DefaultWorkingDirectory;
                return Environment.ExpandEnvironmentVariables(defaultWorkingDirectory);
            }
            else
                return Path.GetDirectoryName(path);
        }

        private string GetFileName()
        {
            return _options.Path;
        }

        private string GetArguments()
        {
            var arguments = new StringBuilder();

            var workingDirectory = GetWorkingDirectory();

            if (_options.LeftPanelLocation == PanelLocation.SolutionExplorerDirectory)
                arguments.AppendFormat(" -l=\"{0}\"", workingDirectory);
            else if (_options.LeftPanelLocation == PanelLocation.SpecificDirectory)
                arguments.AppendFormat(" -l=\"{0}\"", _options.LeftPanelSpecificDirectory);

            if (_options.RightPanelLocation == PanelLocation.SolutionExplorerDirectory)
                arguments.AppendFormat(" -r=\"{0}\"", workingDirectory);
            else if (_options.RightPanelLocation == PanelLocation.SpecificDirectory)
                arguments.AppendFormat(" -r=\"{0}\"", _options.RightPanelSpecificDirectory);

            arguments.AppendFormat(" -p={0}", _options.ActivePanel == ActivePanel.Left ? "l" : "r");
            
            if (_options.CreateNewTabs)
                arguments.Append(" /t");

            if (_options.ReuseExistingInstance)
                arguments.Append(" /o");

            return arguments.ToString();
        }
    }
}
