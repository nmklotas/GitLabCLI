using System.Threading.Tasks;

namespace GitlabCmd.Console
{
    public sealed class Program
    {
        public static async Task<int> Main(string[] args) => 
            await Container.Build().Resolve<LaunchHandler>().Launch(args);
    }
}