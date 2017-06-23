cd /D C:\Windows\System32
timeout /t 2 /nobreak > NUL
taskkill /FI "IMAGENAME eq cmd*"
