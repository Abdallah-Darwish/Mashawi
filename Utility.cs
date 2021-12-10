namespace Mashawi;
public static class Utility
{
    public static MemoryStream DecodeBase64(string base64)
    {
        MemoryStream res = new();
        res.Write(Convert.FromBase64String(base64));
        res.Flush();
        res.Position = 0;
        return res;
    }

    public static async Task<MemoryStream> DecodeBase64Async(string base64)
    {
        MemoryStream res = new();
        await res.WriteAsync(Convert.FromBase64String(base64)).ConfigureAwait(false);
        await res.FlushAsync().ConfigureAwait(false);
        res.Position = 0;
        return res;
    }

    public static bool IsBase64String(ReadOnlySpan<char> text)
    {
        if (text.Length == 0 || text.Length % 4 != 0) { return false; }
        var index = text.Length - 1;
        if (text[index] == '=') { index--; }
        if (text[index] == '=') { index--; }
        for (var i = 0; i <= index; i++)
        {
            if (IsInvalidBase64(text[i])) { return false; }
        }
        return true;
    }
    private static bool IsInvalidBase64(char value)
    {
        if (value >= 'a' && value <= 'z') { return false; }
        if (value >= 'A' && value <= 'Z') { return false; }
        if (value >= '0' && value <= '9') { return false; }
        return value != '/' && value != '+';
    }
}