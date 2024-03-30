using NAudio.Wave;

namespace AudioDelay;

public class AudioRecorder
{
    private WaveInEvent waveIn;
    private WaveFileWriter writer;
    private MemoryStream memoryStream;

    public AudioRecorder(WaveFormat format)
    {
        waveIn = new WaveInEvent();
        waveIn.DataAvailable += OnDataAvailable;
        waveIn.WaveFormat = format; // CD quality audio, mono channel
        memoryStream = new MemoryStream();
        writer = new WaveFileWriter(new IgnoreDisposeStream(memoryStream),waveIn.WaveFormat);
    }

    public void StartRecording()
    {
        waveIn.StartRecording();
    }

    public void StopRecording()
    {
        waveIn.StopRecording();
        writer.Dispose(); 
        memoryStream.Position = 0;
    }
    
    public byte[] GetRecordedData()
    {
        return memoryStream.ToArray();
    }

    private void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        writer.Write(e.Buffer, 0, e.BytesRecorded);
    }
    
}

public class IgnoreDisposeStream : Stream
{
    private Stream _innerStream;

    public IgnoreDisposeStream(Stream innerStream)
    {
        _innerStream = innerStream;
    }

    protected override void Dispose(bool disposing)
    {
        // Do not dispose of the inner stream
    }

    public override void Flush()
    {
        _innerStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _innerStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _innerStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _innerStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _innerStream.Write(buffer, offset, count);
    }

    public override bool CanRead => _innerStream.CanRead;
    public override bool CanSeek => _innerStream.CanSeek;
    public override bool CanWrite => _innerStream.CanWrite;
    public override long Length => _innerStream.Length;
    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }
}