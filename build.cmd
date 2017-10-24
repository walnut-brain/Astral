@echo off
setlocal

cd "%~dp0"

echo Building Astral packages
dotnet pack "%cd%\src\Astral.Core" --configuration Debug -o "%cd%\artifacts\" 
dotnet pack "%cd%\src\Astral.Markup" --configuration Debug -o "%cd%\artifacts\"
dotnet pack "%cd%\src\Astral.Payloads" --configuration Debug -o "%cd%\artifacts\" 
dotnet pack "%cd%\src\Astral.Payloads.Json" --configuration Debug -o "%cd%\artifacts\" 
dotnet pack "%cd%\src\Astral.Schema" --configuration Debug -o "%cd%\artifacts\"
dotnet pack "%cd%\src\Astral.Schema.Json" --configuration Debug -o "%cd%\artifacts\"
dotnet pack "%cd%\src\Astral.Schema.Yaml" --configuration Debug -o "%cd%\artifacts\"
dotnet pack "%cd%\src\Astral" --configuration Debug -o "%cd%\artifacts\" 
dotnet pack "%cd%\src\Astral.RabbitLink" --configuration Debug -o "%cd%\artifacts\" 
@if %errorlevel% neq 0 ( pause & exit /b %errorlevel%)

pause