using Apex.MVVM;
using Ping_test.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_test.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public Command StartTest { get; private set; }
        private bool canTypeAddress;
        public bool CanTypeAddress
        {
            get { return canTypeAddress; }
            set
            {
                if (canTypeAddress == value) return;
                canTypeAddress = value;
                RaisePropertyChanged();
            }
        }

        private bool canSubmitAddress;
        public bool CanSubmitAddress
        {
            get { return canSubmitAddress; }
            set
            {
                if (canSubmitAddress == value) return;
                canSubmitAddress = value;
                RaisePropertyChanged();
            }
        }

        public string ButtonContent { get; private set; }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                if (address == value) return;
                address = value;
                RaisePropertyChanged();
            }
        }

        private string output;
        public string Output
        {
            get { return output; }
            set
            {
                if (output == value) return;
                output = value;
                RaisePropertyChanged();
            }
        }

        private double progress;
        public double Progress
        {
            get { return progress; }
            set
            {
                if (progress == value) return;
                progress = value;
                RaisePropertyChanged();
            }
        }

        private int progressMax;
        public int ProgressMax
        {
            get { return progressMax; }
            set
            {
                if (progressMax == value) return;
                progressMax = value;
                RaisePropertyChanged();
            }
        }

        private readonly PingTest _test;
        public MainWindowViewModel()
        {
            StartTest = new Command(RunTest);
            _test = PingTest.getInstace();

            PrepareUI();
            RefreshUI();

            ProgressMax = _test.Duration * _test.ProgressTicksInSec;
            ButtonContent = $"Wysyłaj zapytania PING przez {_test.Duration} sek.";
        }

        private async void RunTest()
        {
            PrepareUI();

            var textBlockProgress = new Progress<string>(s => Output += "\n" + s);
            var pgBarProgress = new Progress<double>(s => Progress = s);

            var result = await _test.RunAsync(pgBarProgress, textBlockProgress, Address);
            Output += result;

            RefreshUI();
        }
        private void PrepareUI()
        {
            Output = "Odpowiedzi PING:";
            CanTypeAddress = false;
            CanSubmitAddress = false;
            Progress = 0;
        }

        private void RefreshUI()
        {
            CanTypeAddress = true;
            CanSubmitAddress = true;
        }
    }
}
