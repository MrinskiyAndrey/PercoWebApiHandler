
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConfigurationService
{
    public class Config
    {
        
        public string Login { get; set; } = "admin";
        public string Password { get; set; } = "Qaz123()";
        public int NumberOfDaysEvents { get; set; } = 60;
        public string PathToPercoXML { get; set; } = "perco.xml";
        public string PathToNewEmployeesView { get; set; } = @"D:\scripts_from_percoserver\PERCO.SQL";
        public string PathToUsersForAccessTemplate { get; set; } = @"C:\Accesses\";
        public string PathToPhotos { get; set; } = @"\photos_from_percoserver\";

        private static readonly string FileName = "Config.json";

       
        public Config() { }


        public static Config Load()
        {
            // Создаем файл с дефолтными настройками, если его нет
            if (!File.Exists(FileName))
            {
                var newConfig = new Config();              
                newConfig.Save(); 
                return newConfig;
            }

            try
            {
                string json = File.ReadAllText(FileName);
                return JsonSerializer.Deserialize<Config>(json) ?? new Config();
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка чтения файла конфигурации: {ex.Message}");
                return new Config();
            }
        }

        // Метод для сохранения текущих настроек
        public void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(FileName, json);
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка сохранения конфига: {ex.Message}");
            }
        }
    }
}
