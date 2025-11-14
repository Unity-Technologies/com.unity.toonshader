# replace tabs
git ls-files -z '*.cs' '*.md' '*.shader' '*.cginc' '*.hlsl' | xargs -0 sed -Ei 's/\t/    /g'

# remove UTF-8 BOM, and trailing spaces or tabs at the end of lines
git ls-files -z '*.cs' '*.md' '*.shader' '*.cginc' '*.hlsl' | xargs -0 sed -Ei 's/\r//g;s/\s+$//;s/\xef\xbb\xbf//g'

