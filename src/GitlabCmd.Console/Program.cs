using System.Threading.Tasks;
using GitlabCmd.Console.App;

namespace GitlabCmd.Console
{
    public sealed class Program
    {
        public static async Task<int> Main(string[] args) => 
            await Container.Build().Resolve<LaunchHandler>().Launch(args);
    }
}