dotnet test test\GitlabCmd.Console.Test | Write-Host
if ($LastExitCode -ne 0)
{
	exit 1
}

dotnet test test\GitlabCmd.Gitlab.Test | Write-Host
if ($LastExitCode -ne 0)
{
	exit 1
}
