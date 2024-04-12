test:
	dotnet publish ./AudioDelay/AudioDelay.csproj -c Release --self-contained true --runtime linux-x64 -p:PublishSingleFile=true -o ./AudioDelay/publish/linux-x64
	./AudioDelay/publish/linux-x64/AudioDelay --devices