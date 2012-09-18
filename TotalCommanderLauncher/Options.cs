using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        [Description("Total Commander will open this folder in case it cannot guess the working directory from the current Solution Explorer selection.")]
        public string DefaultWorkingDirectory { get; set; }

        [Category("Left Panel")]
        [DisplayName("Left Panel Location")]
        [Description("The location to open in the left Total Commander panel.")]
        public PanelLocation LeftPanelLocation { get; set; }

        [Category("Left Panel")]
        [DisplayName("Left Panel Specific Directory")]
        [Description("The folder to open in the left Total Commander panel if Left Panel Location is set to Specific Directory.")]
        public string LeftPanelSpecificDirectory { get; set; }

        [Category("Right Panel")]
        [DisplayName("Right Panel Location")]
        [Description("The location to open in the right Total Commander panel.")]
        public PanelLocation RightPanelLocation { get; set; }

        [Category("Right Panel")]
        [DisplayName("Right Panel Specific Directory")]
        [Description("The folder to open in the right Total Commander panel if Right Panel Location is set to Specific Directory.")]
        public string RightPanelSpecificDirectory { get; set; }

        [Category("Settings")]
        [DisplayName("Active Panel")]
        [Description("Total Commander panel to activate.")]
        public ActivePanel ActivePanel { get; set; }

        [Category("Settings")]
        [DisplayName("Create New Tabs")]
        [Description("Create new Total Commander tabs for the directories to open.")]
        public bool CreateNewTabs { get; set; }

        [Category("Settings")]
        [DisplayName("Reuse Existing Instance")]
        [Description("Activate an already running Total Commander instance passing it the directories to open.")]
        public bool ReuseExistingInstance { get; set; }

        public string GetValidationErrors()
        {
            if (string.IsNullOrEmpty(Path))
                return "Total Commander Path was not set.";

            if (!FileExists(Path))
                return "Total Commander Path points to a non-existent file.";

            if (!string.IsNullOrEmpty(DefaultWorkingDirectory) && !Directory.Exists(Environment.ExpandEnvironmentVariables(DefaultWorkingDirectory)))
                return "Default Working Directory points to a non-existent path.";

            if (LeftPanelLocation == PanelLocation.SpecificDirectory && string.IsNullOrEmpty(LeftPanelSpecificDirectory))
                return "Left Panel Specific Directory must be set when Left Panel Location is set to Specific Directory.";

            if (!string.IsNullOrEmpty(LeftPanelSpecificDirectory) && !Directory.Exists(Environment.ExpandEnvironmentVariables(LeftPanelSpecificDirectory)))
                return "Left Panel Specific Directory points to a non-existent path.";

            if (RightPanelLocation == PanelLocation.SpecificDirectory && string.IsNullOrEmpty(RightPanelSpecificDirectory))
                return "Right Panel Specific Directory must be set when Right Panel Location is set to Specific Directory.";

            if (!string.IsNullOrEmpty(RightPanelSpecificDirectory) && !Directory.Exists(Environment.ExpandEnvironmentVariables(RightPanelSpecificDirectory)))
                return "Right Panel Specific Directory points to a non-existent path.";

            return null;
        }

        #region DialogPage Members

        public override void ResetSettings()
        {
            base.ResetSettings();

            Path = "totalcmd64.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
            LeftPanelLocation = PanelLocation.SolutionExplorerDirectory;
            LeftPanelSpecificDirectory = null;
            RightPanelLocation = PanelLocation.SolutionExplorerDirectory;
            RightPanelSpecificDirectory = null;
            ActivePanel = ActivePanel.Left;
            ReuseExistingInstance = false;
            CreateNewTabs = false;
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
    }
}
