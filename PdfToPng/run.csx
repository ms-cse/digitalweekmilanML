#r "Microsoft.WindowsAzure.Storage"
#r "System.Drawing"
#r "System.Web"
#r "System.Configuration"

using System.Drawing;
using System.Drawing.Imaging;

using System;
using System.Text;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

public static async Task Run(
    Stream pdfinput, 
    string name,
    Stream png,
    Binder binder,
    TraceWriter log)
{
    log.Info($"PdfToPng triggered. Processing file: {name}");

    string uploadFileName = name;
    
    // _pdfConversionFilePath is the path the temp folder is in 
    string _pdfConversionFilePath = @"D:\home\data\Functions\sampledata";
    log.Info($"_pdfConversionFilePath: {_pdfConversionFilePath}");
    string filePath = string.Format(@"{0}\{1}", _pdfConversionFilePath, uploadFileName);
    log.Info($"filePath: {filePath}");
    
    //save the input stream to a local file    
    var outputfileStream = File.Create(filePath);
    pdfinput.Seek(0, SeekOrigin.Begin);
    pdfinput.CopyTo(outputfileStream);
    outputfileStream.Close();
        
    // The png file will have the same name as the PDF file but with a png extension
    string fileName = Path.GetFileNameWithoutExtension(filePath);
    log.Info($"fileName: {fileName}");
    string pngFilePath = string.Format(@"{0}\{1}.png", _pdfConversionFilePath, fileName);
    log.Info($"pngFilePath: {pngFilePath}");

    // Delete png file if already exists
    log.Info($"File.Exists(pngFilePath): {File.Exists(pngFilePath)}");
    if (File.Exists(pngFilePath))
    {
        File.Delete(pngFilePath);
    }

    // Use Ghostscript to convert from pdf to png
    //int desired_x_dpi = 96;
    //int desired_y_dpi = 96;
    int desired_x_dpi = 150;
    int desired_y_dpi = 150;

    string inputPdfPath = filePath;
    string outputPath = Path.GetDirectoryName(pngFilePath);

    GhostscriptVersionInfo gvi =
        new GhostscriptVersionInfo(@"D:\home\data\Functions\packages\nuget\ghostscript.net\1.2.1\lib\net40\gsdll32.dll");

    log.Info($"using (GhostscriptRasterizer _rasterizer = new GhostscriptRasterizer())");
    using (GhostscriptRasterizer _rasterizer = new GhostscriptRasterizer())
    {
        log.Info($"_rasterizer.Open: {inputPdfPath}");
        _rasterizer.Open(inputPdfPath, gvi, true);
        //how many pages to rasterize?
        //all pages:
        //int maxPages = _rasterizer.PageCount;
        //for now let's take only the first page
        int maxPages = 1;
        for (int pageNumber = 1; pageNumber <= maxPages; pageNumber++)
        {
            string pageFilePath = Path.Combine(outputPath, fileName + "-Page-" + pageNumber.ToString() + ".png");
            log.Info($"pageFilePath: {pageFilePath}");

            log.Info($"_rasterizer.GetPage: {pageNumber}");
            Image img = _rasterizer.GetPage(desired_x_dpi, desired_y_dpi, pageNumber);
            log.Info($"img.Save: {pageFilePath}");
            img.Save(pageFilePath, ImageFormat.Png);

            string pngfileName = Path.GetFileName(pageFilePath);
            log.Info($"pngfileName: {pngfileName}");

            log.Info($"GetBlockBlobReference");
            
            log.Info($"UploadFromStream: {pageFilePath}");
            using (FileStream fileStream = File.OpenRead(pageFilePath))
            {
                using (MemoryStream ms = new MemoryStream()) {
                    fileStream.CopyTo(ms);
                    
                    var attributes = new Attribute[]
                    {
                        new BlobAttribute("images/"+pngfileName),
                        new StorageAccountAttribute("fd73f4103955dsvm_STORAGE")
                    };

                    using (var writer = await binder.BindAsync<CloudBlobStream>(attributes))
                    {
                        var bytes = ms.GetBuffer();
                        writer.Write(bytes,0,bytes.Length);
                    }
                }
            }

            log.Info($"File.Delete png file: {pageFilePath}");
            File.Delete(pageFilePath);
        }
    }
    
    log.Info($"File.Delete PDF file: {filePath}");
    File.Delete(filePath);

    log.Info($"Completed processing {uploadFileName}");
}