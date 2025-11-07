#!/usr/bin/env python3

import os
import re
from datetime import datetime, timezone

PROPERTY_NAME_PATTERN = re.compile(r'(?:\]\s*|^)([A-Za-z_][A-Za-z0-9_]*)\s*\(')


def extract_properties_from_shader_content(content):
    properties_match = re.search(r"Properties\s*\{", content)
    if not properties_match:
        return content.strip()

    open_brace_index = content.find('{', properties_match.start())
    if open_brace_index == -1:
        return content.strip()

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
        return content.strip()

    block = content[open_brace_index + 1:close_brace_index]
    lines = block.split('\n')
    cleaned_lines = [line.strip() for line in lines]

    while cleaned_lines and cleaned_lines[0] == "":
        cleaned_lines.pop(0)
    while cleaned_lines and cleaned_lines[-1] == "":
        cleaned_lines.pop()

    return '\n'.join(cleaned_lines)


def get_property_name(line: str):
    if not line or line.startswith("//"):
        return None
    matches = PROPERTY_NAME_PATTERN.findall(line)
    if not matches:
        return None
    candidate = matches[-1]
    if candidate.startswith('['):
        return None
    return candidate


def cleanup_lines(lines):
    lines = list(lines)
    while lines and lines[0] == "":
        lines.pop(0)
    while lines and lines[-1] == "":
        lines.pop()
    cleaned = []
    for line in lines:
        if line == "" and cleaned and cleaned[-1] == "":
            continue
        cleaned.append(line)
    return cleaned


def build_property_sections(common_properties, tessellation_properties, indent):
    entries = []
    name_to_index = {}

    def add_lines(raw_text, source):
        if not raw_text:
            return
        for raw_line in raw_text.splitlines():
            stripped = raw_line.strip()
            entry = {
                "line": stripped,
                "source": source,
                "is_hidden": "[HideInInspector]" in stripped,
            }
            if stripped == "":
                entry["name"] = None
                entries.append(entry)
                continue
            name = get_property_name(stripped)
            entry["name"] = name
            source_map = name_to_index.setdefault(source, {})
            if name and name in source_map:
                existing_index = source_map[name]
                existing_entry = entries[existing_index]

                def should_replace(existing, new):
                    if existing["is_hidden"] and not new["is_hidden"]:
                        return True
                    if not existing["is_hidden"] and new["is_hidden"]:
                        return False
                    return True

                if should_replace(existing_entry, entry):
                    entries[existing_index] = entry
            else:
                entries.append(entry)
                if name:
                    source_map[name] = len(entries) - 1

    add_lines(common_properties, "common")
    add_lines(tessellation_properties, "tess")

    seen_pairs = set()
    property_count = 0
    tess_override_names = set(name_to_index.get("tess", {}).keys())
    for entry in entries:
        entry["skip_for_tess"] = entry["source"] == "common" and entry.get("name") in tess_override_names
    for entry in entries:
        name = entry.get("name")
        if not name:
            continue
        key = (entry["source"], name)
        if key in seen_pairs:
            continue
        seen_pairs.add(key)
        property_count += 1

    def render(section_source, skip_overrides=False):
        lines = []
        for entry in entries:
            if entry["source"] != section_source:
                continue
            if skip_overrides and entry.get("skip_for_tess"):
                continue
            if entry["line"]:
                lines.append(indent + entry["line"])
            else:
                lines.append("")
        return "\n".join(cleanup_lines(lines))

    common_block = render("common")
    common_block_for_tess = render("common", skip_overrides=True)
    tess_block = render("tess")

    return property_count, common_block, tess_block, common_block_for_tess


def load_properties_from_shader(shader_path, descriptor):
    with open(shader_path, 'r', encoding='utf-8') as f:
        shader_content = f.read()

    if not shader_content:
        print(f"ERROR: {descriptor} shader file is empty or could not be read")
        return None

    shader_content = shader_content.lstrip('\ufeff')

    properties = extract_properties_from_shader_content(shader_content)
    if not properties:
        print(f"ERROR: Could not extract {descriptor} properties from {shader_path}")
        return None

    print(f"Successfully extracted {descriptor} properties. Length: {len(properties)} characters")
    return properties


