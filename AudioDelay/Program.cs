// See https://aka.ms/new-console-template for more information


using AudioDelay;

Console.WriteLine("Recording started...");
AudioRecorder recorder = new AudioRecorder();
recorder.StartRecording();

// Delay for 5 seconds (5000 milliseconds)
Thread.Sleep(5000);

recorder.StopRecording();
Console.WriteLine("Recording stopped.");

recorder.GetRecordedData();

return 0;