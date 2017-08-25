dotnet test test\src\GitlabCmd.Console.Test
if ($LastExitCode -ne 0)
{
	exit 1
}

dotnet test test\src\GitlabCmd.Gitlab.Test
if ($LastExitCode -ne 0)
{
	exit 1
}
