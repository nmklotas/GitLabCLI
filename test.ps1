dotnet test test\GitlabCmd.Console.Test
if ($LastExitCode -ne 0)
{
	exit 1
}

dotnet test test\GitlabCmd.Gitlab.Test
if ($LastExitCode -ne 0)
{
	exit 1
}
