using System.Threading.Tasks;
using GitlabCmd.Console.App;

namespace GitlabCmd.Console.Cmd
{
    public interface IVisitableOption
    {
        Task Accept(LaunchOptionsVisitor visitor);
    }
}