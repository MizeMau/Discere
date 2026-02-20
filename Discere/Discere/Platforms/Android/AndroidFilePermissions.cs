#if ANDROID
using Microsoft.Maui.Storage;

public static class AndroidFilePermissions
{
    public static async Task<string?> CopyModelToAppDataAsync(string modelFileName = "tiny-model.gguf")
    {
        var destPath = Path.Combine(FileSystem.AppDataDirectory, modelFileName);

        // Already copied
        if (File.Exists(destPath))
            return destPath;

        // Ask user to pick the model file
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select model file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "*/*" } } 
                // If you want stricter filtering:
                // { DevicePlatform.Android, new[] { "application/octet-stream" } }
            })
        });

        if (result == null)
            return null; // user cancelled

        // Optional: validate filename
        if (!result.FileName.Equals(modelFileName, StringComparison.OrdinalIgnoreCase))
            return null;

        // Copy using SAF stream
        await using var sourceStream = await result.OpenReadAsync();
        await using var destinationStream = File.Create(destPath);

        await sourceStream.CopyToAsync(destinationStream);

        return destPath;
    }
}
#endif