using System.Diagnostics;
using GitLabCLI.Core;

namespace GitLabCLI.Console
{
    public sealed class DefaultBrowser : IBrowser
    {
        public void Open(string url) => Process.Start(url);
    }
}
