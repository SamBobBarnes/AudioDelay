cmake_minimum_required(VERSION 3.30)
project(cpp)

set(CMAKE_TOOLCHAIN_FILE ${CMAKE_SOURCE_DIR}/../vcpkg/scripts/buildsystems/vcpkg.cmake CACHE STRING "")

find_package(portaudio CONFIG REQUIRED)
target_link_libraries(main PRIVATE portaudio_static)

set(CMAKE_CXX_STANDARD 20)

add_executable(cpp main.cpp)
