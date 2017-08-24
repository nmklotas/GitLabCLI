$ErrorActionPreference = "Stop"

function Build()
{
    & dotnet restore | Write-Host
    if($LASTEXITCODE -ne 0)
    {
        exit 1
    }

    & dotnet build  | Write-Host
    if($LASTEXITCODE -ne 0)
    {
        exit 2
    }

    & dotnet publish -f netcoreapp2.0 -c RELEASE -r win-x86 | Write-Host
    & dotnet publish -f netcoreapp2.0 -c RELEASE -r linux-x64 | Write-Host
    & dotnet publish -f netcoreapp2.0 -c RELEASE -r osx-x64 | Write-Host

    if($LASTEXITCODE -ne 0)
    {
        exit 3
    }
}

Build