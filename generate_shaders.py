#!/usr/bin/env python3

import re
import os
import shutil

def generate_shader_files():
    try:
        # Read the common properties file
        common_properties_path = "com.unity.toonshader/Runtime/Integrated/Shaders/CommonPropertiesWithComments.txt"
        with open(common_properties_path, 'r') as f:
            common_properties = f.read()
        
        if not common_properties:
            print("ERROR: Common properties file is empty or could not be read")
            return False
        
        print(f"Successfully read common properties file. Length: {len(common_properties)} characters")
        
        # Read the tessellation properties file
        tessellation_properties_path = "com.unity.toonshader/Runtime/Integrated/Shaders/TessellationProperties.txt"
        with open(tessellation_properties_path, 'r') as f:
            tessellation_properties = f.read()
        
        if not tessellation_properties:
            print("ERROR: Tessellation properties file is empty or could not be read")
            return False
        
        print(f"Successfully read tessellation properties file. Length: {len(tessellation_properties)} characters")
        
        # Generate UnityToon.shader
        print("\nGenerating UnityToon.shader...")
        success1 = generate_shader("com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon.shader", 
                                 common_properties, "")
        
        # Generate UnityToonTessellation.shader
        print("\nGenerating UnityToonTessellation.shader...")
        success2 = generate_shader("com.unity.toonshader/Runtime/Integrated/Shaders/UnityToonTessellation.shader", 
                                 common_properties, tessellation_properties)
        
        if success1 and success2:
            print("\nBoth shader files generated successfully!")
            return True
        else:
            print("\nSome shader files failed to generate.")
            return False
        
    except Exception as e:
        print(f"ERROR: {e}")
        import traceback
        traceback.print_exc()
        return False

def generate_shader(shader_path, common_properties, tessellation_properties):
    try:
        # Read the original shader file
        with open(shader_path, 'r') as f:
            original_content = f.read()
        
        if not original_content:
            print(f"ERROR: {shader_path} file is empty or could not be read")
            return False
        
        print(f"Successfully read {shader_path}. Length: {len(original_content)} characters")
        
        # Find the Properties block
        properties_pattern = r"Properties\s*\{"
        start_match = re.search(properties_pattern, original_content)
        
        if not start_match:
            print(f"ERROR: Could not find Properties block start in {shader_path}")
            return False
        
        print(f"Found Properties block start at position {start_match.start()}")
        
        # Find the matching closing brace
        start_index = start_match.start()
        brace_count = 0
        end_index = start_index
        found_start = False
        
        for i in range(start_index, len(original_content)):
            if original_content[i] == '{':
                brace_count += 1
                found_start = True
            elif original_content[i] == '}':
                brace_count -= 1
                if found_start and brace_count == 0:
                    end_index = i
                    break
        
        if brace_count != 0:
            print(f"ERROR: Could not find matching closing brace for Properties block in {shader_path}")
            return False
        
        print(f"Found Properties block end at position {end_index}")
        
        # Build new Properties block
        new_properties = []
        new_properties.append("    Properties {")
        
        # Add common properties
        common_lines = common_properties.split('\n')
        property_count = 0
        for line in common_lines:
            if line.strip():
                new_properties.append(f"        {line.strip()}")
                if not line.strip().startswith("//"):
                    property_count += 1
        
        # Add tessellation properties if provided
        if tessellation_properties:
            new_properties.append("")
            new_properties.append("        // Tessellation-specific properties")
            tessellation_lines = tessellation_properties.split('\n')
            for line in tessellation_lines:
                if line.strip():
                    new_properties.append(f"        {line.strip()}")
                    if not line.strip().startswith("//"):
                        property_count += 1
        
        new_properties.append("    }")
        
        new_properties_text = '\n'.join(new_properties)
        
        print(f"Generated new Properties block with {property_count} properties. Length: {len(new_properties_text)} characters")
        
        # Create backup of original file
        backup_path = shader_path + ".backup"
        shutil.copy2(shader_path, backup_path)
        print(f"Created backup at {backup_path}")
        
        # Replace the Properties block
        new_content = original_content[:start_index] + new_properties_text + original_content[end_index + 1:]
        
        print(f"Generated new shader content. Original length: {len(original_content)}, New length: {len(new_content)}")
        
        # Write the new shader file
        with open(shader_path, 'w') as f:
            f.write(new_content)
        
        print(f"Successfully wrote {shader_path}")
        return True
        
    except Exception as e:
        print(f"ERROR generating {shader_path}: {e}")
        return False

if __name__ == "__main__":
    success = generate_shader_files()
    if success:
        print("\nAll shader files generated successfully!")
    else:
        print("\nShader generation failed!")