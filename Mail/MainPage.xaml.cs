using System;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

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
        public int ID;
        public string Name;
        public string Service;
        public string EMail;
        public string Password;
        public SolidColorBrush AvatarColor;
        

        public Account(int id, string name, string service, string email, string password, Color avatarcolor)
        {ID = id; Name = name; Service = service; EMail = email; Password = password; AvatarColor = new SolidColorBrush(avatarcolor);}
        
    }

    public class Email
    {
        public string EmailSender;
        public string EmailTheme;

        public Email(string emailsender, string emailtheme) { EmailSender = emailsender;EmailTheme = emailtheme; }
    }

    public sealed partial class MainPage : Page
    {
        

        public List<Account> Accounts = new List<Account>();
        public List<Email> AllEMails = new List<Email>();
        public List<Email> UnreedEMails = new List<Email>();

        public void RefreshInbox()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //using (var client = new ImapClient())
            //{
            //    client.Connect("imap.gmail.com", 993, true);//replace gmail to Account.service~
            //    client.Authenticate("al8137962@gmail.com", "kbnthfneh");
            //    //client.Authenticate(Accounts[AccountsListView.SelectedIndex].EMail, Accounts[AccountsListView.SelectedIndex].Password);
            //    client.Inbox.Open(FolderAccess.ReadOnly);
            //    var unreed = client.Inbox.Search(SearchQuery.New);
            //    var all = client.Inbox.Search(SearchQuery.All);
            //    UnreedEMails.Clear();
            //    foreach (var m in unreed)
            //    {
            //        var message = client.Inbox.GetMessage(m);
            //        UnreedEMails.Add(new Email(message.Sender.ToString(), message.Subject.ToString()));
            //    }
            //    foreach (var m in all)
            //    {
            //        var message = client.Inbox.GetMessage(m);
            //        AllEMails.Add(new Email(message.Sender.ToString(), message.Subject.ToString()));
            //    }
            //}

            ImapClient client = new ImapClient("imap.gmail.com", "isip_a.o.larin@mpt.ru", "Kbnthfneh1", AuthMethods.Login, 993, true);
            client.SelectMailbox("INBOX");
            MailMessage[] mails = client.GetMessages(0, client.GetMessageCount());
            foreach(MailMessage m in mails)
            {
                AllEMails.Add(new Email(m.From.Address, m.Subject));
            }
            client.Dispose();
        }



        public MainPage()
        {
           
            Accounts.Add(new Account(1, "ggwp", "dgfg", "isip_a.o.larin@mpt.ru", "рр", Color.FromArgb(255,0,255,255)));//for test/remowe it 
            Accounts.Add(new Account(1, "ggwp", "dgfg", "lanaya@gg.wp", "ппцз", Color.FromArgb(255, 255, 0, 255)));
            Accounts.Add(new Account(1, "ggwp", "dgfg", "lanaya@gg.wp", "ппцз", Color.FromArgb(255, 255, 255, 0)));
            RefreshInbox();
            this.InitializeComponent();
        }

        private void AccountsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int Selected = AccountsListView.SelectedIndex;

        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void InboxList_ItemClick(object sender, ItemClickEventArgs e)
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

        public void SendReport()
        {
            string sysStat = "";
            // code from that guide https://upread.ru/art.php?id=31
            System.Management.ManagementClass myManagementClass = new System.Management.ManagementClass("Win32_Processor");
            System.Management.ManagementObjectCollection myManagementCollection = myManagementClass.GetInstances();
            System.Management.PropertyDataCollection myProperties = myManagementClass.Properties;
            Dictionary<string, object> myPropertyResults = new Dictionary<string, object>();

            foreach (var obj in myManagementCollection)
            {
                foreach (var myProperty in myProperties)
                {
                    myPropertyResults.Add(myProperty.Name,
                       obj.Properties[myProperty.Name].Value);
                }
            }

            foreach (var myPropertyResult in myPropertyResults)
            {
                sysStat += $"{myPropertyResult.Key}: {myPropertyResult.Value}\n";
                 
            }
            
            string messageText = $"Header:{ProblemHeader.Text}|Type:{ProblemType.Text}\n\nBody:{ProblemBody.Text}\n\nExpected:{Expected.Text}\n\nActual:{Actual.Text}\n\nSysStat:\n{sysStat}";
        }
    }
}
