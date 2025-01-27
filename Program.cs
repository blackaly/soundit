using System; 
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.IO;
using Soundit;
using System.Net;
using Visitordata;

class Program {
    static void Main(string[] args){

        if(args.Length <= 0){
            Console.WriteLine("SoundIt: ");
            Console.WriteLine("soundit \t\t [change] <PATH> \t to change directory");
            return;
        }
        Run(args[0]).GetAwaiter().GetResult();
    }

    static async Task Run(string s){

        SoundIt sound = new SoundIt(s);
        var data = await sound.Execute();
        if(string.IsNullOrEmpty(data[1]) || string.IsNullOrEmpty(data[0])){Console.WriteLine("Invalid input!"); return;}        
        data[1] = RemoveSpecial(data[1]);

        if(string.IsNullOrEmpty(data[1])){
            data[1] = Path.GetRandomFileName();
        }

        string PATH = @"C:\Users\"+ Environment.UserName + @"\Downloads\" + $"{data[1]}" + ".mp3";
        if(File.Exists(PATH)) {
            Console.WriteLine("File is already EXISTS! Do you need to delete it? [Y/N] "); 
            string ok = Console.ReadLine();
            if(!string.IsNullOrEmpty(ok)){
                ok = ok.ToLower();
                if(ok.Equals("y")) File.Delete(PATH);
                else return;
            }else return; 
            
        }
        try{
            using(var client = new HttpClient()){
            await DownloadFileWithProgressAsync(client, data[0], PATH);
        }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }
        Console.WriteLine(PATH);
    } 

    static void DEBUG(string s){Console.WriteLine(s);}  
    static string RemoveSpecial(string str){
        if(string.IsNullOrEmpty(str)) return string.Empty;
        string cmp = "#<$+%>!`*'|{?\"}=/:\\@";
        string res = string.Empty;
        foreach(var s in str){
            if(cmp.Contains(s)) continue;
            res += s; 
        }

        return res;
    }
    static async Task DownloadFileWithProgressAsync(HttpClient httpClient, string url, string filePath)
    {
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
        var downloadedBytes = 0L;
        var buffer = new byte[8192];
        var isMoreToRead = true;

        using var contentStream = await response.Content.ReadAsStreamAsync();
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        while (isMoreToRead)
        {
            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
            if (read == 0)
            {
                isMoreToRead = false;
            }
            else
            {
                await fileStream.WriteAsync(buffer, 0, read);

                downloadedBytes += read;
                ReportProgress(downloadedBytes, totalBytes);
            }
        }
    }

    static void ReportProgress(long downloadedBytes, long totalBytes)
    {
        if (totalBytes <= 0)
        {
            Console.Write($"\rDownloaded {downloadedBytes} bytes...");
        }
        else
        {
            var progressPercentage = (int)((double)downloadedBytes / totalBytes * 100);
            Console.Write($"\rDownloading... {progressPercentage}% [{new string('#', progressPercentage / 10)}{new string('-', 10 - progressPercentage / 10)}]");
        }
    }

}
