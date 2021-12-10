using Microsoft.Extensions.Options;
using SkiaSharp;

namespace Mashawi.Services;
public abstract class BookFileManager
{
    public static string SaveDirectory { get; private set; }

    public static void Init(IServiceProvider sp)
    {
        var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
        SaveDirectory = Path.Combine(appOptions.DataSaveDirectory, "BooksCovers");
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }
    }
    protected virtual string GetEntityFileName(int bookId) => bookId.ToString();
    public string GetBookFilePath(int bookId) => Path.Combine(SaveDirectory, GetEntityFileName(bookId));

    public async Task SaveFile(int bookId, Stream content)
    {
        var filePath = GetBookFilePath(bookId);

        await using var fileStream =
            new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        await content.CopyToAsync(fileStream).ConfigureAwait(false);
        fileStream.SetLength(content.Position);
        await fileStream.FlushAsync().ConfigureAwait(false);
    }
    public async Task SaveBase64File(int bookId, string contentBase64)
    {
        var filePath = GetBookFilePath(bookId);

        await using var fileStream =
            new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        await using var content = await Utility.DecodeBase64Async(contentBase64).ConfigureAwait(false);
        await content.CopyToAsync(fileStream).ConfigureAwait(false);
        fileStream.SetLength(content.Position);
        await fileStream.FlushAsync().ConfigureAwait(false);
    }
    public async Task SaveImage(int bookId, Stream content)
    {
        var filePath = GetBookFilePath(bookId);

        await using var fileStream =
            new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        using var pic = SKImage.FromEncodedData(content);
        using var encodedPic = pic.Encode(SKEncodedImageFormat.Jpeg, 100).AsStream();
        await SaveFile(bookId, encodedPic).ConfigureAwait(false);
    }
    public async Task SaveBase64Image(int bookId, string contentBase64)
    {
        var filePath = GetBookFilePath(bookId);

        await using var fileStream =
            new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        await using var content = await Utility.DecodeBase64Async(contentBase64).ConfigureAwait(false);
        using var pic = SKImage.FromEncodedData(content);
        using var encodedPic = pic.Encode(SKEncodedImageFormat.Jpeg, 100).AsStream();
        await SaveFile(bookId, encodedPic).ConfigureAwait(false);
    }

    public Stream? GetFile(int bookId)
    {
        var filePath = GetBookFilePath(bookId);
        return !File.Exists(filePath)
            ? null
            : new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
    }

    public static bool ValidateImage(Stream imageData)
    {
        using var image = SKImage.FromEncodedData(imageData);
        return image != null;
    }
    public static async Task<bool> ValidateBase64Image(string imageData)
    {
        await using var imageStream = await Utility.DecodeBase64Async(imageData).ConfigureAwait(false);
        return ValidateImage(imageStream);
    }

    public void DeleteFile(int bookId)
    {
        var picPath = GetBookFilePath(bookId);
        if (File.Exists(picPath))
        {
            File.Delete(picPath);
        }
    }
}