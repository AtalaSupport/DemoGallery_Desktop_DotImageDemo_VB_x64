# DotImage Demo
The most comprehensive of all the demos demonstrating most of the image processing 
commands and codecs.  This is a good place to learn about the UI features that 
dotImage offers, as well as testing of image effects, and transforms.

This is the VB.NET version. A [C# version](https://github.com/AtalaSupport/DemoGallery_Desktop_DotImageDemo_CS_x64.git) is also available.


## Licensing
This application requires a license for DotImage Document Imaging. If you wish to have PDF support, you can add our PdfReader addon licensing. You may also request a 30 day evaulation if youre evaluating if DotImage / our OCR is right for you.


## SDK Dependencies
This app was built based on 2026.2.0.0. It targets .NET Framework 4.6.2 and was created in Visual Studio 2022. You must have our SDK installed (and licesed per above)

[Download DotImage](https://www.atalasoft.com/BeginDownload/DotImageDownloadPage)


### Using NuGet for SDK Dependencies
We do publish our SDK components to NuGet. We have chosen to base the demo on local installed SDK because this leads to much smaller applications (NuGet packages add a lot of overhead due to the way they're packaged and deployed, and many of our demos -- including this one -- are often used to reproduce issues that need to be submitted to support. Apps that use NuGet are often significantly larger and run up against our maximum support case upload size)

Still, if you wish to use NuGet for the dependencies instead of relying on locally installed SDK, you can.

- Take note of each of the references we've included:
    - Atalasoft.DotImage.dll
    - Atalasoft.DotImage.AdvancedDocClean.dll
    - Atalasoft.DotImage.CommonDecoders.dll
    - Atalasoft.DotImage.Dicom.dll
    - Atalasoft.DotImage.Dwg.dll
    - Atalasoft.DotImage.Heif.dll
    - Atalasoft.DotImage.Jbig2.dll
    - Atalasoft.DotImage.Jpeg2000.dll
    - Atalasoft.DotImage.Lib.dll
    - Atalasoft.DotImage.Office.dll
    - Atalasoft.DotImage.Pdf.dll
    - Atalasoft.DotImage.PdfDoc.Bridge.dll
    - Atalasoft.DotImage.PdfReader.dll
    - Atalasoft.DotImage.Raw.dll
    - Atalasoft.DotImage.WinControls.dll
    - Atalasoft.PdfDoc.dll
    - Atalasoft.Shared.dll
- Remove those referneces
- Open the NuGet Package Manger from `Tools -> NuGet Package Manager -> Manage NuGet Packages for this Solution`
- Browse for and install  Atalasoft.DotImage.WinControls.x64 - It will pull in DotImage Document Imaging (the base SDK) and our windows controls and shared dll
- Browse for and install Atalasoft.Dwg.x64. (optional if you wish to have support for DWG and other CADD files)
- Browse for and install Atalasoft.Dicom.x64. (optional if you wish to have support for Dicom files)
- Browse for and install Atalasoft.Heif.x64. (optional if you wish to have support for HEIF/HEIC files)
- Browse for and install Atalasoft.Jbig2.x64. (optional if you wish to have support for Jbig2 files)
- Browse for and install Atalasoft.Jpeg2000.x64. (optional if you wish to have support for Jpeg2000 files)
- Browse for and install Atalasoft.Pdf.x64  to bring in the PdfEncoder
- Browse for and install Atalasoft.PdfReader.x64. (optional if you wish to have support for PDF files)
- Browse for and install Atalasoft.Raw.x64. (optional if you wish to have support for RAW files)


## Downloading source
The sources can be downloaded for [c#](https://github.com/AtalaSupport/DemoGallery_Desktop_DotImageDemo_CS_x64/archive/refs/heads/main.zip) and [VB.NET](https://github.com/AtalaSupport/DemoGallery_Desktop_DotImageDemo_VB_x64/archive/refs/heads/main.zip)


## Cloning
We recommend the following to ensure you clone with the required submodule

Example: git for windows
```bash
git clone https://github.com/AtalaSupport/DemoGallery_Desktop_DotImageDemo_VB_x64.git DotImageDemo --recursive
cd DotImageDemo
git submodule init
git submodule update
```


## Related documentation
In addition to this README, the Atalasoft documentation set includes the following:  
- [AtalaSupport Github](https://github.com/AtalaSupport/) For an extensive set of sample apps.  
- [Atalasoft's APIs & Developer Guides page](https://www.atalasoft.com/Support/APIs-Dev-Guides) for our Developers guide and API references.  
- [Atalasoft Support](http://www.atalasoft.com/support/) for our main support portal.
- [Atalasoft Knowledgebase](http://www.atalasoft.com/kb2) where you can find answers to common questions / issues.  


## Getting Help for Atalasoft products
Atalasoft regularly updates our support [Knowledgebase](http://www.atalasoft.com/kb2) with the latest information about our products. To access some resources, you must have a valid Support Agreement with an authorized Atalasoft Reseller/Partner or with Atalasoft directly. Use the tools that Atalasoft provides for researching and identifying issues. 

Customers with an active evaluation, or those with active support / maintenance may [create a support case](https://www.atalasoft.com/Support/my-portal/Cases/Create-Case) 24/7, or call in to support ([+1 949 236-6510](tel:19492366510) ) during our normal support hours (Monday - Friday 8:00am to 5:00PM Eastern (New York) time).  

Customers who are unable to create a case or call in may [email our Sales Team](email:sales@atalasoft.com).  



