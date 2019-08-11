using System;
using System.Windows;
using Ping_test.Domain;

namespace Ping_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //This value change whole Test Duration
        private const int TEST_TIME_IN_SEC = 7;
    
        private readonly Test _test;
        public MainWindow()
        {
            InitializeComponent();

            _test = Test.getInstace();
            _test.SetDuration(TEST_TIME_IN_SEC);

            progressBar.Maximum = TEST_TIME_IN_SEC * _test.ProgressTicksInSec;
            submitButton.Content = $"Wysyłaj zapytania PING przez {TEST_TIME_IN_SEC} sek.";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            PrepareUI();

            string pingAddress = inputTextBox.Text;

            var textBlockProgress = new Progress<string>(s => outputTextBlock.Text += "\n" + s);
            var pgBarProgress = new Progress<double>(s => progressBar.Value = s);

            var result = await _test.RunAsync(pgBarProgress, textBlockProgress, pingAddress);
            outputTextBlock.Text += result;

            RefreshUI();
        }

        private void PrepareUI()
        {
            outputTextBlock.Text = "Odpowiedzi PING:";
            inputTextBox.IsEnabled = false;
            submitButton.IsEnabled = false;
            progressBar.Value = 0;
        }

        private void RefreshUI()
        {
            inputTextBox.IsEnabled = true;
            submitButton.IsEnabled = true;
        }
    }
}
