using System; 
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.IO;
using Soundit;
using System.Net;

class Program{
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
            using(var client = new WebClient()){
            client.DownloadProgressChanged += DownloadProgressCallback;
            await client.DownloadFileTaskAsync(data[0], PATH);
            
            }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }
        Console.WriteLine(PATH);
    } 

    static void DEBUG(string s){Console.WriteLine(s);}  
    static void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
    {
        Console.Write($"\rDownloading... {e.ProgressPercentage}% [{new string('#', (int)(e.ProgressPercentage / 10))}{new string('-', (int)(10 - e.ProgressPercentage / 10))}]");
    }

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
}