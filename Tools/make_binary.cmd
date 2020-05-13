@echo off
REM SurfaceLights *requires* .NET compiler version 6.0 or higher and Framework version not less than 4.5.
"C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe" ..\Source\SurfaceLights.csproj /t:Clean,Build /p:Configuration=Release
