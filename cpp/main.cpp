#include <portaudio.h>
#include <iostream>

int main() {
    Pa_Initialize();
    std::cout << "PortAudio initialized successfully!" << std::endl;
    Pa_Terminate();
    return 0;
}
