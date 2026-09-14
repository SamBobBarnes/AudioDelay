using System.Runtime.InteropServices;

namespace AudioDelay.Interop;

internal static class PortAudioNative
{
  [DllImport("portaudio", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Pa_Initialize")]
  internal static extern int Initialize();

  [DllImport("portaudio", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Pa_Terminate")]
  internal static extern int Terminate();

  [DllImport("portaudio", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Pa_GetErrorText")]
  internal static extern IntPtr GetErrorText(int errorCode);

  internal static string GetErrorMessage(int errorCode) =>
    Marshal.PtrToStringAnsi(GetErrorText(errorCode)) ?? $"PortAudio error {errorCode}";
}
