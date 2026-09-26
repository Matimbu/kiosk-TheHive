$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$installation = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
if (-not $installation) { throw 'Visual Studio with MSBuild is required.' }
$msbuild = Join-Path $installation 'MSBuild/Current/Bin/MSBuild.exe'
$compiler = Join-Path $installation 'MSBuild/Current/Bin/Roslyn/csc.exe'
& $msbuild (Join-Path $repo 'kiosk.sln') /p:Configuration=Release /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
$output = Join-Path $repo 'kiosk/bin/Release'
& $compiler /nologo "/out:$output/SmokeTests.exe" "/r:$output/kiosk.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll (Join-Path $PSScriptRoot 'SmokeTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
& "$output/SmokeTests.exe" "$output/qa"
if ($LASTEXITCODE -ne 0) { throw 'Smoke tests failed.' }
