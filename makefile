﻿publish:
	dotnet publish ./AudioDelay/AudioDelay.csproj -c Release  -o out --ucr --self-contained
	iscc ./InstallationFiles/InstallerScript.iss