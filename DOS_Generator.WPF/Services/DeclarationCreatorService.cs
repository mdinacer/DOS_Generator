using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.ViewModels.Template;
using Microsoft.Extensions.DependencyInjection;
using OpenXmlPowerTools;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;

namespace DOS_Generator.WPF.Services
{
    public static class DeclarationCreatorService
    {
        #region Properties

        private const string EmptySpace = "…………………………………………";

        #endregion

        #region Constructors

        #endregion

        #region Functions

        public static async Task<List<string>> GenerateDeclarations(List<Declaration> declarations)
        {
            if (declarations == null || !declarations.Any()) return null;

            var paths = new List<string>();

            foreach (var declaration in declarations)
            {
                var path = await GenerateDeclaration(declaration);
                paths.Add(path);
            }

            return paths;
        }

        public static async Task<string> GenerateDeclaration(Declaration declaration)
        {
            if (declaration == null) return null;

            //Create a word file
            var outputFile = CreateDocument(declaration.Ship.Name);


            // Replace placeholders content and populate the table
            await UpdateDeclarationContents(outputFile, declaration);

            return outputFile;
        }

        private static async Task UpdateDeclarationContents(string outputFile, Declaration declaration)
        {
            using (var document = WordprocessingDocument.Open(outputFile, true))
            {
                string docText;
                using (var streamReader = new StreamReader(document.MainDocumentPart.GetStream(FileMode.Open)))
                {
                    docText = await streamReader.ReadToEndAsync();
                    streamReader.Dispose();
                }

                var fields = GenerateFields(declaration);

                foreach (var (key, value) in fields) docText = new Regex(key).Replace(docText, value);

                await using (var streamWriter =
                    new StreamWriter(document.MainDocumentPart.GetStream(FileMode.Create, FileAccess.Write)))
                {
                    await streamWriter.WriteAsync(docText);
                }

                #region Stamp

                if (!string.IsNullOrEmpty(declaration.Officer.TemplatePath) &&
                    File.Exists(declaration.Officer.TemplatePath))
                {
                    var sourceFile = declaration.Officer.TemplatePath;
                    var tempFile = $".\\Resources\\stamp{Path.GetExtension(sourceFile)}";
                    EncryptionService.DecryptFile(App.User.Name, sourceFile, tempFile);
                    await ReplaceImage(document, tempFile, "image1.png");
                }

                #endregion

                #region Table

                var table = document.MainDocumentPart.Document.Body.Elements<Table>().ElementAt(2);
                var rowsCount = table.Elements<TableRow>().Count();


                var templateView = App.ServiceProvider.GetRequiredService<DeclarationTemplateViewModel>();

                var entries = templateView.GetEntries(); // create a boolean array from the Declaration entries
                for (var i = 2; i < rowsCount - 1; i++)
                {
                    var row = table.Elements<TableRow>().ElementAt(i);
                    var cell = row.Elements<TableCell>().ElementAt(1);
                    var paragraph = cell.Elements<Paragraph>().First();
                    var element = entries.ElementAt(i - 2);

                    var run = paragraph.Elements<Run>().First();
                    var textElement = run.Elements<Text>().First();
                    textElement.Text = element ? declaration.Officer?.Initials ?? "Yes" : "N/A";
                }

                #endregion
            }
        }

        /// <summary>
        ///     Copy the template file to the "Generated" directory
        /// </summary>
        private static string CreateDocument(string shipName)
        {
            var templateFile = string.IsNullOrWhiteSpace(App.Settings?.TemplatePath)
                ? ".\\Resources\\template.docx"
                : App.Settings?.TemplatePath;
            var generatedDosPath = App.Settings?.GeneratedDeclarationsPath ?? ".\\Declarations\\";

            if (!File.Exists(templateFile) || string.IsNullOrEmpty(shipName))
                return null; // Checks if the Template file exists and that the declaration is not null

            var path = $"{generatedDosPath}{DateTime.Today:dd.MM.yyyy}\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path); // Create the generated declarations folder if not found

            var fileName = $"{path}{shipName}_dos.docx";

            File.Copy(templateFile, fileName, true);

            return fileName;
        }


