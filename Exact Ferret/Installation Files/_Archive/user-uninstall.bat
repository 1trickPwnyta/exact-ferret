@echo off
@setlocal enableextensions
@cd /d "%~dp0"
powershell -ExecutionPolicy ByPass .\user-uninstall-helper.ps1