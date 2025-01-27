using System;
using System.Text;
using Bufferprotocol;

namespace Visitordata;

static class VisitorData
{
    private static Random random = new();
    private static char[]  visitorSessionIdChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_".ToCharArray();
    private static string visitorSessionIdString = string.Empty;
    // Generate the session id for the user

    public static string Generate()
    {
       
        visitorSessionIdString = GenerateString();
        
        var bytes = new BufferProtocol().AddString(1, visitorSessionIdString).AddNumber(5, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000 - random.Next(600000))
        .AddBytes(6, 
        new BufferProtocol()
        .AddString(1, "EG")
        .AddBytes(2, new BufferProtocol()
        .AddString(2, "")
        .AddNumber(4, random.Next(255) + 1)
        .ToBytesArray()
        ).ToBytesArray());

        return Uri.EscapeDataString(
            Convert.ToBase64String(bytes.ToBytesArray()).Replace('+', '-').Replace('/', '_').TrimEnd('=')
        );
    }
    public static string GenerateString()
    {
        StringBuilder sb = new();
        for (int i = 0; i < 11; i++)
        {
            sb.Append(visitorSessionIdChar[random.Next(visitorSessionIdChar.Length)]);
        }
        return sb.ToString();
    }
}