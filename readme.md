# Data cleaning made serverless #

This repo contains the code for running the OCR PDF pipeline in Azure functions.
We defined three Azure functions in one function app and they work in this order:
1. **PdfToPng** - reads a PDF once uploaded in blob storage 'invoices' container, converts it to PNG and outputs it in 'images' container.
2. **PngToOcr** - reads a PNG once created in the 'images' container, puts it through Cognitive Services OCR and creates a json with the result in 'json' container.
3. **OcrToFlat** - reads a JSON once created in the 'json' container, flattens it and outputs it in in the 'flatjson' container.

<p align="center">
<img src="https://drive.google.com/uc?id=18qkU1S1PEiKo1foYQfWpMF2Br5bJl40C" width="800">
</p>

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

## Important License Information ##
As the used GhostScript version is licensed under [AGPL](http://www.gnu.org/licenses/agpl-3.0.html), you need to consider to share your source code in terms of AGPL.
Just for sharpening your awareness, an important piece from the [AGPL](http://www.gnu.org/licenses/agpl-3.0.html) license:

_"If your software can interact with users remotely through a computer network, you should also make sure that it provides a way for users to get its source. For example, if your program is a web application, its interface could display a "Source" link that leads users to an archive of the code. There are many ways you could offer source, and different solutions will be better for different programs; see section 13 for the specific requirements."_

## More info ##
If you want to get to know more about this approach just have a look at the detailed [blog post](https://medium.com/@codeprincess/machine-learning-is-like-washing-clothes-383859f2e94) :)
And if you want to get in contact with the people who commited this crime, ping @codeprincess and @jeroenterheerdt.

