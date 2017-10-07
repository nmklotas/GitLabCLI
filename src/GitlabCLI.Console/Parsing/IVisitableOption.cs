using System.Threading.Tasks;

namespace GitLabCLI.Console.Parsing
{
    public interface IVisitableOption
    {
        Task Accept(LaunchOptionsVisitor visitor);
    }
}