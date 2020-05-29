using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Messaging;
using SQLite;
using Plugin.Media.Abstractions;

namespace XamarinContacts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactPage : ContentPage
    {
        public ContactPage()
        {
            InitializeComponent();
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Contact contact = (Contact)BindingContext;
            if (!string.IsNullOrEmpty(contact.Name))
            {
                try
                {
                    App.Repository.SaveContact(contact);
                    Navigation.PopAsync();
                }
                catch (SQLiteException ex)
                {
                    // вид сообщения об ошибке:
                    // UNIQUE constraint failed: Contact.Name
                    if (ex.Message.Contains("Contact.Name"))
                    {
                        DisplayAlert("Ошибка!", "Контакт с таким именем уже есть", "Ладно");
                    }
                }
            }
            else
            {
                DisplayAlert("Ошибка!", "Имя контакта не может быть пустым", "Ладно");
            }
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Contact contact = (Contact)BindingContext;
            App.Repository.DeleteContact(contact.Id);
            Navigation.PopAsync();
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void MakePhoneCall(object sender, EventArgs e)
        {
            var dialer = CrossMessaging.Current.PhoneDialer;
            if (dialer.CanMakePhoneCall)
            {
                DisplayAlert("Ошибка!", "Нет доступа к телефонным звонкам.", "Ладно");
                return;
            }
            Contact contact = (Contact)BindingContext;
            dialer.MakePhoneCall(contact.Phone);
        }

        private void SendMessage(object sender, EventArgs e)
        {
            var dialer = CrossMessaging.Current.SmsMessenger;
            if (dialer.CanSendSms)
            {
                DisplayAlert("Ошибка!", "Нет доступа к отправке SMS.", "Ладно");
                return;
            }
            Contact contact = (Contact)BindingContext;
            dialer.SendSms(contact.Phone);
        }

        private void SendEmail(object sender, EventArgs e)
        {
            var dialer = CrossMessaging.Current.EmailMessenger;
            if (dialer.CanSendEmail)
            {
                DisplayAlert("Ошибка!", "Нет доступа к отправке писем.", "Ладно");
                return;
            }
            Contact contact = (Contact)BindingContext;
            dialer.SendEmail(contact.Email, "", $"Привет, {contact.Name}");
        }

        private async void TakePhoto(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Ошибка!", "Отсутствует камера.", "Ладно");
                return;
            }
            string fileName = $"{(BindingContext as Contact).Name}_{DateTime.Now:dd-MM-yy_hh-mm-ss}.jpg";
            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                SaveToAlbum = true,
                Directory = "Photos",
                Name = fileName
            });
            if (file == null)
                return;
            (BindingContext as Contact).ImageFile = file.Path;
        }

        private async void PickPhoto(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                DisplayAlert("Ошибка!", "Нет доступа к галерее.", "Ладно");
                return;
            }
            MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
            (BindingContext as Contact).ImageFile = photo?.Path;
        }

    }
}