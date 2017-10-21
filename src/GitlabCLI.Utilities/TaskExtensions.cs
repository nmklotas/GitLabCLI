using System.Threading.Tasks;

namespace GitLabCLI.Utilities
{
    public static class TaskExtensions
    {
        public static Task<Task> Wrap(this Task task) => Task.FromResult(task);
    }
}
