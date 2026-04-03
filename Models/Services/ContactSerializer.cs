using System;
using System.IO;
using Contacts.Model;
using Newtonsoft.Json;

namespace Contacts.Model.Services
{
    /// <summary>
    /// Выполняет сохранение и загрузку объекта <see cref="Contact"/> в формате JSON.
    /// </summary>
    public class ContactSerializer
    {
        /// <summary>
        /// Получает путь к файлу, в котором хранятся данные контакта.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ContactSerializer"/>.
        /// </summary>
        /// <param name="filePath">
        /// Пользовательский путь к файлу сохранения.
        /// Если значение не указано, используется файл <c>contacts.json</c> в папке Documents/Contacts.
        /// </param>
        public ContactSerializer(string? filePath = null)
        {
            var defaultDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Contacts"
            );

            FilePath = filePath ?? Path.Combine(defaultDir, "contacts.json");
        }

        /// <summary>
        /// Сохраняет данные контакта в JSON-файл.
        /// </summary>
        /// <param name="contact">Контакт, который необходимо сохранить.</param>
        public void Save(Contact contact)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            var json = JsonConvert.SerializeObject(contact, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        /// <summary>
        /// Загружает контакт из файла хранения.
        /// </summary>
        /// <returns>
        /// Экземпляр <see cref="Contact"/>, восстановленный из файла.
        /// Если файл отсутствует или не удалось десериализовать данные, возвращается пустой контакт.
        /// </returns>
        public Contact Load()
        {
            if (!File.Exists(FilePath))
                return new Contact("", "", "");

            var json = File.ReadAllText(FilePath);
            var contact = JsonConvert.DeserializeObject<Contact>(json);

            return contact ?? new Contact("", "", "");
        }
    }
}
