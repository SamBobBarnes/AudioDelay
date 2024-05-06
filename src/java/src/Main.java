import AudioRecorder.CapturePlayback;

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
        CapturePlayback capturePlayback = new CapturePlayback();

        capturePlayback.record();
        System.out.println("recording...");

        //delay 5 seconds
        try {
            Thread.sleep(10000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }


        System.out.println("recording stopped");
        capturePlayback.stopRecording();
        try {
            Thread.sleep(1000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        System.out.println("Playing...");
        capturePlayback.playback();
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