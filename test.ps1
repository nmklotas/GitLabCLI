nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.519 OpenCover
nuget install -Verbosity quiet -OutputDirectory packages -Version 2.4.5.0 ReportGenerator
nuget install -Verbosity quiet -OutputDirectory packages -Version 1.0.3 CodeCov

$coverageFolder = "$PSScriptRoot\coverage"
Remove-Item $coverageFolder -force -recurse -ErrorAction SilentlyContinue | Out-Null
New-Item $coverageFolder -type directory | Out-Null

$openCover="$PSScriptRoot\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe"
$reportGenerator="$PSScriptRoot\packages\ReportGenerator.2.4.5.0\tools\ReportGenerator.exe"
$codeCov = "$PSScriptRoot\packages\CodeCov.1.0.3\tools\codecov.exe"

Write-Host "Calculating coverage with OpenCover."
& $openCover `
  -target:"c:\Program Files\dotnet\dotnet.exe" `
  -targetargs:"test test\GitlabCmd.Gitlab.Test\GitLabCLI.GitLab.Test.csproj" `
  -mergeoutput `
  -hideskipped:File `
  -output:coverage/coverage.xml `
  -oldStyle `
  -filter:"+[GitLabCLI*]* -[GitLabCLI.*Test*]* -[GitLabCLI.Utilities]*" `
  -searchdirs:$test/bin/Debug/netcoreapp2.0 `
  -returntargetcode `
  -register:user | Write-Host
  
if ($LastExitCode -ne 0)
{
	exit 1
}

& $openCover `
  -target:"c:\Program Files\dotnet\dotnet.exe" `
  -targetargs:"test test\GitlabCmd.Console.Test\GitLabCLI.Console.Test.csproj" `
  -mergeoutput `
  -hideskipped:File `
  -output:coverage/coverage.xml `
  -oldStyle `
  -filter:"+[GitLabCLI*]* -[GitLabCLI.*Test*]* -[GitLabCLI.Utilities]*" `
  -searchdirs:$test/bin/Debug/netcoreapp2.0 `
  -returntargetcode `
  -register:user | Write-Host

if ($LastExitCode -ne 0)
{
	exit 1
}

Write-Host "Generating HTML report"
& $reportGenerator `
  -reports:coverage/coverage.xml `
  -targetdir:coverage `
  -verbosity:Error | Write-Host
  
Write-Host "Uploading coverage file"
& $codecov -f "coverage/coverage.xml" -t 6372499d-9a71-4043-be5e-73a76e0b7b29
