using System.Runtime.InteropServices;
using System.Text;

internal static class NativeMethods
{
    [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
    public static extern int llama_android_init(string path);

    [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
    public static extern int llama_android_infer(
        string prompt,
        StringBuilder output,
        int outSize);

    [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
    public static extern void llama_android_free();
}