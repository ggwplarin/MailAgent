using System;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.Mail;
using System.Net;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AE.Net.Mail;


// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Mail
{


    public class Account
    {

        public string Name;
        public string Service;
        public string EMail;
        public string Password;
        public SolidColorBrush AvatarColor;


        public Account(string name, string service, string email, string password, Color avatarcolor)
        { Name = name; Service = service; EMail = email; Password = password; AvatarColor = new SolidColorBrush(avatarcolor); }

    }


    public class Email
    {
        public string EmailSender;
        public string EmailTheme;
        public string EmailBody;
        public Email(string emailsender, string emailtheme, string emailBody) { EmailSender = emailsender; EmailTheme = emailtheme; EmailBody = emailBody; }
    }

    public sealed partial class MainPage : Page
    {

        bool sortDirection = true;

        ObservableCollection<Account> Accounts = new ObservableCollection<Account>();

        ObservableCollection<Email> AllEMails = new ObservableCollection<Email>();

        public AE.Net.Mail.MailMessage[] GetMailMessagesAsync(string service, string email, string password)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            

            ImapClient client = new ImapClient(service, email, password, AuthMethods.Login, 993, true);

            client.SelectMailbox("INBOX");

            AE.Net.Mail.MailMessage[] mails = client.GetMessages(0, client.GetMessageCount(), false);
            
            client.Dispose();
            return mails;
        }
        //TODO: сделать нормально
        public async void RefreshInbox(string service, string email, string password)
        {
            gg.IsActive = true;
             AE.Net.Mail.MailMessage[] mails = await Task.Run(() => GetMailMessagesAsync(service, email, password));
           
            foreach (AE.Net.Mail.MailMessage m in mails)
            {

                AllEMails.Add(new Email(m.From.Address, m.Subject, m.Body));
                
            }
            
               AllEMails = new ObservableCollection<Email>(AllEMails.Reverse());
                InboxList.ItemsSource = AllEMails;
            
            
            gg.IsActive = false;

        }



        public MainPage()
        {
            Accounts.Add(new Account("ggwp", "gmail.com", "isip_a.o.larin@mpt.ru", "Kbnthfneh1", Color.FromArgb(255, 0, 255, 255)));//for test/remowe it


            this.InitializeComponent();
        }

        private void AccountsListView_ItemClick(object sender, ItemClickEventArgs e)
        {


        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }



        private void InboxList_Loaded(object sender, RoutedEventArgs e)
        {

        }



        private void Report_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void Report_Loaded(object sender, RoutedEventArgs e)
        {
            ReportTip.IsOpen = true;
        }

        public void SendReport(string messageSubject, string messageText)
        {



            // отправитель - устанавливаем адрес и отображаемое в письме имя
            System.Net.Mail.MailAddress from = new MailAddress("report.agent.mail@gmail.com", "ReportAgent");
            // кому отправляем
            System.Net.Mail.MailAddress to = new MailAddress("isip_a.o.larin@mpt.ru");
            // создаем объект сообщения
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(from, to);
            // тема письма
            m.Subject = messageSubject;
            // текст письма
            m.Body = messageText;
            // письмо представляет код html
            m.IsBodyHtml = false;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("report.agent.mail@gmail.com", "#reporter1");

            smtp.EnableSsl = true;
            smtp.Send(m);

        }
        public bool IsNotEmptyStr(string inputStr)
        {
            return inputStr.Replace("\n", "").Replace(" ", "").Length > 0;
        }
        private async void SendReportBtn_Click(object sender, RoutedEventArgs e)
        {
            ReportBtnNoConnection.IsOpen = false;

            ReportBtnWarn.IsOpen = false;
            if (IsNotEmptyStr(ProblemHeader.Text) && IsNotEmptyStr(ProblemBody.Text) && IsNotEmptyStr(Expected.Text) && IsNotEmptyStr(Actual.Text) && ProblemType.SelectedIndex != -1)
            {
                string messageSubject = $"{ProblemHeader.Text}|{ProblemType.SelectedIndex}";
                string messageText = $"Header:{ProblemHeader.Text}|Type:{ProblemType.SelectedIndex}\n\nBody:{ProblemBody.Text}\n\nExpected:{Expected.Text}\n\nActual:{Actual.Text}";
                try
                {
                    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                    System.Net.NetworkInformation.PingReply pingReply = ping.Send("www.google.com");
                    if (pingReply.Status.ToString() != "Success") { ReportBtnNoConnection.IsOpen = true; }
                    else
                    {

                        await Task.Run(() => SendReport(messageSubject, messageText));
                        ProblemHeader.Text = "";
                        ProblemBody.Text = "";
                        Expected.Text = "";
                        Expected.Text = "";
                        ProblemType.SelectedIndex = -1;
                    }
                }
                catch
                {
                    ReportBtnNoConnection.IsOpen = true;
                }
            }
            else
            {
                ReportBtnWarn.IsOpen = true;
            }
        }

        private void AddNewAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }



        private void AcceptAddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                ImapClient client = new ImapClient("imap." + EmailServiceBox.Text, EmailAdressBox.Text, EmailPasswordBox.Password, AuthMethods.Login, 993, true);
                client.Dispose();


                Accounts.Add(new Account(AccountNameBox.Text, EmailServiceBox.Text, EmailAdressBox.Text, EmailPasswordBox.Password, AccountColorPicker.Color));
                AccountNameBox.Text = "";
                EmailAdressBox.Text = "";
                EmailPasswordBox.Password = "";
                EmailServiceBox.Text = "";

                AddNewAccFlyout.Hide();
            }
            catch
            {

            }
        }

        private void RefreshInboxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsListView.SelectedIndex > -1)
            {
                string service = "imap." + Accounts[AccountsListView.SelectedIndex].Service;
                string email = Accounts[AccountsListView.SelectedIndex].EMail;
                string password = Accounts[AccountsListView.SelectedIndex].Password;
                RefreshInbox(service, email, password);
                

            }
        }

        private void InboxList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InboxList.SelectedIndex != -1)
            {
                try
                {

                    MessageHeader.Text = AllEMails[InboxList.SelectedIndex].EmailTheme;
                    MessageFrom.Text = "From: " + AllEMails[InboxList.SelectedIndex].EmailSender;

                    MessageBody.Text = AllEMails[InboxList.SelectedIndex].EmailBody;
                }
                catch
                {

                }
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DirectionBtn_Click(object sender, RoutedEventArgs e)
        {
            

            AllEMails = new ObservableCollection<Email>(AllEMails.Reverse());
            InboxList.ItemsSource = AllEMails;
        }

        private void NewEmailBtn_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void SendEmailBtn_Click(object sender, RoutedEventArgs e)
        {

        }







        //private bool Filter(Contact contact)
        //{
        //    return contact.FirstName.Contains(FilterByFirstName.Text, StringComparison.InvariantCultureIgnoreCase) &&
        //            contact.LastName.Contains(FilterByLastName.Text, StringComparison.InvariantCultureIgnoreCase) &&
        //            contact.Company.Contains(FilterByCompany.Text, StringComparison.InvariantCultureIgnoreCase);
        //}



    }
}
