# Discere

This appliction is ment for Students that like to learn with Index Cards.
Okay, I made it for myself to studdy for my final exame.
Getting the local AI working was a huge pain but it workes now.

## Setup
1. Download [Llama.cpp](https://github.com/ggml-org/llama.cpp "Llama.cpp").
2. Put "android-wrapper.cpp" and "CMakeLists.txt" into the src folder
3. Put "build.bat" into the root folder
4. Update the path to ndk in the bat file (if needed also the target ABI)
5. Execute
6. Replace the so files in "Android\libs\{targetABI}\"
7. Set Build Action of the files to AndroidNativeLibrary
8. Download and put the AI model you want to use on your Phone
8.1 FYI I use [Meta-Llama-3.1-8B-Instruct-GGUF](https://huggingface.co/bartowski/Meta-Llama-3.1-8B-Instruct-GGUF/blob/main/Meta-Llama-3.1-8B-Instruct-Q4_K_M.gguf "Meta-Llama-3.1-8B-Instruct-GGUF")
9. Hope it workes