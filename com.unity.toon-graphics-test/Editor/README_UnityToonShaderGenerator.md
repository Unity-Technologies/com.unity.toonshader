# Unity Toon Shader Generator

This tool helps maintain consistency between `UnityToon.shader` and `UnityToonTessellation.shader` by using a single source of truth for shared properties.

## Files

- **CommonProperties.shaderblock**: Plain text list of shared property definitions (no Shader wrapper) with original comments preserved (`Packages/com.unity.toonshader/Runtime/Shaders/Common/Parts/`)
- **TessellationProperties.shaderblock**: Plain text list of tessellation-only property definitions (no Shader wrapper) (`Packages/com.unity.toonshader/Runtime/Shaders/Common/Parts/`)
- **ShaderGenerator.cs**: Unity Editor script (now located in the graphics test package) that generates the shader files from the property assets

## How to Use

1. **Edit Properties**:
   - Modify the plain-text part files under `Packages/com.unity.toonshader/Runtime/Shaders/Common/Parts/`
   - The files contain only property declarations, so standard ShaderLab property syntax applies

2. **Generate Shader Files**:
   - In Unity, choose `Toon Shader > Generate Shader Files`
   - The tool will automatically:
     - Replace the Properties blocks in both shader files
     - Preserve all other shader content (HLSLINCLUDE, SubShaders, etc.)

## Property File Format

Each property file is plain text containing only property declarations, e.g.

```
// Comments are preserved
[HideInInspector] _simpleUI ("SimpleUI", Int ) = 0
[Enum(OFF, 0, ON, 1)] _isUnityToonshader("Material is touched by Unity Toon Shader", Int) = 1
_BaseColor ("BaseColor", Color) = (1,1,1,1)
```

You can use the usual ShaderLab property syntax, including comments and blank lines. The generator indents/layers them into the target shaders automatically.

## Benefits

- **Single Source of Truth**: All shared properties are defined in one place
- **Consistency**: Ensures both shader files have identical shared properties
- **Maintainability**: Easy to add, remove, or modify properties across both shaders
- **Preservation**: All non-Properties content and comments are preserved during generation
- **Comment Preservation**: All original comments from the Properties blocks are maintained
- **Traceability**: Each generated shader receives an `//Auto-generated on ...` timestamp banner at the top

## Manual Generation (Alternative)

If you prefer to generate shaders manually, you can run the Python helper from the repository root:

```bash
cd /workspace
python3 generate_UnityToon.py
```

Convenience launchers are provided as `generate_UnityToon.sh` and `generate_UnityToon.bat`.

## Troubleshooting

- **Properties block not found**: Ensure the shader files have a valid `Properties { }` block
- **File not found errors**: Check that the property files exist in the correct paths
- **Generation fails**: Check the Unity Console for detailed error messages

## File Structure

```
com.unity.toon-graphics-test/
└── Editor/
    ├── UnityToonShaderGenerator.cs          # Unity Editor script
    ├── UnityToonShaderGeneratorTest.cs      # Editor test harness
    └── README_UnityToonShaderGenerator.md   # This file

com.unity.toonshader/Runtime/Shaders/Common/Parts/
├── CommonProperties.shaderblock          # Shared properties (plain text)
└── TessellationProperties.shaderblock    # Tessellation-specific properties (plain text)

com.unity.toonshader/Runtime/Integrated/Shaders/
├── UnityToon.shader                # Generated shader (regular)
└── UnityToonTessellation.shader    # Generated shader (tessellation)

generate_UnityToon.py                 # Python generator (root)
test_generate_UnityToon.py            # Python smoke test (root)
test_shader_generation.cs             # .NET console smoke test (root)
generate_UnityToon.sh / .bat          # Platform launchers for the Python generator
```