def generate_shader_files():
    try:
        # Read the common properties shader
        common_properties_path = "com.unity.toonshader/Runtime/Shaders/Common/Parts/CommonProperties.shaderblock"
        common_properties = load_properties_from_shader(common_properties_path, "common")
        if not common_properties:
            return False

        # Read the tessellation properties shader
        tessellation_properties_path = "com.unity.toonshader/Runtime/Shaders/Common/Parts/TessellationProperties.shaderblock"
        tessellation_properties = load_properties_from_shader(tessellation_properties_path, "tessellation")
        if tessellation_properties is None:
            return False

        timestamp = datetime.now(timezone.utc).strftime("%a %b %d %H:%M:%S UTC %Y")
        auto_comment_line = f"//Auto-generated on {timestamp}"

        # Generate UnityToon.shader
        print("\nGenerating UnityToon.shader...")
        success1 = generate_shader(
            "com.unity.toonshader/Runtime/Integrated/Shaders/UnityToon.shader",
            "com.unity.toonshader/Runtime/Shaders/Common/Parts/UnityToon.shadertemplate",
            common_properties,
            "",
            auto_comment_line,
        )

        # Generate UnityToonTessellation.shader
        print("\nGenerating UnityToonTessellation.shader...")
        success2 = generate_shader(
            "com.unity.toonshader/Runtime/Integrated/Shaders/UnityToonTessellation.shader",
            "com.unity.toonshader/Runtime/Shaders/Common/Parts/UnityToonTessellation.shadertemplate",
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


def generate_shader(shader_path, template_path, common_properties, tessellation_properties, auto_comment_line):
    try:
        # Read the template or existing shader file
        if template_path and os.path.exists(template_path):
            with open(template_path, 'r', encoding='utf-8') as f:
                original_content = f.read()
            source_path = template_path
        elif os.path.exists(shader_path):
            with open(shader_path, 'r', encoding='utf-8') as f:
                original_content = f.read()
            source_path = shader_path
        else:
            print(f"ERROR: Could not locate shader or template for {shader_path}")
            return False

        if not original_content:
            print(f"ERROR: {shader_path} file is empty or could not be read")
            return False

        original_content = original_content.lstrip('\ufeff')

        print(f"Successfully read {source_path}. Length: {len(original_content)} characters")

        property_count, common_block, tess_block, common_block_for_tess = build_property_sections(common_properties, tessellation_properties, "        ")

        use_common_block = common_block_for_tess if tessellation_properties else common_block

        new_content = original_content

        if "        [COMMON_PROPERTIES]" in new_content:
            new_content = new_content.replace("        [COMMON_PROPERTIES]", use_common_block or "", 1)
        elif "[COMMON_PROPERTIES]" in new_content:
            new_content = new_content.replace("[COMMON_PROPERTIES]", use_common_block or "", 1)
        else:
            print(f"ERROR: Common properties placeholder not found in template for {shader_path}")
            return False

        if "[TESSELLATION_PROPERTIES]" in new_content:
            replacement = tess_block or ""
            new_content = new_content.replace("        [TESSELLATION_PROPERTIES]", replacement, 1)
            new_content = new_content.replace("[TESSELLATION_PROPERTIES]", replacement, 1)

        print(f"Generated properties with {property_count} entries. Common length: {len(common_block)}, Tess length: {len(tess_block)}")

        new_content = apply_auto_generated_comment(new_content, auto_comment_line)

        print(f"Generated new shader content. Original length: {len(original_content)}, New length: {len(new_content)}")

        # Write the new shader file
        new_content = new_content.lstrip('\ufeff')
        new_content = new_content.replace('\r\n', '\n').replace('\r', '\n')
        if not new_content.endswith('\n'):
            new_content += '\n'
        os.makedirs(os.path.dirname(shader_path), exist_ok=True)
        with open(shader_path, 'w', encoding='utf-8') as f:
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
