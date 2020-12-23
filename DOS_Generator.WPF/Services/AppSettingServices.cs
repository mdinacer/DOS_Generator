using System.IO;
using System.Xml.Serialization;
using DOS_Generator.WPF.Domain;

namespace DOS_Generator.WPF.Services
{
    public static class AppSettingsServices
    {
        #region Properties

        #endregion

        #region Constructors

        #endregion

        #region Functions
        /// <summary>
        ///  Persist the setting on the XML file
        /// </summary>
        public static void WriteSettings(AppSettings settings)
        {
            if (settings == null) return;

            var writer = new XmlSerializer(settings.GetType());
            const string path = ".\\AppSettings.xml";
            using var file = new StreamWriter(path);
            writer.Serialize(file, settings);
            file.Close();
        }

        /// <summary>
        ///  Read the XML file and return the settings object
        /// </summary>
        public static AppSettings ReadSettings()
        {
            if (!File.Exists(".\\AppSettings.xml")) return null;
            const string path = ".\\AppSettings.xml";
            var reader = new XmlSerializer(typeof(AppSettings));

            using var file = new StreamReader(path);
            var setting = (AppSettings) reader.Deserialize(file);
            file.Close();
            return setting;
        }

        #endregion

        #region Events

        #endregion

        #region Commands

        #endregion
    }
}