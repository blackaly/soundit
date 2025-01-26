using System; 
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace Soundit;

public class SoundIt{
    private const string PATTERN = @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})";
    private string Youtube; 
    private string URL; 
    private Dictionary<string, dynamic> Payload;

    public SoundIt(string url)
    {
        this.Payload = new Dictionary<string, dynamic>();
        this.URL = url;
        this.Youtube = @"https://www.youtube.com/youtubei/v1/player?key=AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";
    }

    private async Task<JObject> MakeRequest(string id){
        var jsonSer = JsonConvert.SerializeObject(this.Payload);
        var data = new StringContent(jsonSer, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var response =  await client.PostAsync(this.Youtube, data);
        var reader = await response.Content.ReadAsByteArrayAsync();
        var result = Encoding.UTF8.GetString(reader, 0, reader.Length);
        JObject content = JObject.Parse(result);
        return content;

    }

    public async Task<string[]> Execute(){
        string[] elements = new string[2];

        if(string.IsNullOrEmpty(URL)) return elements;

        string id = isCorrectPattern(URL);
        if(string.IsNullOrEmpty(id)) return elements;
        AddPayload(id);

        string audio = string.Empty;
        string videoName = string.Empty;
        var content = await MakeRequest(id);
        var res = content["streamingData"]["adaptiveFormats"].ToList();
        foreach(var r in res){
            if(r["audioQuality"] != null){
                string temp = r["audioQuality"].ToString();
                if(temp.Contains("AUDIO_QUALITY_MEDIUM")){
                    elements[0] = r["url"].ToString();
                    elements[1] = content["videoDetails"]["title"].ToString();
                    break;
                }
            }
        }
        return elements;
    }

    private string isCorrectPattern(string s){
        Match m = Regex.Match(s, PATTERN);

        if(m.Success) return m.Groups[1].Value; 
        return string.Empty;  
    }

    private void AddPayload(string id){
        this.Payload.Add("videoId", id);
        var context = new Dictionary<string, Dictionary<string, dynamic>>(){
            {"client", new Dictionary<string, dynamic>(){
                {"clientName", "IOS"},
                {"clientVersion", "19.45.4"},
                {"androidSdkVersion", 30},
                {"deviceMake", "Apple"},
                {"deviceModel", "iPhone16,2"},
                {"platform", "MOBILE"},
                {"osName", "IOS"},
                {"osVersion", "18.1.0.22B83"},
                {"visitorData", "Cgtsb0JmaEZ4R2JUOCidntq8BjIKCgJFRxIEGgAgDDoMCAEg1P2wwdTjo8tn"}

            }}
        };
        this.Payload.Add("context", context);
        
    }
}