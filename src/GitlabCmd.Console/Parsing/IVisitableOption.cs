using System.Threading.Tasks;

namespace GitLabCmd.Console.Parsing
{
    public interface IVisitableOption
    {
        Task Accept(LaunchOptionsVisitor visitor);
    }
}