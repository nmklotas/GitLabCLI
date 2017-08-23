namespace GitlabCmd.Console
{
    public static class ExitCode
    {
        public static int Success { get; } = 0;

        public static int InvalidArguments { get; } = 1;

        public static int InvalidConfiguration { get; } = 2;

        public static int UnexpectedFailure { get; } = 3;
    }
}
