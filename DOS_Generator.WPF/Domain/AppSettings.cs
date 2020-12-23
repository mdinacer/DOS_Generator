namespace DOS_Generator.WPF.Domain
{
    public class AppSettings
    {
        public string SecurityOfficerName { get; set; } = "Undefined";
        public string Phone { get; set; } = "Undefined";
        public string Fax { get; set; } = "Undefined";
        public string Email { get; set; } = "Undefined";
        public string Radio { get; set; } = "Undefined";
        public string GeneratedDeclarationsPath { get; set; } = ".\\Declarations\\"; // Set default path for generated Declarations.
        public string TemplatePath { get; set; } = ".\\Resources\\template.docx"; // Set default path for the template file.
        public bool[] DefaultEntries { get; set; } = new bool[11];
    }
}