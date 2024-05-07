package AudioRecorder;

import Args.Arguments;

import javax.sound.sampled.*;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;

public class WindowsAudioRecorder extends AudioRecorder
{
    String errStr;
    Capture capture;
    Playback playback;
    AudioFormat format;
    Thread recordingThread;
    Thread playbackThread;

    SourceDataLine sourceLine;
    TargetDataLine targetLine;

    byte[] buffer;
    int bufferLengthInBytes;

    private AudioFormat.Encoding encoding = new AudioFormat.Encoding("PCM_SIGNED");
    private int sampleSizeInBits = 16;
    private int channels = 2;
    private int frameSize = sampleSizeInBits / 8 * channels;
    private float rate = 44100.0f;
    private boolean bigEndian = true;
    private int length;
    public WindowsAudioRecorder(Arguments args) {
        super(args);
        capture = new Capture();
        playback = new Playback();

        //region AudioFormat

        format = new AudioFormat(encoding, rate, sampleSizeInBits, channels, frameSize, rate, bigEndian);
        bufferLengthInBytes = (int)(_args.getRecordingLength() * rate * frameSize / 1000);
        length = bufferLengthInBytes / format.getFrameSize();
        buffer = new byte[bufferLengthInBytes];
        //endregion

        try {
            DataLine.Info sourceInfo = new DataLine.Info(SourceDataLine.class, format);
            DataLine.Info targetInfo = new DataLine.Info(TargetDataLine.class, format);

            sourceLine = (SourceDataLine) AudioSystem.getLine(sourceInfo);
            sourceLine.open(format, bufferLengthInBytes);
            sourceLine.start();

            targetLine = (TargetDataLine) AudioSystem.getLine(targetInfo);
            targetLine.open(format, targetLine.getBufferSize());
            targetLine.start();

            recordingThread = new Thread(() -> {
                System.out.println("Recording... inside thread");
                while (true) {
                    sourceLine.write(buffer, 0, buffer.length);
                }
            });

            playbackThread = new Thread(() -> {
                System.out.println("Playing... inside thread");
                int numBytesRead;
                while (true) {
                    numBytesRead = targetLine.read(buffer, 0, buffer.length);
                    if (numBytesRead == -1) break;
                }
            });
        } catch (Exception e) {
            System.err.println(e);
        }
    }

    public void Play()
    {
        playbackThread.start();
    }

    public void StopPlayback()
    {
        playbackThread.interrupt();
        targetLine.drain();
        targetLine.stop();
        targetLine.close();
        targetLine = null;
    }

    public void Start()
    {
        recordingThread.start();
    }

    public void Stop()
    {
        recordingThread.interrupt();
        sourceLine.drain();
        sourceLine.stop();
        sourceLine.close();
        sourceLine = null;
    }

    class Capture implements Runnable {

        TargetDataLine line;
        Thread thread;

        public void start() {
            errStr = null;
            thread = new Thread(this);
            thread.setName("Capture");
            thread.start();
        }

        public void stop() {
            thread = null;
        }

        private void shutDown(String message) {
            if ((errStr = message) != null && thread != null) {
                thread = null;
                System.err.println(errStr);
            }
        }

        public void run() {
            //write to stream from default recording device

            DataLine.Info info = new DataLine.Info(TargetDataLine.class, format);

            if (!AudioSystem.isLineSupported(info)) {
                shutDown("Line matching " + info + " not supported.");
                return;
            }

            try {
                line = (TargetDataLine) AudioSystem.getLine(info);
                line.open(format, line.getBufferSize());
            } catch (LineUnavailableException ex) {
                shutDown("Unable to open the line: " + ex);
                return;
            } catch (SecurityException ex) {
                shutDown(ex.toString());
                JavaSound.showInfoDialog();
                return;
            } catch (Exception ex) {
                shutDown(ex.toString());
                return;
            }

            ByteArrayOutputStream out = new ByteArrayOutputStream();
            int numBytesRead;

            line.start();

            while (thread != null) {
                if((numBytesRead = line.read(buffer, 0, bufferLengthInBytes)) == -1) {
                    break;
                }
                out.write(buffer, 0, numBytesRead);
            }

            line.stop();
            line.close();
            line = null;

            // stop and close the output stream
            try {
                out.flush();
                out.close();
            } catch (IOException ex) {
                ex.printStackTrace();
            }
        }
    }

    class Playback implements Runnable {

        SourceDataLine line;
        Thread thread;

        public void start() {
            errStr = null;
            thread = new Thread(this);
            thread.setName("Playback");
            thread.start();
        }

        public void stop() {
            thread = null;
        }

        private void shutDown(String message) {
            if ((errStr = message) != null) {
                System.err.println(errStr);
            }
            if (thread != null) {
                thread = null;
            }
        }

        public void run() {

            var readStream = new AudioInputStream(new ByteArrayInputStream(buffer), format, length);

            if (readStream == null) {
                shutDown("No loaded audio to play back");
                return;
            }

            try {
                readStream.reset();
            } catch (Exception e) {
                shutDown("Unable to reset the stream\n" + e);
                return;
            }

            DataLine.Info info = new DataLine.Info(SourceDataLine.class,
                                                   format);
            if (!AudioSystem.isLineSupported(info)) {
                shutDown("Line matching " + info + " not supported.");
                return;
            }

            try {
                line = (SourceDataLine) AudioSystem.getLine(info);
                line.open(format, bufferLengthInBytes);
            } catch (LineUnavailableException ex) {
                shutDown("Unable to open the line: " + ex);
                return;
            }

            byte[] data = new byte[bufferLengthInBytes];
            int numBytesRead = 0;

            line.start();

            while (thread != null) {
                try {
                    if ((numBytesRead = readStream.read(data)) == -1) {
                        break;
                    }
                    int numBytesRemaining = numBytesRead;
                    while (numBytesRemaining > 0 ) {
                        numBytesRemaining -= line.write(data, 0, numBytesRemaining);
                    }
                } catch (Exception e) {
                    shutDown("Error during playback: " + e);
                    break;
                }
            }

            if (thread != null) {
                line.drain();
            }
            line.stop();
            line.close();
            line = null;
            shutDown(null);
        }
    }
}
