using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace messageSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {



            InitializeComponent();

            txtSubject.IsEnabled = false;

            

        }


        Message message = new Message();
        bool anEmail = false;
        bool aTweet = false;
        bool aSMS = false;
        //Store words depending if hashtags or mentions
        List<string> hashtagsList = new List<string>();
        List<string> urlList = new List<string>();
        List<string> mentionsList = new List<string>();
        List<string> SIRemailList = new List<string>();
        //Used to display the results for the trends list
        List<string> displayHashtag = new List<string>();
        List<string> displayMention = new List<string>();
       





        private void btnSerialize_Click(object sender, RoutedEventArgs e)
        {
            message.type = string.Empty;
            Random rnd = new Random();
            int number = rnd.Next(111111111, 999999999);          
            message.header = txtHeader.Text;
            message.body = txtBody.Text;
            message.subject = txtSubject.Text;

            //checking if details are correct

            if (string.IsNullOrWhiteSpace(txtHeader.Text) || (string.IsNullOrWhiteSpace(txtBody.Text)))
            {
                MessageBox.Show("Missing details");

            }
            else
            {


                if (!aTweet && !anEmail && !aSMS)
                {

                    MessageBox.Show("Incorrect Message Type");
                }

                //

                if (aTweet == true)
                {
                    txtType.Text = "T" + number;


                    string hashtag = txtBody.Text;
                    var regex = new Regex(@"(?<=#)\w+");
                    var matches = regex.Matches(hashtag);


                    foreach (Match m in matches)
                    {
                        hashtagsList.Add("#" + m.Value);

                        


                    }


                    string textAT = txtBody.Text;
                    var regex1 = new Regex(@"(?<=@)\w+");
                    var matches1 = regex1.Matches(textAT);


                    foreach (Match m in matches1)
                    {

                        mentionsList.Add("@" + m.Value);


                    }

                }

                if (aSMS == true)
                {
                    txtType.Text = "S" + number;


                }

                if (anEmail == true)
                {

                    txtType.Text = "E" + number;


                    string input = txtBody.Text;
                    string pattern = @"((file||http|ftp|https|)://)+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,15})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?";
                    string replacement = "<url quarantined>";
                    Regex rgx = new Regex(pattern);

                    var matches = rgx.Matches(input);

                    //adding matches to a list

                    foreach (Match m in matches)
                    {
                        urlList.Add(m.Value);

                       

                    }

                    string result = rgx.Replace(input, replacement);

                    //changing body text

                    txtBody.Text = result;
                    message.body = result;


                    string separator = "\n";
                    //setting values to message class to later use in json serialize
                    message.quarantine = string.Join(separator, urlList);





                    try
                    {

                        //Check for SIR+DATE IN SUBJECT

                        string SIRdate = txtSubject.Text;
                     
                        var regex3 = new Regex(@"\SIR \d{2}[/]\d{2}[/]\d{2}");
                        var matches3 = regex3.Matches(SIRdate);
                        if (regex3.IsMatch(SIRdate))
                        {

                           
                       
                        foreach (Match m in matches3)
                        {
                            SIRemailList.Add(m.Value);
                           

                        }




                        //SIR CODE IN TEXTBODY

                        string code = txtBody.GetLineText(0);
                      
                        var regex2 = new Regex(@"\b\d{2}[-]\d{3}[-]\d{2}");
                        var matches2 = regex2.Matches(code);

                        foreach (Match mx in matches2)
                        {
                            
                            SIRemailList.Add(mx.Value);
                            


                        }


                        //  FIND A WORD IN THE FIRST LINE FOR SIR

                        string stringToCheck;
                        stringToCheck = txtBody.GetLineText(1);
                        string[] stringArray = { "Theft of Properties", "Staff Attack", "Device Damage", "Raid", "Customer Attack", "Staff Abuse", "Bomb Threat", "Terrorism", "Suspicious Incident", "Sport Injury", "Personal Info Leak" };


                        if (stringArray.Any(stringToCheck.Contains))
                            {

                            SIRemailList.Add(stringToCheck);
                           


                            }
                        }

                    }
                    catch
                    {

                     
                    }


                }

                if (aSMS == true || aTweet == true) {

                    //Adding to dictionary

                    Dictionary<string, string> abreviations = new Dictionary<string, string>();
                    

                    using (StreamReader sr = new StreamReader("textwords.csv"))
                    {
                        string line = string.Empty;

                        while ((line = sr.ReadLine()) != null)
                        {

                            line = line.Replace("\"", "");
                            string[] test = line.Split(',');
                            string Key = test[0];
                            string Value = test[1];
                            abreviations.Add(Key, Value);
                        }



                    }
                    // Adding extension

                    foreach (KeyValuePair<string, string> kvp in abreviations)
                    {
                        string check = (@"(^|\s)" + kvp.Key + @"(\s|$)");
                        if(Regex.IsMatch(txtBody.Text, check))
                        {
                            //insert the translation
                            int indexOfKey = txtBody.Text.IndexOf(kvp.Key) + kvp.Key.Length;
                            txtBody.Text = txtBody.Text.Insert(indexOfKey, " <" + kvp.Value.ToString() + ">");
                        }
                    }


                }

                    //creating json file (SERIALIZE)

                    message.type = txtType.Text;

                    string Json = JsonConvert.SerializeObject(message, Formatting.Indented);
                    string fileName = txtType.Text + ".Json";
                    using (StreamWriter str = File.CreateText(@".\\" + fileName + ""))

                    {
                        str.Write(Json);
                    }
                




            }


        }



        private void txtHeader_TextChanged(object sender, TextChangedEventArgs e)
        {

            //Checking if its an email

            if (txtHeader.Text.Length > 0)
            {
                try
                {

                    var test = new MailAddress(txtHeader.Text);
                    anEmail = true;
                }
                catch (FormatException ex)
                {

                    anEmail = false;

                }
            }
            //Setting values of textboxes if its an email
            if (anEmail == true)
            {
                txtSubject.IsEnabled = true;
                txtBody.MaxLength = 1028;
                txtSubject.MaxLength = 20;
                labelChars.Content = "1028 Characters limit";
                txtBody.Width = 324;


            }
            //if its not an email then subject remains disabled
            else
            {
                txtSubject.IsEnabled = false;

            }
            //Checking if its a tweet
            try
            {
                if (txtHeader.Text.Substring(0, 1).Equals("@") && txtHeader.Text.Length <= 15)
                {
                    aTweet = true;

                }
            }

            catch
            {
                aTweet = false;

            }

            //Setting values if tweet is true

            if (aTweet == true)
            {

                txtBody.MaxLength = 140;
                labelChars.Content = "140 Characters limit";
                txtSubject.IsEnabled = false;
                txtBody.Width = 214;

            }
            //Checking if its an SMS (REGEX TO CHECK FOR + AND 1-15 digits (max length for international numbs is 15 digits)

            Regex NumericRegex = new Regex(@"\+\d{11,15}");
            if (NumericRegex.IsMatch(txtHeader.Text))
            {

                aSMS = true;


            }
            else
            {
                aSMS = false;
            }



            //Setting values for SMS
            if (aSMS == true)
            {
                txtBody.MaxLength = 140;
                labelChars.Content = "140 Characters limit";
                txtBody.Width = 214;

            }

            if (!aSMS && !aTweet && !anEmail)
            {
                labelChars.Content = "Characters Available";
                txtBody.Width = 214;

            }






        }

        private void btnDeserialize_Click(object sender, RoutedEventArgs e)
        {
           
            //loading files data into txtbox

            string LoadFile = txtFileName.Text;

            try
            {
                using (StreamReader r = new StreamReader(LoadFile))
                {
                    string json = r.ReadToEnd();

                    dynamic array = JsonConvert.DeserializeObject<Message>(json);
                    txtBody.Text = array.body;
                    txtHeader.Text = array.header;
                    txtSubject.Text = array.subject;
                    txtType.Text = array.type;
                    urlList.Add(array.quarantine);
                    MessageBox.Show(array.type + " loaded");

                }

            }
            catch
            {
                MessageBox.Show("Incorrect file name, Type file name with extension, eg test.json");

            }




        }






        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

            //Clearing all the user input values
            
            txtHeader.Text = "";
            txtBody.Text = "";
            txtSubject.IsEnabled = false;
            txtSubject.Text = "";
            txtType.Text = "";
            labelChars.Content = "Characters Available";
            aTweet = false;
            anEmail = false;
            aSMS = false;
            txtBody.Width = 214;
            
            
     


        }


        

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            window2 win2 = new window2();

            //Hashtag trend list

            var groups = hashtagsList.GroupBy(v => v);
            foreach (var group in groups)
                displayHashtag.Add(group.Key + " was used " + group.Count() + " times.");

            //Mention trend list

            var groups2 = mentionsList.GroupBy(v => v);
            foreach (var group in groups2)
                displayMention.Add(group.Key + " was used " + group.Count() + " times.");

            //SETTING DISPLAY OF THE ARRAY WITH THE HASHTAG AND MENTION RESULTS
            string separator = "\n";
            win2.displayHashtag.Text = string.Join(separator, displayHashtag);
            win2.displayMentions.Text = string.Join(separator, displayMention);
            win2.Show();





        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".json";
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                txtFileName.Text = filename;

            }
        }

        //display quarantined form
        private void btnQuarantined_Click(object sender, RoutedEventArgs e)
        {
            WindowEmail winEmail = new WindowEmail();
            string separator = "\n";
            winEmail.txtEmail.Text = string.Join(separator, urlList);
            winEmail.Show();
        }

        //display SIR form
        private void btnSIR_Click(object sender, RoutedEventArgs e)
        {
            SIRemails sirEmail = new SIRemails();
            string separator = "\n";
            sirEmail.txtSIR.Text = string.Join(separator, SIRemailList);
            sirEmail.Show();



        }
    }
}


