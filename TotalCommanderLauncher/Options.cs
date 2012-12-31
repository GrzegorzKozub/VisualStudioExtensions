using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace GrzegorzKozub.VisualStudioExtensions.TotalCommanderLauncher
{
    [Guid(Guids.Options)]
    public class Options : DialogPage
    {
        public Options()
        {
            Path = "totalcmd64.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
        }

        [Category("Application")]
        [DisplayName("Total Commander Path")]
        [Description("The full path to the Total Commander executable file.")]
        public string Path { get; set; }

        [Category("Directories")]
        [DisplayName("Default Working Directory")]
        [Description("Total Commander will open this folder in case it cannot guess the working directory from the current Solution Explorer selection. This is passed to the -l and/or -r command line option.")]
        public string DefaultWorkingDirectory { get; set; }

        [Category("Left Panel")]
        [DisplayName("Left Panel Location")]
        [Description("The location to open in the left Total Commander panel.")]
        [TypeConverter(typeof(PanelLocationConverter))]
        public PanelLocation LeftPanelLocation { get; set; }

        [Category("Left Panel")]
        [DisplayName("Left Panel Specific Path")]
        [Description("The folder or file to open in the left Total Commander panel if Left Panel Location is set to Specific Path. This is passed to the -l command line option.")]
        public string LeftPanelSpecificPath { get; set; }

        [Category("Right Panel")]
        [DisplayName("Right Panel Location")]
        [Description("The location to open in the right Total Commander panel.")]
        [TypeConverter(typeof(PanelLocationConverter))]
        public PanelLocation RightPanelLocation { get; set; }

        [Category("Right Panel")]
        [DisplayName("Right Panel Specific Path")]
        [Description("The folder or file to open in the right Total Commander panel if Right Panel Location is set to Specific Path. This is passed to the -r command line option.")]
        public string RightPanelSpecificPath { get; set; }

        [Category("Settings")]
        [DisplayName("Active Panel")]
        [Description("Total Commander panel to activate. The same as the -p command line option.")]
        [TypeConverter(typeof(ActivePanelConverter))]
        public ActivePanel ActivePanel { get; set; }

        [Category("Settings")]
        [DisplayName("Create New Tabs")]
        [Description("Create new Total Commander tabs for the directories to open. The same as the -t command line option.")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool CreateNewTabs { get; set; }

        [Category("Settings")]
        [DisplayName("Reuse Existing Instance")]
        [Description("Activate an already running Total Commander instance passing it the directories to open. The same as the -o command line option.")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ReuseExistingInstance { get; set; }

        public string GetValidationErrors()
        {
            if (string.IsNullOrEmpty(Path))
                return "Total Commander Path was not set.";

            if (!FileExists(Path))
                return "Total Commander Path points to a non-existent file.";

            if (!string.IsNullOrEmpty(DefaultWorkingDirectory) && !PathExists((DefaultWorkingDirectory)))
                return "Default Working Directory points to a non-existent path.";

            if (LeftPanelLocation == PanelLocation.SpecificPath && string.IsNullOrEmpty(LeftPanelSpecificPath))
                return "Left Panel Specific Path must be set when Left Panel Location is set to Specific Path.";

            if (!string.IsNullOrEmpty(LeftPanelSpecificPath) && !PathExists(LeftPanelSpecificPath))
                return "Left Panel Specific Path points to a non-existent path.";

            if (RightPanelLocation == PanelLocation.SpecificPath && string.IsNullOrEmpty(RightPanelSpecificPath))
                return "Right Panel Specific Path must be set when Right Panel Location is set to Specific Path.";

            if (!string.IsNullOrEmpty(RightPanelSpecificPath) && !PathExists(RightPanelSpecificPath))
                return "Right Panel Specific Path points to a non-existent path.";

            return null;
        }

        #region DialogPage Members

        public override void ResetSettings()
        {
            base.ResetSettings();

            Path = "totalcmd64.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
            LeftPanelLocation = PanelLocation.SolutionExplorerPath;
            LeftPanelSpecificPath = null;
            RightPanelLocation = PanelLocation.SolutionExplorerPath;
            RightPanelSpecificPath = null;
            ActivePanel = ActivePanel.Left;
            ReuseExistingInstance = false;
            CreateNewTabs = false;
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            var validationErrors = GetValidationErrors();

            if (validationErrors != null)
            {
                MessageBox.Show(validationErrors, "ConEmu Launcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.ApplyBehavior = ApplyKind.CancelNoNavigate;
            }
            else
                base.OnApply(e);
        }

        #endregion

        private static bool FileExists(string filePath)
        {
            if (System.IO.Path.IsPathRooted(filePath))
                return File.Exists(filePath);

            foreach (var path in Environment.GetEnvironmentVariable("PATH").Split(';'))
            {
                if (File.Exists(System.IO.Path.Combine(path, filePath)))
                    return true;
            }

            return false;
        }

        private static bool PathExists(string path)
        {
            var pathExpanded = Environment.ExpandEnvironmentVariables(path);
            return File.Exists(pathExpanded) || Directory.Exists(pathExpanded);
        }
    }
}
