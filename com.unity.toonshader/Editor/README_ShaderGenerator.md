# Unity Toon Shader Generator

This tool helps maintain consistency between `UnityToon.shader` and `UnityToonTessellation.shader` by using a single source of truth for shared properties.

## Files

- **CommonProperties.shader**: Hidden shader asset that contains the shared `Properties` block for both shaders, with original comments preserved
- **TessellationProperties.shader**: Hidden shader asset that contains tessellation-specific properties only present in the tessellation shader
- **ShaderGenerator.cs**: Unity Editor script that generates the shader files from the property files

## How to Use

1. **Open the Shader Generator Window**:
   - In Unity, go to `Tools > Unity Toon Shader > Generate Shader Files`
   - This opens the Shader Generator window

2. **Edit Properties**:
   - Click "Open Common Properties File" to edit the shared properties shader (includes all original comments)
   - Click "Open Tessellation Properties File" to edit the tessellation-specific properties shader
   - Make your changes directly in the shader assets

3. **Generate Shader Files**:
   - Click "Generate Shader Files" button
   - The tool will automatically:
     - Create backups of the original shader files (`.backup` extension)
     - Replace the Properties blocks in both shader files
     - Preserve all other shader content (HLSLINCLUDE, SubShaders, etc.)

## Property File Format

The property files are valid ShaderLab assets that wrap the shared definitions in a minimal hidden shader. The generator extracts only the body of the `Properties` block.

```
Shader "Hidden/UnityToon/CommonProperties"
{
    Properties
    {
        // Comments are preserved
        [HideInInspector] _simpleUI ("SimpleUI", Int ) = 0
        [Enum(OFF, 0, ON, 1)] _isUnityToonshader("Material is touched by Unity Toon Shader", Int) = 1
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass { }
    }
}
```

Inside the `Properties` block you can use the same syntax as in any ShaderLab shader. Comments and blank lines are preserved.

## Benefits

- **Single Source of Truth**: All shared properties are defined in one place
- **Consistency**: Ensures both shader files have identical shared properties
- **Maintainability**: Easy to add, remove, or modify properties across both shaders
- **Backup Safety**: Original files are automatically backed up before generation
- **Preservation**: All non-Properties content and comments are preserved during generation
- **Comment Preservation**: All original comments from the Properties blocks are maintained

## Manual Generation (Alternative)

If you prefer to generate shaders manually or from command line, you can use the Python script:

```bash
cd /workspace
python3 generate_shaders.py
```

This will generate both shader files from the property shader assets.

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
    ├── CommonProperties.shader         # Shared properties with comments (hidden shader asset)
    ├── TessellationProperties.shader   # Tessellation-specific properties (hidden shader asset)
    ├── UnityToon.shader                # Generated shader (regular)
    └── UnityToonTessellation.shader    # Generated shader (tessellation)
```