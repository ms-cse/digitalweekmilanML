#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

public static async Task Run (
    Stream image,
    Stream json,
    string name,
    Binder binder, 
    TraceWriter log)
{    
    string apiKey = System.Environment.GetEnvironmentVariable("CS_OCR_API_KEY", EnvironmentVariableTarget.Process);
    string csOCRUrl = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/ocr";

    log.Info("processing file: "+name);

    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
    
    HttpContent content = new StreamContent(image);
    content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

    HttpResponseMessage result = await client.PostAsync(csOCRUrl, content);
    
    var csData = result.Content.ReadAsStringAsync().Result;

    //log.Info("data from image: "+csData);

    if (csData != null) {

        string filename = name.Substring(0, name.LastIndexOf('.'))+".json";
        //log.Info("filename to be written: json/"+filename); 

        var attributes = new Attribute[]
        {
            new BlobAttribute("json/"+filename),
            new StorageAccountAttribute("fd73f4103955dsvm_STORAGE")
        };

        using (var writer = await binder.BindAsync<TextWriter>(attributes))
        {
            writer.Write(csData);
        }

        log.Info("json data should have been written.");
    } else {
        log.Info("no data returned from OCR :(");
    }
    
}