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

        [Category("Tabs")]
        [DisplayName("Tab 1 Name")]
        [Description("Case sensitive tab name configured in Console settings to open when launching Console. This is passed to the -t command line option.")]
        public string TabName1 { get; set; }

        [Category("Tabs")]
        [DisplayName("Tab 2 Name")]
        [Description("Case sensitive tab name configured in Console settings to open when launching Console. This is passed to the -t command line option.")]
        public string TabName2 { get; set; }

        [Category("Tabs")]
        [DisplayName("Tab 3 Name")]
        [Description("Case sensitive tab name configured in Console settings to open when launching Console. This is passed to the -t command line option.")]
        public string TabName3 { get; set; }

        [Category("Tabs")]
        [DisplayName("Tab 4 Name")]
        [Description("Case sensitive tab name configured in Console settings to open when launching Console. This is passed to the -t command line option.")]
        public string TabName4 { get; set; }

        [Category("Tabs")]
        [DisplayName("Tab 5 Name")]
        [Description("Case sensitive tab name configured in Console settings to open when launching Console. This is passed to the -t command line option.")]
        public string TabName5 { get; set; }

        public string GetValidationErrors()
        {
            if (string.IsNullOrEmpty(Path))
                return "Console Path was not set.";

            if (!FileExists(Path))
                return "Console Path points to a non-existent file.";

            if (!string.IsNullOrEmpty(DefaultWorkingDirectory) && !PathExists(DefaultWorkingDirectory))
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

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            var validationErrors = GetValidationErrors();

            if (validationErrors != null)
            {
                MessageBox.Show(validationErrors, "Console Launcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
