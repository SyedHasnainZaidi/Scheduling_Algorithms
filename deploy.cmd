@echo off
setlocal

if "%DEPLOYMENT_SOURCE%" == "" set DEPLOYMENT_SOURCE=%~dp0
if "%DEPLOYMENT_TARGET%" == "" set DEPLOYMENT_TARGET=%~dp0\\artifacts\\wwwroot

echo [Azure Deploy] Publishing SimulatorProject.csproj...
dotnet publish "%DEPLOYMENT_SOURCE%\\SimulatorProject.csproj" -c Release -o "%DEPLOYMENT_TARGET%" --nologo
if %ERRORLEVEL% NEQ 0 goto error

echo [Azure Deploy] Publish completed successfully.
goto end

:error
echo [Azure Deploy] Failed to publish SimulatorProject.csproj.
exit /b 1

:end
endlocal
exit /b 0
