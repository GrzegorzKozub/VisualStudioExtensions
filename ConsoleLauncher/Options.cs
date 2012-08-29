using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrzegorzKozub.VisualStudioExtensions.ConsoleLauncher
{
    [Guid(Guids.Options)]
    public class Options : DialogPage
    {
        public Options()
        {
            Path = "console.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
        }

        [Category("Application")]
        [DisplayName("Console Path")]
        [Description("The full path to the Console executable file.")]
        public string Path { get; set; }

        [Category("Directories")]
        [DisplayName("Default Working Directory")]
        [Description("Console will open in this folder in case it cannot guess the working directory from the current Solution Explorer selection.")]
        public string DefaultWorkingDirectory { get; set; }

        [Category("Settings")]
        [DisplayName("Tab 1 Name")]
        [Description("The tab name configured in Console settings to open when launching Console. Case sensitive.")]
        public string TabName1 { get; set; }

        [Category("Settings")]
        [DisplayName("Tab 2 Name")]
        [Description("The tab name configured in Console settings to open when launching Console. Case sensitive.")]
        public string TabName2 { get; set; }

        [Category("Settings")]
        [DisplayName("Tab 3 Name")]
        [Description("The tab name configured in Console settings to open when launching Console. Case sensitive.")]
        public string TabName3 { get; set; }

        [Category("Settings")]
        [DisplayName("Tab 4 Name")]
        [Description("The tab name configured in Console settings to open when launching Console. Case sensitive.")]
        public string TabName4 { get; set; }

        [Category("Settings")]
        [DisplayName("Tab 5 Name")]
        [Description("The tab name configured in Console settings to open when launching Console. Case sensitive.")]
        public string TabName5 { get; set; }

        public string GetValidationErrors()
        {
            if (string.IsNullOrEmpty(Path))
                return "Console Path was not set.";

            if (System.IO.Path.IsPathRooted(Path) && !File.Exists(Path))
                return "Console Path points to a non-existent file.";

            if (!string.IsNullOrEmpty(DefaultWorkingDirectory) && !Directory.Exists(Environment.ExpandEnvironmentVariables(DefaultWorkingDirectory)))
                return "Default Working Directory points to a non-existent path.";

            return null;
        }

        #region DialogPage Members

        public override void ResetSettings()
        {
            base.ResetSettings();

            Path = "console.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
            TabName1 = null;
            TabName2 = null;
            TabName3 = null;
            TabName4 = null;
            TabName5 = null;
        }

        #endregion
    }
}
