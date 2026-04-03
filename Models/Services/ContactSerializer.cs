// Model/Services/ContactSerializer.cs
using System;
using System.IO;
using Contacts.Model;
using Newtonsoft.Json;

namespace Contacts.Model.Services
{
    public class ContactSerializer
    {
        public string FilePath { get; }

        public ContactSerializer(string? filePath = null)
        {
            var defaultDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Contacts"
            );

            FilePath = filePath ?? Path.Combine(defaultDir, "contacts.json");
        }

        public void Save(Contact contact)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            var json = JsonConvert.SerializeObject(contact, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

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
