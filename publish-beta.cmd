@echo off
setlocal

cd "%~dp0"

echo Building Astral packages
dotnet pack "%cd%\src\Astral.Fun" --configuration Debug -o "%cd%\artifacts\" 
dotnet pack "%cd%\src\Astral.Contracts" --configuration Debug -o "%cd%\artifacts\" 
dotnet pack "%cd%\src\Astral.Markup" --configuration Debug -o "%cd%\artifacts\"
dotnet pack "%cd%\src\Astral.Payloads" --configuration Debug -o "%cd%\artifacts\" 
dotnet pack "%cd%\src\Astral" --configuration Debug -o "%cd%\artifacts\" 
dotnet pack "%cd%\src\RabbitLink.Services" --configuration Debug -o "%cd%\artifacts\" 
dotnet pack "%cd%\src\RabbitLink.Services.Astral" --configuration Debug -o "%cd%\artifacts\" 
@if %errorlevel% neq 0 ( pause & exit /b %errorlevel%)

pause