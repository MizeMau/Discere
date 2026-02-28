#if ANDROID
using Microsoft.Maui.Storage;

public static class AndroidFilePermissions
{
    public static async Task<string?> CopyModelToAppDataAsync(string modelFileName = "model.gguf")
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

    public static async Task<FileResult?> PickDatabaseFileAsync()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select SQLite Database",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".db", ".sqlite", ".sqlite3" } },
            { DevicePlatform.Android, new[] { "application/octet-stream" } },
            { DevicePlatform.iOS, new[] { "public.database" } }
        })
        });

        return result;
    }
    public static async Task<string?> ImportDatabaseAsync()
    {
        var result = await PickDatabaseFileAsync();
        if (result == null)
            return null;

        var destinationPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "database.db");

        using var sourceStream = await result.OpenReadAsync();
        using var destinationStream = File.Create(destinationPath);

        await sourceStream.CopyToAsync(destinationStream);

        Preferences.Set("DatabasePath", destinationPath);

        return destinationPath;
    }
}
#endif