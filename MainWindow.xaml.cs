using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Spectre.Console;

namespace WpfApp52
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          
        }
        const string key = "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";
        const string url = "https://api.openai.com/v1/chat/completions";

        // Initialise the chat by describing the assistant,
        // and providing the assistants first question to the user
        public List<dynamic> messages = new List<dynamic>
{
    new {role = "system",
        content = "You are ChatGPT, a large language " +
                                    "model trained by OpenAI. " +
                                    "Answer as concisely as possible.  " +
                                    "Make a joke every few lines just to spice things up."},
    new {role = "assistant",
        content = "How can I help you?"}
};
        private async void sendBtn_ClickAsync(object sender, RoutedEventArgs e)
        {
            UserMessageUC myMessageUC = new UserMessageUC();
            myMessageUC.userText.Text = textBox.Text;
            myMessageUC.userTime.Content = DateTime.Now.ToShortTimeString();
            listBox.Items.Add(myMessageUC);
            BotMessageUC botMessageUC = new BotMessageUC();
            botMessageUC.botTime.Content = DateTime.Now.ToShortTimeString();
            listBox.Items.Add(botMessageUC);
            textBox.Text = String.Empty;

            // Capture the users messages and add to
            // messages list for submitting to the chat API
            messages.Add(new { role = "user", content = myMessageUC.userText.Text });

            // Create the request for the API sending the
            // latest collection of chat messages
            var request = new
            {
                messages,
                model = "gpt-3.5-turbo",
                max_tokens = 300,
            };

            // Send the request and capture the response
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
            var requestJson = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
            var httpResponseMessage = await httpClient.PostAsync(url, requestContent);
            var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeAnonymousType(jsonString, new
            {
                choices = new[] { new { message = new { role = string.Empty, content = string.Empty } } },
                error = new { message = string.Empty }
            });


            if (!string.IsNullOrEmpty(responseObject?.error?.message))  // Check for errors
            {
                MessageBox.Show(Markup.Escape(responseObject?.error.message));
            }
            else  // Add the message object to the message collection
            {
                var messageObject = responseObject?.choices[0].message;
                messages.Add(messageObject);
                botMessageUC.botMessage.Text = Markup.Escape(messageObject.content);
            }
        }
    }
}
