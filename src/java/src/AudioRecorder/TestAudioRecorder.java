package AudioRecorder;

import Args.Arguments;

import javax.sound.sampled.*;

public class TestAudioRecorder {

    Capture capture;
    Playback playback;
    AudioFormat format;

    byte[] buffer;
    int bufferLengthInBytes;

    //region AudioFormat/memory length
    private AudioFormat.Encoding encoding = new AudioFormat.Encoding("PCM_SIGNED");
    private int sampleSizeInBits = 16;
    private int channels = 2;
    private int frameSize = sampleSizeInBits / 8 * channels;
    private float rate = 44100.0f;
    private boolean bigEndian = true;
    private int length;
    //endregion

    //region Audio Devices
    SourceDataLine sourceLine;
    TargetDataLine targetLine;
    DataLine.Info sourceInfo = new DataLine.Info(SourceDataLine.class, format);
    DataLine.Info targetInfo = new DataLine.Info(TargetDataLine.class, format);
    //endregion

    public TestAudioRecorder(Arguments _args) {
        capture = new Capture();
        playback = new Playback();

        //region AudioFormat/memory length
        format = new AudioFormat(encoding, rate, sampleSizeInBits, channels, frameSize, rate, bigEndian);
        bufferLengthInBytes = (int)(_args.Runtime * rate * frameSize / 1000);
        length = bufferLengthInBytes / format.getFrameSize();
        buffer = new byte[bufferLengthInBytes];
        //endregion

        try {
            //region Audio Devices
            sourceLine = (SourceDataLine) AudioSystem.getLine(sourceInfo);
            sourceLine.open(format, bufferLengthInBytes);

            targetLine = (TargetDataLine) AudioSystem.getLine(targetInfo);
            targetLine.open(format, targetLine.getBufferSize());
            //endregion



        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    public void startCapture() {
        capture.start();
    }

    public void stopCapture() {
        capture.interrupt();
    }

    public void startPlayback() {
        playback.start();
    }

    public void stopPlayback() {
        playback.interrupt();
    }

    class Capture extends Thread {
        boolean finished = false;
        public void run() {
            System.out.println("Recording...");
            sourceLine.start();
            while (!finished) {
                // recording

                sourceLine.write(buffer, 0, buffer.length);

                if(Thread.interrupted()) {
                    finished = true;
                }
            }
            sourceLine.flush();
            sourceLine.stop();
            sourceLine.close();
            sourceLine = null;
            System.out.println("Recording stopped.");
        }
    }

    class Playback extends Thread {
        boolean finished = false;
        public void run() {
            System.out.println("Playing...");
            int numBytesRead;
            targetLine.start();
            while (!finished) {
                // playback

                numBytesRead = targetLine.read(buffer, 0, buffer.length);
                if (numBytesRead == -1) break;

                if(Thread.interrupted()) {
                    finished = true;
                }
            }

            targetLine.drain();
            targetLine.stop();
            targetLine.close();
            targetLine = null;
            System.out.println("Playback stopped.");
        }
    }
}


