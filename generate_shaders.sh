#!/bin/bash

echo "Unity Toon Shader Generator"
echo "=========================="
echo

echo "Generating shader files from common properties..."
python3 generate_shaders.py

if [ $? -eq 0 ]; then
    echo
    echo "Shader generation completed successfully!"
    echo "Both UnityToon.shader and UnityToonTessellation.shader have been updated."
    echo "Original files have been backed up with .backup extension."
else
    echo
    echo "Shader generation failed!"
    echo "Check the error messages above."
    exit 1
fi