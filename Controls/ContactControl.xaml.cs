using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Contacts.Model;

namespace Contacts.Controls
{
    /// <summary>
    /// Пользовательский элемент управления для отображения и редактирования
    /// данных контакта (имя, телефон, email).
    /// </summary>
    public partial class ContactControl : UserControl
    {
        public ContactControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Запрещает ввод в поле телефона недопустимых символов.
        /// </summary>
        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(Contact.IsPhoneCharAllowed);
        }

        /// <summary>
        /// Запрещает вставку из буфера обмена текста с недопустимыми символами.
        /// </summary>
        private void PhoneBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(Contact.IsPhoneCharAllowed))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
