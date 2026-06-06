using System;
using System.Collections.ObjectModel;
using System.IO;
using Contacts.Model;
using Newtonsoft.Json;

namespace Contacts.Model.Services
{
    /// <summary>
    /// Выполняет сохранение и загрузку объекта <see cref="Contact"/> в формате JSON.
    /// </summary>
    public static class ContactsSerializer
    {
        private static readonly string DocumentsPath = Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments
        );
        private static readonly string AppFolder = Path.Combine(DocumentsPath, "Contacts");
        private static readonly string FilePath = Path.Combine(AppFolder, "contacts.json");

        static ContactsSerializer()
        {
            // Создаем папку, если её нет
            if (!Directory.Exists(AppFolder))
            {
                Directory.CreateDirectory(AppFolder);
            }
        }

        public static void SaveContacts(ObservableCollection<Contact> contacts)
        {
            try
            {
                var json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        public static ObservableCollection<Contact> LoadContacts()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    var contacts = JsonConvert.DeserializeObject<ObservableCollection<Contact>>(
                        json
                    );
                    return contacts ?? new ObservableCollection<Contact>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
            }

            return new ObservableCollection<Contact>();
        }
    }
}
