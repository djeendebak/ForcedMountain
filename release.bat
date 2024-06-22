@echo off
dotnet build
set "source_folder=ForcedMountain\bin\Debug\netstandard2.0"
set "destination_folder=ThunderStore\plugins\ForcedMountain"
set "file_to_copy=ForcedMountain.dll"

copy "%source_folder%\%file_to_copy%" "%destination_folder%"
copy "README.md" "ThunderStore\"
PowerShell -Command "Compress-Archive -Path 'ThunderStore\*' -DestinationPath 'ForcedMountain.zip' -Force"
