using System.Diagnostics;
using GitLabCLI.Core;

namespace GitLabCLI.Console
{
    public sealed class BrowserProcessStarter : IBrowser
    {
        public void Open(string url)
        {
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }
    }
}
