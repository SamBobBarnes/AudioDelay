// See https://aka.ms/new-console-template for more information


using AudioDelay;
using NAudio.Wave;

var format = new WaveFormat(44100, 1);

Console.WriteLine("Recording started...");
AudioRecorder recorder = new AudioRecorder(format);
recorder.StartRecording();

// Delay for 5 seconds (5000 milliseconds)
Thread.Sleep(5000);

recorder.StopRecording();
Console.WriteLine("Recording stopped.");

var recording = recorder.GetRecordedData();

AudioPlayer player = new AudioPlayer(recording, format);

Console.WriteLine("Playing Audio...");
player.Play();

// Delay for 5 seconds (5000 milliseconds)
Thread.Sleep(5000);

player.Stop();

player.Dispose();

Console.WriteLine("Audio Finished.");

return 0;