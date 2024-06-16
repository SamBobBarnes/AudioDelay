# AudioDelay

## Description
AudioDelay is a C# application that records audio and plays it back after a specified delay. It's useful for scenarios where you need to delay the playback of audio.

## Features
- Record and playback audio with a specified delay.
- Debug mode for detailed logging.
- Command-line arguments for easy configuration.

## Usage
You can configure the application using the following command-line arguments:

- `--delay`: The delay before the audio starts playing back (in milliseconds).
- `--content-length`: The length of the audio content to record (in milliseconds).
- `--help`: Display the help text.
- `--debug`: Enable debug mode for detailed logging.

Example usage:

```bash
dotnet run -- --delay 2000 --content-length 3000 --debug
```
This will start the application, record for 3000 milliseconds, and then play back the audio after a delay of 2000 milliseconds. Debug mode is also enabled.

## Installation
### Setup File (Recommended)
Installation can be completed by downloading the setup file from the latest release and executing it.

After executing, you will need to update your path file to include the installation directory. This will allow you to run the application from the command line.

#### Adding to Path
1. Click on the Windows icon.
2. Click on the Settings icon.
3. Select "System."
4. Click on "About."
5. Click on "System info."
6. Choose "Advanced system settings."
7. Click on "Environment variables..."
8. Under "System variables," find the "Path" row and click "Edit."
9. Click "New" and then "Browse." Select the folder `C:\Program Files\AudioDelay`.
10. Click "OK" and close all previous windows.

### From Source
You can also install the application from source by following these steps:

1. Clone the repository.
2. Navigate to the project directory.
3. Run the following command:
```bash
dotnet publish -c Release -r win-x64 --self-contained
```
4. Navigate to the publish directory.
5. Run the following command:
```bash
.\AudioDelay.exe --delay 2000 --content-length 3000 --debug
```

## Development
This project is developed in C# using JetBrains Rider. The project includes unit tests to ensure the functionality of the application.  

This project also uses Inno Setup to create the setup file from script.

## Contributing
Contributions are welcome! Please feel free to submit a pull request.  

## License
This project is licensed under the terms of the GPL license.