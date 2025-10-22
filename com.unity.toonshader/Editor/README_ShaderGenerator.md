# Unity Toon Shader Generator

This tool helps maintain consistency between `UnityToon.shader` and `UnityToonTessellation.shader` by using a single source of truth for shared properties.

## Files

- **CommonProperties.txt**: Contains all shared properties between the two shader files
- **TessellationProperties.txt**: Contains tessellation-specific properties only present in the tessellation shader
- **ShaderGenerator.cs**: Unity Editor script that generates the shader files from the property files

## How to Use

1. **Open the Shader Generator Window**:
   - In Unity, go to `Tools > Unity Toon Shader > Generate Shader Files`
   - This opens the Shader Generator window

2. **Edit Properties**:
   - Click "Open Common Properties File" to edit shared properties
   - Click "Open Tessellation Properties File" to edit tessellation-specific properties
   - Make your changes to the property files

3. **Generate Shader Files**:
   - Click "Generate Shader Files" button
   - The tool will automatically:
     - Create backups of the original shader files (`.backup` extension)
     - Replace the Properties blocks in both shader files
     - Preserve all other shader content (HLSLINCLUDE, SubShaders, etc.)

## Property File Format

The property files use the same format as ShaderLab Properties blocks, but without the `Properties { }` wrapper:

```
// Comments are supported
[HideInInspector] _simpleUI ("SimpleUI", Int ) = 0
[Enum(OFF, 0, ON, 1)] _isUnityToonshader("Material is touched by Unity Toon Shader", Int) = 1
_BaseColor ("BaseColor", Color) = (1,1,1,1)
```

## Benefits

- **Single Source of Truth**: All shared properties are defined in one place
- **Consistency**: Ensures both shader files have identical shared properties
- **Maintainability**: Easy to add, remove, or modify properties across both shaders
- **Backup Safety**: Original files are automatically backed up before generation
- **Preservation**: All non-Properties content is preserved during generation

## Manual Generation (Alternative)

If you prefer to generate shaders manually or from command line, you can use the Python script:

```bash
cd /workspace
python3 generate_shaders.py
```

This will generate both shader files from the property files.

## Troubleshooting

- **Properties block not found**: Ensure the shader files have a valid `Properties { }` block
- **File not found errors**: Check that the property files exist in the correct paths
- **Generation fails**: Check the Unity Console for detailed error messages
- **Restore from backup**: If generation fails, you can restore from the `.backup` files

## File Structure

```
com.unity.toonshader/
├── Editor/
│   ├── ShaderGenerator.cs          # Unity Editor script
│   └── README_ShaderGenerator.md   # This file
└── Runtime/Integrated/Shaders/
    ├── CommonProperties.txt        # Shared properties
    ├── TessellationProperties.txt  # Tessellation-specific properties
    ├── UnityToon.shader           # Generated shader (regular)
    └── UnityToonTessellation.shader # Generated shader (tessellation)
```