using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Discere.Service
{
    public class AIService
    {
        public async Task Init()
        {

        }

        public async Task<string> EvaluateAsync(string question, string correct, string user)
        {
            var prompt = $@"
<|begin_of_text|>
<|start_header_id|>system<|end_header_id|>
Du bist ein strenger, objektiver Lehrer.
Du bewertest anhand der Musterlösung.
Keine zusätzlichen Erklärungen.
Kein Fließtext außerhalb des geforderten Formats.
<|eot_id|>

<|start_header_id|>user<|end_header_id|>
Frage:
{question}

Musterlösung:
{correct}

Antwort des Schülers:
{user}

Aufgabe:
Vergleiche die Schülerantwort mit der Musterlösung.
Bewerte fachliche Richtigkeit und Vollständigkeit.

Antworte exakt in diesem Format:
Kommentar: <maximal ein kurzer Satz>

Keine weiteren Zeilen.
<|eot_id|>

<|start_header_id|>assistant<|end_header_id|>";
            return await Ask(prompt);
        }

        public async Task<string> Ask(string prompt)
        {
            string path = "";
#if ANDROID
    path = await AndroidFilePermissions.CopyModelToAppDataAsync();
    if (path == null)
    {
        Console.WriteLine("Cannot access or copy model file!");
        return "";
    }
#endif

            var init = NativeMethods.llama_android_init(path);
            if (init != 0)
            {
                return "";
            }

            var sb = new StringBuilder(8192);

            NativeMethods.llama_android_infer(
                prompt,
                sb,
                sb.Capacity);

            NativeMethods.llama_android_free();

            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            //    resultLabel.Text = sb.ToString();
            //    System.Diagnostics.Debug.WriteLine(resultLabel.Text);
            //});
            return sb.ToString();
        }
    }
}
