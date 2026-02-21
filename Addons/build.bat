rmdir /s /q build-android

cmake -B build-android ^
  -G Ninja ^
  -DCMAKE_TOOLCHAIN_FILE=C:\Users\johan\AppData\Local\Android\Sdk\ndk\29.0.14206865\build\cmake\android.toolchain.cmake ^
  -DANDROID_ABI=arm64-v8a ^
  -DANDROID_PLATFORM=android-24 ^
  -DCMAKE_BUILD_TYPE=Release ^
  -DLLAMA_BUILD_TESTS=OFF ^
  -DLLAMA_BUILD_EXAMPLES=OFF ^
  -DLLAMA_BUILD_SERVER=OFF ^
  -DLLAMA_BUILD_TOOLS=OFF ^
  -DBUILD_SHARED_LIBS=ON ^
  -DGGML_OPENMP=OFF
  
cmake --build build-android

pause