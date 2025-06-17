using Microsoft.Extensions.Hosting;
using CallProcess.Application.Interfaces.CallProcessing;

namespace CallProcess.Processing
{
    public class CallProcessor : BackgroundService
    {
        #region Private Properties

        private readonly IProcessFile _processFile;
        
        private readonly ICallManager _callManager;

        #endregion

        public CallProcessor(IProcessFile processFile, ICallManager callManager)
        {
            _processFile = processFile;
            _callManager = callManager;
        }

        #region Overridden method

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("Starting CallProcessor...");

                /***** Initialize CallManager (load cache to memory) *****/
                await _callManager.Init();

                /***** Start listening to pub-sub updates *****/
                await _callManager.Start();

                /***** Start file processing console listener *****/
                _ = Task.Run(async () => await _processFile.Init(), stoppingToken);
            }
            catch (Exception ex)
            {
                Console.Write("Error starting application :: " + ex.ToString());
            }
        }

        #endregion
    }
}