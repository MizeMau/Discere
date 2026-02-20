using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Discere.Service
{
    public static class LlamaNative
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct llama_model_params
        {
            public int n_gpu_layers;
            public int split_mode;
            public int main_gpu;
            public IntPtr tensor_split;
            public IntPtr progress_callback;
            public IntPtr progress_callback_user_data;
            [MarshalAs(UnmanagedType.I1)] public bool vocab_only;
            [MarshalAs(UnmanagedType.I1)] public bool use_mmap;
            [MarshalAs(UnmanagedType.I1)] public bool use_mlock;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct llama_context_params
        {
            public int seed;
            public int n_ctx;
            public int n_batch;
            public int n_threads;
            public int n_threads_batch;
            [MarshalAs(UnmanagedType.I1)] public bool f16_kv;
            [MarshalAs(UnmanagedType.I1)] public bool logits_all;
            [MarshalAs(UnmanagedType.I1)] public bool vocab_only;
            [MarshalAs(UnmanagedType.I1)] public bool use_mmap;
            public IntPtr progress_callback;
            public IntPtr progress_callback_user_data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct llama_batch
        {
            public int n_tokens;
            public IntPtr token;
            public IntPtr pos;
            public IntPtr seq_id;
            public IntPtr logits;
        }

        [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
        public static extern llama_model_params llama_model_default_params();

        [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_backend_init([MarshalAs(UnmanagedType.I1)] bool numa);

        [DllImport("llama", EntryPoint = "llama_load_model_from_file", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr llama_load_model_from_file([MarshalAs(UnmanagedType.LPUTF8Str)] string path, llama_model_params parameters);

        [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr llama_new_context_with_model(
            IntPtr model,
            llama_context_params parameters);

        [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_tokenize(
            IntPtr model,
            string text,
            int text_len,
            int[] tokens,
            int n_max_tokens,
            bool add_bos,
            bool special);

        [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_decode(IntPtr ctx, ref llama_batch batch);

        [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr llama_get_logits(IntPtr ctx);

        [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_n_vocab(IntPtr model);

        [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr llama_token_to_piece(
            IntPtr model,
            int token);
    }
}
