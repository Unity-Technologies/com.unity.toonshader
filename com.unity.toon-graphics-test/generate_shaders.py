#!/usr/bin/env python3

import re
from datetime import datetime, timezone


def extract_properties_from_shader_content(content):
    properties_match = re.search(r"Properties\s*\{", content)
    if not properties_match:
        return None

    open_brace_index = content.find('{', properties_match.start())
    if open_brace_index == -1:
        return None

    brace_count = 1
    close_brace_index = -1

    for index in range(open_brace_index + 1, len(content)):
        char = content[index]
        if char == '{':
            brace_count += 1
        elif char == '}':
            brace_count -= 1
            if brace_count == 0:
                close_brace_index = index
                break

    if close_brace_index == -1:
        return None

    block = content[open_brace_index + 1:close_brace_index]
    lines = block.split('\n')
    cleaned_lines = [line.strip() for line in lines]

    while cleaned_lines and cleaned_lines[0] == "":
        cleaned_lines.pop(0)
    while cleaned_lines and cleaned_lines[-1] == "":
        cleaned_lines.pop()

    return '\n'.join(cleaned_lines)


def load_properties_from_shader(shader_path, descriptor):
    with open(shader_path, 'r') as f:
        shader_content = f.read()

    if not shader_content:
        print(f"ERROR: {descriptor} shader file is empty or could not be read")
        return None

    properties = extract_properties_from_shader_content(shader_content)
    if not properties:
        print(f"ERROR: Could not extract {descriptor} properties from {shader_path}")
        return None

    print(f"Successfully extracted {descriptor} properties. Length: {len(properties)} characters")
    return properties


def generate_shader_files():
    try:
        # Read the common properties shader
        common_properties_path = "com.unity.toonshader/Runtime/Integrated/Shaders/CommonPropertiesPart.shader"
        common_properties = load_properties_from_shader(common_properties_path, "common")
        if not common_properties:
            return False

        # Read the tessellation properties shader
        tessellation_properties_path = "com.unity.toonshader/Runtime/Integrated/Shaders/TessellationPropertiesPart.shader"
        tessellation_properties = load_properties_from_shader(tessellation_properties_path, "tessellation")
        if tessellation_properties is None:
            return False

        timestamp = datetime.now(timezone.utc).strftime("%a %b %d %H:%M:%S UTC %Y")
        auto_comment_line = f"//Auto-generated on {timestamp}"

        # Generate UnityToon.shader
        print("\nGenerating UnityToon.shader...")
        success1 = generate_shader(
            "com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon.shader",
            common_properties,
            "",
            auto_comment_line,
        )

        # Generate UnityToonTessellation.shader
        print("\nGenerating UnityToonTessellation.shader...")
        success2 = generate_shader(
            "com.unity.toonshader/Runtime/Integrated/Shaders/UnityToonTessellation.shader",
            common_properties,
            tessellation_properties,
            auto_comment_line,
        )

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


def generate_shader(shader_path, common_properties, tessellation_properties, auto_comment_line):
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

        # Determine indentation
        line_start = original_content.rfind('\n', 0, start_index)
        if line_start == -1:
            line_start = 0
        else:
            line_start += 1
        base_indent = "    "
        block_indent = base_indent + "    "

        # Build new Properties block
        new_properties = []
        new_properties.append(f"{base_indent}Properties {{")

        # Add common properties
        common_lines = common_properties.split('\n')
        property_count = 0
        for line in common_lines:
            stripped_line = line.strip()
            if stripped_line:
                new_properties.append(f"{block_indent}{stripped_line}")
                if not stripped_line.startswith("//"):
                    property_count += 1

        # Add tessellation properties if provided
        if tessellation_properties:
            new_properties.append("")
            new_properties.append(f"{block_indent}// Tessellation-specific properties")
            tessellation_lines = tessellation_properties.split('\n')
            for line in tessellation_lines:
                stripped_line = line.strip()
                if stripped_line:
                    new_properties.append(f"{block_indent}{stripped_line}")
                    if not stripped_line.startswith("//"):
                        property_count += 1

        new_properties.append(f"{base_indent}}}")

        new_properties_text = '\n'.join(new_properties)

        print(f"Generated new Properties block with {property_count} properties. Length: {len(new_properties_text)} characters")

        # Replace the Properties block
        new_content = original_content[:line_start] + new_properties_text + original_content[end_index + 1:]

        new_content = apply_auto_generated_comment(new_content, auto_comment_line)

        print(f"Generated new shader content. Original length: {len(original_content)}, New length: {len(new_content)}")

        # Write the new shader file
        with open(shader_path, 'w') as f:
            f.write(new_content)

        print(f"Successfully wrote {shader_path}")
        return True

    except Exception as e:
        print(f"ERROR generating {shader_path}: {e}")
        return False


def apply_auto_generated_comment(content, comment_line):
    auto_prefix = "//Auto-generated on "
    lines = content.splitlines()

    if lines and lines[0].startswith(auto_prefix):
        lines[0] = comment_line
    else:
        lines.insert(0, comment_line)

    joined = "\n".join(lines)
    if not joined.endswith("\n"):
        joined += "\n"
    return joined


if __name__ == "__main__":
    success = generate_shader_files()
    if success:
        print("\nAll shader files generated successfully!")
    else:
        print("\nShader generation failed!")
