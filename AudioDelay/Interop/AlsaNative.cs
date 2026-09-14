using System.Runtime.InteropServices;
using System.Text;

namespace AudioDelay.Interop;

internal static class AlsaNative
{
  internal const int PlaybackStream = 0;
  internal const int CaptureStream = 1;
  internal const int AccessRwInterleaved = 3;
  internal const int FormatS16Le = 2;

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_pcm_open")]
  private static extern int Open(out IntPtr pcm, byte[] name, int stream, int mode);

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_pcm_close")]
  internal static extern int Close(IntPtr pcm);

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_pcm_prepare")]
  internal static extern int Prepare(IntPtr pcm);

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_pcm_drop")]
  internal static extern int Drop(IntPtr pcm);

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_pcm_drain")]
  internal static extern int Drain(IntPtr pcm);

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_pcm_recover")]
  internal static extern long Recover(IntPtr pcm, long error, int silent);

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_pcm_readi")]
  internal static extern long ReadInterleaved(IntPtr pcm, byte[] buffer, ulong size);

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_pcm_writei")]
  internal static extern long WriteInterleaved(IntPtr pcm, byte[] buffer, ulong size);

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_pcm_set_params")]
  private static extern int SetParams(
    IntPtr pcm,
    int format,
    int access,
    uint channels,
    uint rate,
    int softResample,
    uint latencyUs);

  [DllImport("asound", CallingConvention = CallingConvention.Cdecl, EntryPoint = "snd_strerror")]
  private static extern IntPtr StrError(int error);

  internal static IntPtr OpenPcm(string name, int stream)
  {
    var encodedName = Encoding.ASCII.GetBytes($"{name}\0");
    var error = Open(out var pcm, encodedName, stream, 0);
    ThrowIfError(error, $"opening ALSA {(stream == CaptureStream ? "capture" : "playback")} device '{name}'");
    return pcm;
  }

  internal static void ConfigurePcm(IntPtr pcm, uint channels, uint sampleRate, uint latencyUs)
  {
    var error = SetParams(pcm, FormatS16Le, AccessRwInterleaved, channels, sampleRate, 1, latencyUs);
    ThrowIfError(error, "configuring ALSA stream");
  }

  internal static void ThrowIfError(long error, string operation)
  {
    if (error >= 0)
      return;

    var message = Marshal.PtrToStringAnsi(StrError((int)error)) ?? $"ALSA error {error}";
    throw new InvalidOperationException($"Error {operation}: {message}");
  }
}
