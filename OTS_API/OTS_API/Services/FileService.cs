using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;
using OTS_API.DatabaseContext;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using OTS_API.Utilities;
using Microsoft.VisualBasic;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf;
using Xabe.FFmpeg;
using Aspose.Slides;
using Aspose.Words;

namespace OTS_API.Services
{
    public class FileService
    {
        private readonly ILogger<FileService> logger;
        private readonly OTSDbContext dbContext;

        public FileService(ILogger<FileService> logger, OTSDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<Models.File> SavePublicFileAsync(IFormFile file)
        {          
            try
            {
                var fileInfo = new Models.File()
                {
                    Id = 0,
                    Name = file.FileName,
                    Path = Config.publicFilePath + DateTime.Now.ToString("yyyyMMddHHmmss") + CodeGenerator.GetCode(10),
                    Size = file.Length
                };

                using (var stream = System.IO.File.Create(fileInfo.Path))
                {
                    await file.CopyToAsync(stream);
                }
                await dbContext.Files.AddAsync(fileInfo);
                await dbContext.SaveChangesAsync();

                return fileInfo;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Server Failed to Save File");
            }
        }

        public async Task<Models.File> SavePrivateFileAsync(IFormFile file, string desPath)
        {
            try
            {
                var fileInfo = new Models.File()
                {
                    Id = 0,
                    Name = file.FileName,
                    Path = Config.privateFilePath + desPath + DateTime.Now.ToString("yyyyMMddHHmmss") + CodeGenerator.GetCode(5) + Path.GetExtension(file.FileName),
                    Size = file.Length
                };

                using (var stream = System.IO.File.Create(fileInfo.Path))
                {
                    await file.CopyToAsync(stream);
                }
                await dbContext.Files.AddAsync(fileInfo);
                await dbContext.SaveChangesAsync();
                if (file.ContentType.ToLower().Contains("pdf"))
                {
                    _ = this.GeneratePreviewPdf(fileInfo);
                }
                else if (file.ContentType.ToLower().Contains("video"))
                {
                    _ = this.GeneratePreviewVideo(fileInfo);
                }
                else if (fileInfo.Name.ToLower().Contains("pptx"))
                {
                    _ = this.GeneratePreviewPPTX(fileInfo);
                }
                else if (fileInfo.Name.ToLower().Contains("ppt"))
                {
                    _ = this.GeneratePreviewPPT(fileInfo);
                }
                else if (fileInfo.Name.ToLower().Contains("doc"))
                {
                    _ = this.GeneratePreviewWord(fileInfo);
                }

                return fileInfo;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Server Failed to Save File");
            }
        }

        public async Task<Models.File> GetFileAsync(int id)
        {
            try
            {
                var file = await dbContext.Files.FindAsync(id);
                if (file == null)
                {
                    throw new Exception("File Not Found!");
                }
                return file;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
            
        }

        public Task GeneratePreviewPdf(Models.File fileInfo)
        {
            return Task.Run(() =>
            {
                try
                {
                    var pdfDoc = PdfReader.Open(fileInfo.Path, PdfDocumentOpenMode.Import);
                    var previewPdf = new PdfDocument();
                    previewPdf.Version = pdfDoc.Version;
                    var pageCount = (int)(pdfDoc.PageCount * 0.3 + 1);
                    for (int i = 0; i < pageCount; i++)
                    {
                        previewPdf.AddPage(pdfDoc.Pages[i]);
                    }
                    previewPdf.Save(fileInfo.Path + "(preview)");
                }
                catch (Exception e)
                {
                    logger.LogError("Unable to Create Preview Version of file: " + fileInfo.Name + "\n" + e.Message);
                }
            });
        }

        public async Task GeneratePreviewVideo(Models.File fileInfo)
        {
            try
            {
                FFmpeg.SetExecutablesPath("C:\\Program Files\\FFmpeg\\ffmpeg-n4.3.1-26-gca55240b8c-win64-lgpl-4.3\\bin");

                var destPath = fileInfo.Path + "(preview)";
                var sourceInfo = await FFmpeg.GetMediaInfo(fileInfo.Path);

                var spliter = await FFmpeg.Conversions.FromSnippet.Split(fileInfo.Path, destPath, TimeSpan.Zero, sourceInfo.Duration * 0.3);
                await spliter.Start();
            }
            catch (Exception e)
            {
                logger.LogError("Unable to Create Preview Version of file: " + fileInfo.Name + "\n" + e.Message);
            }
        }

        public Task GeneratePreviewPPTX(Models.File fileInfo)
        {
            return Task.Run(() =>
            {
                try
                {
                    var pptDoc = new Presentation(fileInfo.Path);

                    var pageCount = pptDoc.Slides.Count / 3 + 1;
                    var previewDoc = new Presentation();
                    for (int i = 0; i < pageCount; i++)
                    {
                        previewDoc.Slides.AddClone(pptDoc.Slides[i]);
                    }
                    previewDoc.Save(fileInfo.Path + "(preview)", Aspose.Slides.Export.SaveFormat.Pptx);
                }
                catch (Exception e)
                {
                    logger.LogError("Unable to Create Preview Version of file: " + fileInfo.Name + "\n" + e.Message);
                }
            });
        }

        public Task GeneratePreviewPPT(Models.File fileInfo)
        {
            return Task.Run(() =>
            {
                try
                {
                    var pptDoc = new Presentation(fileInfo.Path);

                    var pageCount = pptDoc.Slides.Count / 3 + 1;
                    var previewDoc = new Presentation();
                    for (int i = 0; i < pageCount; i++)
                    {
                        previewDoc.Slides.AddClone(pptDoc.Slides[i]);
                    }
                    previewDoc.Save(fileInfo.Path + "(preview)", Aspose.Slides.Export.SaveFormat.Ppt);
                }
                catch (Exception e)
                {
                    logger.LogError("Unable to Create Preview Version of file: " + fileInfo.Name + "\n" + e.Message);
                }
            });
        }

        public Task GeneratePreviewWord(Models.File fileInfo)
        {
            return Task.Run(() =>
            {
                try
                {
                    var doc = new Document(fileInfo.Path);
                    var pageCount = doc.PageCount / 3 + 1;
                    var newDoc = doc.ExtractPages(0, pageCount);
                    newDoc.Save(fileInfo.Path + "(preview)");
                }
                catch (Exception e)
                {
                    logger.LogError("Unable to Create Preview Version of file: " + fileInfo.Name + "\n" + e.Message);
                }
            });
        }

        //public Task Test()
        //{
        //    var doc = new Document("D://Videos//source.docx");
        //    var pageCount = doc.PageCount / 3 + 1;
        //    var newDoc = doc.ExtractPages(0, pageCount);
        //    newDoc.Save("D://Videos//output.docx");

        //    return Task.CompletedTask;
        //}
    }
}
