using System.Threading.Tasks;

namespace GitlabCmd.Console.Parsing
{
    public interface IVisitableOption
    {
        Task Accept(LaunchOptionsVisitor visitor);
    }
}