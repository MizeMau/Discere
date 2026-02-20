using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static Discere.Service.LlamaNative;

namespace Discere.Service
{
    public class AIService
    {
        IntPtr model;
        IntPtr context;

        int n_ctx = 512;

        public async Task Init()
        {
            //var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            //if (status != PermissionStatus.Granted)
            //    status = await Permissions.RequestAsync<Permissions.StorageRead>();

            //if (status != PermissionStatus.Granted)
            //{
            //    Console.WriteLine("Cannot access external storage!");
            //    return;
            //}


            ////string modelPath =
            ////    Path.Combine(
            ////        FileSystem.Current.AppDataDirectory,
            ////        "tiny-model.gguf");

            //string modelPath = "/storage/emulated/0/Discere/tiny-model.gguf";


            //if (!File.Exists(modelPath))
            //    return;

            //string dest = Path.Combine(FileSystem.AppDataDirectory, "tiny-model.gguf");
            //if (!File.Exists(dest))
            //{

            //    using var src = File.OpenRead("/storage/emulated/0/Discere/tiny-model.gguf");
            //    using var dst = File.Create(dest);
            //    src.CopyTo(dst);  // copy entire file
            //}

            //await Task.Run(async () =>
            //{
            //    if (!File.Exists(modelPath))
            //    {
            //        using var stream = await FileSystem.OpenAppPackageFileAsync("tiny-model.gguf");
            //        using var file = File.Create(modelPath);

            //        byte[] buffer = new byte[81920]; // 80 KB
            //        int bytesRead;
            //        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            //        {
            //            await file.WriteAsync(buffer, 0, bytesRead);
            //        }
            //    }
            //});
            string modelPath = "";
#if ANDROID
            modelPath = await AndroidFilePermissions.CopyModelToAppDataAsync();
            if (modelPath == null)
            {
                Console.WriteLine("Cannot access or copy model file!");
                return;
            }
#endif

            try
            {
                LlamaNative.llama_backend_init(false);

                var modelParams = LlamaNative.llama_model_default_params();
                modelParams.use_mmap = false;      // important on Android
                modelParams.n_gpu_layers = 0;      // CPU only

                var model = LlamaNative.llama_load_model_from_file(modelPath, modelParams);

                if (model == IntPtr.Zero)
                    throw new Exception("Model failed to load.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }


            var model_params =
                new LlamaNative.llama_model_params();

            model =
                LlamaNative.llama_load_model_from_file(
                    modelPath,
                    model_params);


            var ctx_params = new LlamaNative.llama_context_params();

            ctx_params.n_ctx = n_ctx;
            ctx_params.n_threads = Environment.ProcessorCount;

            context = LlamaNative.llama_new_context_with_model(model, ctx_params);

        }

        public string EvaluateAsync(string question, string correct, string user)
        {
            var prompt = $@"
Du bist ein strenger Lehrer.

Frage:
{question}

Musterlösung:
{correct}

Antwort des Schülers:
{user}

Bewerte kurz.

Format:
Score: <0-100>
Kommentar: <kurzer Satz>";
            return Ask(prompt);
        }

        public string Ask(string prompt)
        {

            var tokens = new int[n_ctx];

            llama_batch batch = LlamaNative.llama_batch_init(n_tokens, 0, 1);


            StringBuilder output =
                new StringBuilder();


            for (int i = 0; i < 128; i++)
            {

                int next = SampleToken();

                if (next == 2)
                    break;


                string piece =
                    Marshal.PtrToStringAnsi(
                        LlamaNative.llama_token_to_piece(
                            model,
                            next));

                output.Append(piece);


                int[] batch = { next };

                LlamaNative.llama_eval(
                    context,
                    batch,
                    1,
                    tokenCount + i);

            }


            return output.ToString();

        }


        int SampleToken()
        {

            IntPtr logitsPtr =
                LlamaNative.llama_get_logits(context);

            int vocab =
                LlamaNative.llama_n_vocab(model);


            float max = float.MinValue;
            int maxToken = 0;


            unsafe
            {

                float* logits =
                    (float*)logitsPtr;

                for (int i = 0; i < vocab; i++)
                {

                    if (logits[i] > max)
                    {

                        max = logits[i];
                        maxToken = i;

                    }

                }

            }


            return maxToken;

        }
    }
}
