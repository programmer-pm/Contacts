namespace Contacts.Model
{
    /// <summary>
    /// Представляет контакт с основными данными пользователя.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Получает или задаёт имя контакта.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Получает или задаёт номер телефона контакта.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Получает или задаёт адрес электронной почты контакта.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Инициализирует новый пустой экземпляр класса <see cref="Contact"/>.
        /// </summary>
        public Contact()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Email = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/> с указанными данными.
        /// </summary>
        /// <param name="name">Имя контакта.</param>
        /// <param name="phoneNumber">Номер телефона контакта.</param>
        /// <param name="email">Адрес электронной почты контакта.</param>
        public Contact(string name, string phoneNumber, string email)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
