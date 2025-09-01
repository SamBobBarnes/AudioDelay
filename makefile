publish:
	dotnet publish ./AudioDelay/AudioDelay.csproj -c Release  -o out --ucr --self-contained

create-installer:
	iscc ./InstallationFiles/InstallerScript.iss
	
