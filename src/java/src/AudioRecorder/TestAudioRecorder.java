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
    private int channels = 1;
    private int frameSize = sampleSizeInBits / 8 * channels;
    private float samplingRate = 44100.0f;
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
        format = new AudioFormat(encoding, samplingRate, sampleSizeInBits, channels, frameSize, samplingRate, bigEndian);
        bufferLengthInBytes = (int) (_args.Runtime * samplingRate * frameSize / 1000);
        length = bufferLengthInBytes / format.getFrameSize();
        buffer = new byte[bufferLengthInBytes];
        //endregion

        try {
            //region Audio Devices
            sourceLine = (SourceDataLine) AudioSystem.getLine(sourceInfo);
            sourceLine.open(format, sourceLine.getBufferSize());

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
            targetLine.start();
            int offset = 0;
            int numBytesRead;
            while ((numBytesRead = targetLine.read(buffer, offset, buffer.length-offset)) != -1 && !finished) {
               offset += numBytesRead;

               if(offset >= buffer.length) {
                   offset = 0;

                   if (Thread.interrupted()) {
                       finished = true;
                   }
               }
            }
            targetLine.close();
            System.out.println("Recording stopped.");
        }
    }

    class Playback extends Thread {
        boolean finished = false;
        int chunkSize = 512;
        public void run() {
            System.out.println("Playing...");

            sourceLine.start();
            int numBytesRead;
            int offset = 0;
            while (offset < buffer.length && !finished) {
                // playback

                sourceLine.write(buffer, offset, Math.min(chunkSize, buffer.length - offset));
                offset += chunkSize;

                if(Thread.interrupted()) {
                    finished = true;
                }
            }

            sourceLine.drain();
            sourceLine.close();
            System.out.println("Playback stopped.");
        }
    }
}


