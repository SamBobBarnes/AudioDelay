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

## Development
This project is developed in C# using JetBrains Rider. The project includes unit tests to ensure the functionality of the application.  

## Contributing
Contributions are welcome! Please feel free to submit a pull request.  

## License
This project is licensed under the terms of the GPL license.