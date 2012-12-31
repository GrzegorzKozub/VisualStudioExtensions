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

namespace GrzegorzKozub.VisualStudioExtensions.ConEmuLauncher
{
    [Guid(Guids.Options)]
    public class Options : DialogPage
    {
        public Options()
        {
            Path = "conemu64.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
        }

        [Category("Application")]
        [DisplayName("ConEmu Path")]
        [Description("The full path to the ConEmu executable file.")]
        public string Path { get; set; }

        [Category("Directories")]
        [DisplayName("Default Working Directory")]
        [Description("ConEmu will open in this folder in case it cannot guess the working directory from the current Solution Explorer selection. This is passed to the /dir command line option.")]
        public string DefaultWorkingDirectory { get; set; }

        [Category("Settings")]
        [DisplayName("Command Line Options")]
        [Description("Parameters other than /dir to pass to ConEmu when launching it. Switches provided here take priority over the ones from other options.")]
        public string CommandLineOptions { get; set; }

        [Category("Settings")]
        [DisplayName("Reuse Existing Instance")]
        [Description("Activate an already running ConEmu instance passing it a task to execute. The same as the /single command line option.")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ReuseExistingInstance { get; set; }

        [Category("Settings")]
        [DisplayName("Task To Execute")]
        [Description("The task name configured in ConEmu settings to execute when launching ConEmu. This is passed to the /cmd command line option.")]
        public string TaskToExecute { get; set; }

        public string GetValidationErrors()
        {
            if (string.IsNullOrEmpty(Path))
                return "ConEmu Path was not set.";

            if (!FileExists(Path))
                return "ConEmu Path points to a non-existent file.";

            if (!string.IsNullOrEmpty(DefaultWorkingDirectory) && !PathExists(DefaultWorkingDirectory))
                return "Default Working Directory points to a non-existent path.";

            if (CommandLineOptions.ContainsParameter("/dir"))
                return "Command Line Options cannot contain the /dir parameter.";

            return null;
        }

        #region DialogPage Members

        public override void ResetSettings()
        {
            base.ResetSettings();

            Path = "conemu64.exe";
            DefaultWorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
            CommandLineOptions = null;
            ReuseExistingInstance = false;
            TaskToExecute = null;
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
