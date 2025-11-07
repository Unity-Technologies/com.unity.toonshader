#!/usr/bin/env python3

import os
import re

from generate_UnityToon import (
    extract_properties_from_shader_content,
    apply_auto_generated_comment,
)


def test_shader_generation():
    try:
        # Test reading the common properties shader
        common_properties_path = "com.unity.toonshader/Runtime/Shaders/Common/Parts/CommonProperties.shaderblock"
        with open(common_properties_path, 'r') as f:
            common_properties_shader = f.read()

        if not common_properties_shader:
            print("ERROR: Common properties shader file is empty or could not be read")
            return

        common_properties = extract_properties_from_shader_content(common_properties_shader)
        if not common_properties:
            print("ERROR: Failed to extract common properties from shader")
            return

        print(f"Successfully extracted common properties. Length: {len(common_properties)} characters")

        # Test reading the tessellation properties shader
        tessellation_properties_path = "com.unity.toonshader/Runtime/Shaders/Common/Parts/TessellationProperties.shaderblock"
        with open(tessellation_properties_path, 'r') as f:
            tessellation_properties_shader = f.read()

        if not tessellation_properties_shader:
            print("ERROR: Tessellation properties shader file is empty or could not be read")
            return

        tessellation_properties = extract_properties_from_shader_content(tessellation_properties_shader)
        if not tessellation_properties:
            print("ERROR: Failed to extract tessellation properties from shader")
            return

        print(f"Successfully extracted tessellation properties. Length: {len(tessellation_properties)} characters")

        # Test reading the original shader files
        unity_toon_path = "com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon.shader"
        with open(unity_toon_path, 'r') as f:
            unity_toon_content = f.read()

        if not unity_toon_content:
            print("ERROR: UnityToon.shader file is empty or could not be read")
            return

        print(f"Successfully read UnityToon.shader. Length: {len(unity_toon_content)} characters")

        # Test the Properties block replacement
        properties_pattern = r"Properties\s*\{"
        start_match = re.search(properties_pattern, unity_toon_content)

        if not start_match:
            print("ERROR: Could not find Properties block start in shader file")
            return

        print(f"Found Properties block start at position {start_match.start()}")

        # Find the matching closing brace
        start_index = start_match.start()
        brace_count = 0
        end_index = start_index
        found_start = False

        for i in range(start_index, len(unity_toon_content)):
            if unity_toon_content[i] == '{':
                brace_count += 1
                found_start = True
            elif unity_toon_content[i] == '}':
                brace_count -= 1
                if found_start and brace_count == 0:
                    end_index = i
                    break

        if brace_count != 0:
            print("ERROR: Could not find matching closing brace for Properties block")
            return

        print(f"Found Properties block end at position {end_index}")

        # Build new Properties block
        new_properties = []
        new_properties.append("    Properties {")

        # Add common properties
        common_lines = common_properties.split('\n')
        property_count = 0
        for line in common_lines:
            if line.strip() and not line.strip().startswith("//"):
                new_properties.append(f"        {line.strip()}")
                property_count += 1

        new_properties.append("    }")

        new_properties_text = '\n'.join(new_properties)

        print(f"Generated new Properties block with {property_count} properties. Length: {len(new_properties_text)} characters")

        # Test the replacement
        new_content = unity_toon_content[:start_index] + new_properties_text + unity_toon_content[end_index + 1:]
        new_content = apply_auto_generated_comment(new_content, "//Auto-generated (test output)")

        print(f"Generated new shader content. Original length: {len(unity_toon_content)}, New length: {len(new_content)}")

        # Write test file
        test_path = "UnityToon_Generated_Test.shader"
        with open(test_path, 'w') as f:
            f.write(new_content)

        print(f"Test shader written to {test_path}")
        print("Shader generation test completed successfully!")

        try:
            import os
            os.remove(test_path)
        except OSError:
            pass

    except Exception as e:
        print(f"ERROR: {e}")
        import traceback
        traceback.print_exc()


if __name__ == "__main__":
    test_shader_generation()