        /// <summary>
        ///     Generate a dictionary of Placeholders and correspondent values
        /// </summary>
        private static Dictionary<string, string> GenerateFields(Declaration declaration)
        {
            if (declaration == null) return null;
            var currentConfig = App.Settings;
            return new Dictionary<string, string>
            {
                {"PortNameField", $"{declaration.Port?.Name}"},
                {
                    "PortFacilityNameField",
                    $"{declaration.Port?.Name ?? EmptySpace}{" - " + declaration.Facility?.Name}"
                },
                {"ShipNameField", declaration.Ship?.Name ?? EmptySpace},
                {"PortOfRegistryField", declaration.Ship?.PortOfRegistry ?? EmptySpace},
                {"ImoNumberField", declaration.Ship?.ImoNumber ?? EmptySpace},
                {"StartDateField", $"{declaration.StartDate:dd/MM/yy}"},
                {"EndDateField", declaration.EndDate == null ? "DEPARTURE" : $"{declaration.EndDate:dd/MM/yy}"},
                {"OperationField", declaration.Operation == 0 ? "LOADING" : "DISCHARGING"},
                {"SecurityLevelField", declaration.SecLevel},
                {"SignatureTimeField", DateTime.Now.ToString("HH:mm")},
                {"SignatureDateField", DateTime.Today.ToString("dd/MM/yyyy")},
                {"OfficerNameField", declaration.Officer.FullName},
                {"OfficerTitleField", declaration.Officer.Title},
                {"PFSONameField", currentConfig.SecurityOfficerName},
                {"PhoneField", currentConfig.Phone},
                {"FaxField", currentConfig.Fax},
                {"EmailField", currentConfig.Email},
                {"RadioField", currentConfig.Radio}
            };
        }

        private static async Task ReplaceImage(WordprocessingDocument document, string source, string destination)
        {
            if(!File.Exists(source)) return;

            var sourceImage = await File.ReadAllBytesAsync(source);
            var outputImage = document.MainDocumentPart.ImageParts // or EndsWith
                .First(p => p.Uri.ToString().Contains(destination));

            if(outputImage == null) return;

            await using (var writer = new BinaryWriter(outputImage.GetStream()))
            {
                writer.Write(sourceImage);
            }
            File.Delete(source);
        }

        public static void ConvertWordToHtml(string input, string output)
        {
            if (!File.Exists(input)) return;

            using var doc = WordprocessingDocument.Open(input, true);
            var convSettings = new HtmlConverterSettings
            {
                FabricateCssClasses = false,
                RestrictToSupportedLanguages = false,
                RestrictToSupportedNumberingFormats = false
            };

            var html = HtmlConverter.ConvertToHtml(doc, convSettings);

            using var fs = new FileStream(output, FileMode.Create);
            using var w = new StreamWriter(fs, Encoding.UTF8);
            w.WriteLine(html);
        }

        //public static string ConvertWordToHtml(string input)
        //{
        //    if (!File.Exists(input)) return null;

        //    using var doc = WordprocessingDocument.Open(input, true);
        //    var convSettings = new HtmlConverterSettings
        //    {
        //        FabricateCssClasses = false,
        //        RestrictToSupportedLanguages = false,
        //        RestrictToSupportedNumberingFormats = false
        //    };

        //    var html = HtmlConverter.ConvertToHtml(doc, convSettings);

        //    using var fs = new FileStream(output, FileMode.Create);
        //    using var w = new StreamWriter(fs, Encoding.UTF8);
        //    w.WriteLine(html);
        //}

        #endregion

        #region Events

        #endregion

        #region Commands

        #endregion
    }
}