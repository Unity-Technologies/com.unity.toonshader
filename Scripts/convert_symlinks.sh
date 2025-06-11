TEMP_SYMLINK="$1Temp" 

echo
echo --------------------------------------------------------------------------
echo Converting symlinks to copies: $1 
echo --------------------------------------------------------------------------
mv $1 $TEMP_SYMLINK
rsync $TEMP_SYMLINK/ $1/ -a --copy-links -v
rm $TEMP_SYMLINK

