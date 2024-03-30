// See https://aka.ms/new-console-template for more information


using AudioDelay;

var arguments = HandleArgs.ParseArgs(args);

if (arguments.Help)
{
    Console.Write(HandleArgs.GetHelpText());
    return 0;
}

AudioRecorderPlayer recorderPlayer = new AudioRecorderPlayer();
Console.WriteLine("Starting recording and playback...");
recorderPlayer.Start();

Thread.Sleep(arguments.Delay);
Console.WriteLine("Starting playback...");
recorderPlayer.Play();

Thread.Sleep(arguments.Runtime);

recorderPlayer.Stop();
Console.WriteLine("Stopped recording.");

Thread.Sleep(arguments.Delay);
recorderPlayer.StopPlayback();
Console.WriteLine("Stopped playback.");

return 0;