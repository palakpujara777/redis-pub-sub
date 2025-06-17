using CallProcess.Application.Interfaces.CallProcessing;
using CacheManagement.Interface;
using CallProcess.Domain.Common;

namespace CallProcess.Processing
{
    public class ProcessFile : IProcessFile
    {
        #region Private Properties

        private readonly ICallManager _callManager;
        
        private readonly ICacheService _cacheService;

        private const string CallingNumberKeyPrefix = "CallingNumber:";

        #endregion

        public ProcessFile(ICallManager callManager, ICacheService cacheService)
        {
            _callManager = callManager;
            _cacheService = cacheService;
        }

        #region Public Methods

        public async Task Init()
        {
            Console.WriteLine("Console Ready. Type 'Process <filename>' to start processing.");

            try
            {
                while (true)
                {
                    var input = Console.ReadLine();

                    if (!string.IsNullOrEmpty(input) && input.StartsWith("Process", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = input.Split(' ', 2);
                        if (parts.Length == 2)
                        {
                            var filename = parts[1].Trim();
                            if (File.Exists(filename))
                            {
                                await Process(filename);
                            }
                            else
                            {
                                Console.WriteLine("File not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading console");
            }
        }

        #endregion

        #region Private Methods

        private async Task Process(string filePath)
        {
            try
            {
                /***** Read all lines from file *****/
                var lines = await File.ReadAllLinesAsync(filePath);

                /***** Check whether file is empty or not *****/
                if (lines.Length == 0)
                {
                    Console.WriteLine("File is empty.");
                    return;
                }

                /***** Check if required columns present in header *****/
                var header = lines[0].Split(',');
                int codeIndex = Array.FindIndex(header, h => h.Equals("Code", StringComparison.OrdinalIgnoreCase));
                int numberIndex = Array.FindIndex(header, h => h.Equals("CallingNumber", StringComparison.OrdinalIgnoreCase));

                if (codeIndex == -1 || numberIndex == -1)
                {
                    Console.WriteLine("Required columns not found (Code, CallingNumber).");
                    return;
                }

                var outputLines = new List<string> { string.Join(",", header.Append("Country")) };
                var duplicateLines = new List<string> { string.Join(",", header) };

                /***** Process lines *****/
                for (int i = 1; i < lines.Length; i++)
                {
                    /***** If code or calling numbers are not present in line then skip *****/
                    var values = lines[i].Split(',');
                    if (values.Length <= Math.Max(codeIndex, numberIndex))
                    {
                        continue;
                    }

                    string code = values[codeIndex];
                    string callingNumber = values[numberIndex];
                    string callingNumberKey = $"{CallingNumberKeyPrefix}{callingNumber}";

                    /***** Check if calling number already exists in cache *****/
                    if (await _cacheService.ExistsAsync(callingNumberKey))
                    {
                        duplicateLines.Add(lines[i]);
                        continue;
                    }

                    var country = _callManager.GetByCode(code) ?? "Unknown";

                    /***** Save this calling number in Redis to avoid future duplicates with duplicate retention period *****/
                    await _cacheService.SetAsync<string>(callingNumberKey, callingNumber, TimeSpan.FromMinutes(CacheHelper.DuplicateRetentionPeriod));

                    outputLines.Add($"{string.Join(",", values)},{country}");
                }

                string outputPath = Path.Combine(Path.GetDirectoryName(filePath), "Processed_" + Path.GetFileName(filePath));
                string duplicatePath = Path.Combine(Path.GetDirectoryName(filePath), "Duplicates_" + Path.GetFileName(filePath));

                await File.WriteAllLinesAsync(outputPath, outputLines);
                await File.WriteAllLinesAsync(duplicatePath, duplicateLines);

                Console.WriteLine($"Processing completed.\nSaved to: {outputPath}\nDuplicates saved to: {duplicatePath}");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error processing file " + ex.ToString());
            }
        }

        #endregion
    }
}