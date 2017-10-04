@echo off
setlocal

cd "%~dp0"

echo Building Astral packages
dotnet pack "%cd%\src\FunEx" --configuration Debug -o "%cd%\artifacts\" 
@if %errorlevel% neq 0 ( pause & exit /b %errorlevel%)

pause