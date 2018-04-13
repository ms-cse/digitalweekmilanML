This repo contains the code for running the OCR PDF pipeline in Azure functions.
We defined three Azure functions in one function app and they work in this order:
1. **PdfToPng** - reads a PDF once uploaded in blob storage 'invoices' container, converts it to PNG and outputs it in 'images' container.
2. **PngToOcr** - reads a PNG once created in the 'images' container, puts it through Cognitive Services OCR and creates a json with the result in 'json' container.
3. **OcrToFlat** - reads a JSON once created in the 'json' container, flattens it and outputs it in in the 'flatjson' container.

Together these three functions will run in the correct order once a PDF is uploaded in the 'invoices' container.

## Requirements ##
The **PdfToPng** function depends on *Ghostscript*, which needs to be copied to the machine the function runs on.
Download the [GhostScript 32 bit assembly](https://ghostscript.com/download/gsdnld.html). Install it and locate the *gsdll32.dlll* on your local machine (on Windows this would be in C:\Program Files (x86)\gs\gsX.XX\bin\).
Alternatively you can take the dll from this repo.

Open the Kudu console in the **PdfToPng** function, go to the Debug Console (CMD) and navigate to this folder:
*D:\home\data\Functions\packages\nuget\ghostscript.net\1.X.X\lib\net40* and upload the dll there.
You should have three files in that folder:
-  Ghostscript.NET.dll
-  Ghostscfript.NET.xml
-  gsdll32.dll (the file you just uploaded).

