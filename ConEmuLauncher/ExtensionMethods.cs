using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GrzegorzKozub.VisualStudioExtensions.ConEmuLauncher
{
    internal static class ExtensionMethods
    {
        internal static bool ContainsParameter(this string target, string parameter)
        {
            return !string.IsNullOrEmpty(target) && Regex.IsMatch(target, string.Format(@"(^|\s){0}($|\s)", parameter.Replace("/", @"\/")), RegexOptions.IgnoreCase);
        }
    }
}
