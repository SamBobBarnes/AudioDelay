import Args.Arguments;
import AudioRecorder.CapturePlayback;
import AudioRecorder.TestAudioRecorder;
import AudioRecorder.WindowsAudioRecorder;

import javax.swing.*;
import java.awt.*;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;


public class Main {
    public static void main(String[] args) {
        console();
//        window();
    }

    private static void console() {
//        CapturePlayback capturePlayback = new CapturePlayback();
//
//        capturePlayback.record();
//        System.out.println("recording...");
//
//        //delay 5 seconds
//        try {
//            Thread.sleep(10000);
//        } catch (InterruptedException e) {
//            e.printStackTrace();
//        }
//
//
//        System.out.println("recording stopped");
//        capturePlayback.stopRecording();
//        try {
//            Thread.sleep(1000);
//        } catch (InterruptedException e) {
//            e.printStackTrace();
//        }
//        System.out.println("Playing...");
//        capturePlayback.playback();
        var args = new Arguments();
        args.Delay = 1000;
        args.Runtime = 5000;
//        var recorder = new WindowsAudioRecorder(args);

        var recorder = new TestAudioRecorder(args);

//        System.out.println("Recording...");
//        recorder.Start();
        recorder.startCapture();
        try {
            Thread.sleep(args.Delay);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

//        System.out.println("Playing...");
//        recorder.Play();
        recorder.startPlayback();

        try {
            Thread.sleep(args.Runtime);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

//        recorder.Stop();
//        System.out.println("Recording stopped.");
        recorder.stopCapture();

        try {
            Thread.sleep(args.Delay);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

//        recorder.StopPlayback();
//        System.out.println("Playback stopped.");
        recorder.stopPlayback();
    }

    private static void window() {
        CapturePlayback capturePlayback = new CapturePlayback();
        JFrame f = new JFrame("Capture/Playback");
        f.addWindowListener(new WindowAdapter() {
            public void windowClosing(WindowEvent e) { System.exit(0); }
        });
        f.getContentPane().add("Center", capturePlayback);
        f.pack();
        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        int w = 720;
        int h = 340;
        f.setLocation(screenSize.width/2 - w/2, screenSize.height/2 - h/2);
        f.setSize(w, h);
        f.show();
    }
}