using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Ping_test.Domain
{
    public class PingTest
    {
        public int Duration { get; private set; }

        private static PingTest uniqueTest = new PingTest();
        private CancellationTokenSource _stopWorkingCts;
        public int ProgressTicksInSec { get; private set; }
        public int ProgressTickDelay { get; private set; }

        public int PingTimeout { get; private set; }
        public int DelayBewteenPings { get; private set; }

        public double Progres { get; set; }

        private PingTest() {
            Duration = 7;
            ProgressTicksInSec = 10;
            PingTimeout = 5000;
            ProgressTickDelay = 100;
            DelayBewteenPings = 300;
        }

        public static PingTest getInstace()
        {
            return uniqueTest;
        }

        public async Task<string> RunAsync(IProgress<double> pgBarProgress, IProgress<string> textBlockProgress, string pingAddress)
        {
            _stopWorkingCts = new CancellationTokenSource();
            string result = "";
            var msDuration = Duration * 1000;

            try
            {
                _stopWorkingCts.CancelAfter(msDuration);

                Task pingTask = Task.Run(() => PingContinousAsync(textBlockProgress, _stopWorkingCts.Token, pingAddress));
                Task timerTask = Task.Run(() => UpdatePgBar(pgBarProgress));
                await timerTask;
                await pingTask;

            }
            catch (OperationCanceledException b)
            {
                result += "\n Test anulowany";
            }
            catch (PingException p)
            {
                result += "\n Zły adres hosta";
            }
            finally
            {
                result += "\n Test zakończony";
                _stopWorkingCts.Dispose();
                _stopWorkingCts = null;
            }

            return result;
        }

        private async Task PingContinousAsync(IProgress<string> progress, CancellationToken cancelToken, string pingAddress)
        {
            Ping ping = new Ping();
            PingReply pingReply;
            var result = "";

            while (!cancelToken.IsCancellationRequested)
            {
                    pingReply = await ping.SendPingAsync(pingAddress, PingTimeout);
                    if (pingReply.Status == IPStatus.Success)
                    {
                        result = $"PING status: Success IP:{pingReply.Address.ToString()} time = {pingReply.RoundtripTime.ToString()}ms";
                    }
                    else
                    {
                        result = "Upłynął limit czasu żadania";
                    }

                    progress.Report(result);

                if (!cancelToken.IsCancellationRequested)
                     Thread.Sleep(DelayBewteenPings);                   
            }
        }

        private void UpdatePgBar(IProgress<double> progress)
        {
            var ticks = Duration * ProgressTicksInSec;

            for (int i = 1; i <= ticks; i++)
            {
               // progress.Report(i);
                Progres = i;
                Thread.Sleep(ProgressTickDelay);
            }
        }

    }
}
