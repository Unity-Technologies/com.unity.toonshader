@echo off
echo Unity Toon Shader Generator
echo ==========================
echo.

echo Generating shader files from common properties...
python3 generate_UnityToon.py

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Shader generation completed successfully!
    echo Both UnityToon.shader and UnityToonTessellation.shader have been updated.
    echo Files now include an auto-generated timestamp at the top of each shader.
) else (
    echo.
    echo Shader generation failed!
    echo Check the error messages above.
)

pause
