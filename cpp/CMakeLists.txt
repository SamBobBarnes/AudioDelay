cmake_minimum_required(VERSION 3.30)
project(cpp)

set(CMAKE_PREFIX_PATH /Users/sbarnes/.vcpkg-clion/vcpkg/installed/arm64-osx CACHE STRING "")

find_package(portaudio CONFIG REQUIRED)

set(CMAKE_CXX_STANDARD 20)

add_executable(cpp main.cpp)

target_link_libraries(cpp PRIVATE portaudio_static)