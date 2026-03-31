using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AiClient;
using System.Threading;
using DotNetEnv;

namespace WpfChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeminiMetadataService _metadataService;
        private IChatService? _chatService;
        string apiKeyFromEnv;
        public MainWindow()
        {
            InitializeComponent();
            // load stored API key from secret store (if any)
          //  SecretStore.SaveApiKey("AIzaSyCVCdynohJIxTAKs6P0zhAYHrbGGmZ2w9o");
            var stored = Env.TraversePath().Load();
            apiKeyFromEnv =  Environment.GetEnvironmentVariable("GeminiAPIKey"); 
            if (!string.IsNullOrEmpty(apiKeyFromEnv))
            {
                ApiKeyTextBox.Text = apiKeyFromEnv;
                _metadataService = new GeminiMetadataService(apiKey: apiKeyFromEnv);
            }
            else
            {
                _metadataService = new GeminiMetadataService(apiKey: "");
            }
        }

        private async void LoadModelsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadModelsButton.IsEnabled = false;
            try
            {
                var apiKey = ApiKeyTextBox.Text?.Trim();
                if (string.IsNullOrEmpty(apiKey))
                {
                    MessageBox.Show("Enter API key first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                //_metadataService = new GeminiMetadataService(apiKey);
                await _metadataService.LoadModelsAsync();

                // Save api key to user secrets file for future runs
                try
                {
                   SecretStore.SaveApiKey(apiKey);
                }
                catch
                {
                    // ignore save failures
                }

                ModelsComboBox.ItemsSource = _metadataService.AvailableModels;
                if (_metadataService.AvailableModels.Count > 0)
                    ModelsComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed loading models: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadModelsButton.IsEnabled = true;
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync();
        }

        private async void InputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await SendMessageAsync();
            }
        }

        private async Task SendMessageAsync()
        {
            var text = InputTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            MessagesListBox.Items.Add("User: " + text);
            InputTextBox.Clear();

            var apiKey = ApiKeyTextBox.Text?.Trim();
            var model = ModelsComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(model))
            {
                MessageBox.Show("Please provide API key and select model.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _chatService = ChatServiceFactory.CreateService("Gemini", apiKey, model);

            var history = new List<ChatMessage> { new ChatMessage(ChatRole.User, text) };
            MessagesListBox.Items.Add("AI: ...");
            try
            {
                var response = await _chatService.GetResponseAsync(history);
                // replace last AI placeholder
                MessagesListBox.Items[MessagesListBox.Items.Count - 1] = "AI: " + response;
            }
            catch (Exception ex)
            {
                MessagesListBox.Items[MessagesListBox.Items.Count - 1] = "AI: (error) " + ex.Message;
            }
        }

        // בתוך ה-Partial Class של ה-Form שלך
        private List<ChatMessage> _chatHistory = new List<ChatMessage>();

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string userInput = InputTextBox.Text;
            if (string.IsNullOrWhiteSpace(userInput)) return;

            // 1. הוספת הודעת המשתמש להיסטוריה
            ChatMessage userMessage = new ChatMessage(ChatRole.User, userInput);
            _chatHistory.Add(userMessage);

            // 2. עדכון ה-UI (הצגת ההודעה של המשתמש)
            MessagesListBox.Items.Add("אתה: " + userInput + Environment.NewLine);
            InputTextBox.Clear();

            // 3. שליחת כל ההיסטוריה ל-Provider שנוצר ב-Factory
            // הערה: ה-service נוצר מראש לפי המודל שנבחר ב-ComboBox
            string aiResponse = await _chatService.GetResponseAsync(_chatHistory);

            // 4. הוספת תשובת ה-AI להיסטוריה (חשוב מאוד! כדי שהוא יזכור מה הוא ענה)
            ChatMessage aiMessage = new ChatMessage(ChatRole.Assistant, aiResponse);
            _chatHistory.Add(aiMessage);

            // 5. עדכון ה-UI (הצגת תשובת ה-AI)
            MessagesListBox.Items.Add("AI: " + aiResponse + Environment.NewLine + Environment.NewLine);
        }
    }
}