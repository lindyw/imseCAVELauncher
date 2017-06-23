@echo off
timeout /t 3 /nobreak > NUL
cd /D C:\Windows\System32
:CheckPoint
timeout /t 1 /nobreak > NUL
@tasklist|find /i /c "SurvivalShooter_1.0.2.exe" > NUL
@IF %ERRORLEVEL% EQU 0 @GOTO CheckPoint
MD C:\Projects\imseCAVELauncher\Resources\AppClosed
taskkill /FI "IMAGENAME eq cmd*"
