using DllHotInjector;
using System.Diagnostics;

namespace Core
{
    public class AutoInjector
    {
        private Thread threadInstance;
        private long sleepNeedAppend = 0;
        private bool lastTimeOuted = false;
        private volatile bool shouldRun = true;
        private volatile Process detectedProcess = null;

        public void initAndStartLoop() {
            threadInstance = new Thread(new ThreadStart(this.doLoop));
            threadInstance.Start();
        }

        private void doLoop() {
            long sysTime = 0;
            while (this.shouldRun) {
                sysTime = System.DateTime.Now.Millisecond;

                try
                {
                    this.doTickOnce();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    long timeEscaped = DateTime.Now.Millisecond - sysTime;
                    long shouldSleep = this.lastTimeOuted ? 50L - timeEscaped + this.sleepNeedAppend : 50L - timeEscaped;

                    if (shouldSleep > 0)
                    {
                        Thread.Sleep((int)shouldSleep);

                        this.sleepNeedAppend = 0L;
                        this.lastTimeOuted = false;
                    }
                    else
                    {
                        this.sleepNeedAppend = shouldSleep;
                        this.lastTimeOuted = true;
                    }
                }
            }
        }

        private void onClientDetected(Process target) {
            InjectUtil.hotInjectDllTo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "JVMHook.dll", target.Id);
        }

        private void doTickOnce() {
            Process neteaseProcess = NeteaseClientScanner.GetNeteaseClientProcess();
            if (neteaseProcess != null && this.detectedProcess == null) 
            {
                Console.WriteLine("Netease MC Client detected! PID:" + neteaseProcess.Id);
                this.onClientDetected(neteaseProcess);
                this.detectedProcess = neteaseProcess;
                new Thread(new ThreadStart(this.waitForProcess)).Start();
            }
        }

        private void waitForProcess() {
            if (this.detectedProcess != null) 
            {
                this.detectedProcess.WaitForExit();
                Console.WriteLine("Process exited");
                this.detectedProcess = null;
                this.shouldRun = false;
            }
        }
    }
}
