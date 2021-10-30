using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScanUtil_ForKasperskyInternship
{
    class Program
    {
        // Возможные варианты запуска:
        // scan_util.exe scan %userprofile%\Documents
        // scan_util.exe status 1234

        // При дебаге использую порт 44346 в scan_service.
#if DEBUG
        private const string serverAddress = "https://localhost:44346/ScanTask";
#else
        private const string serverAddress = "https://localhost:5001/ScanTask";
#endif

        /// <summary>
        /// Основной метод. Запускает соответствующие методы в зависимости от параметров командной строки.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                ErrorMessages.PrintIncorrectInput();
                return;
            }
            try
            {
                // Обрабатывается первое слово (scan или status).
                switch (args[0])
                {
                    case "scan":
                        string dirPath = args[1];
                        if (!Directory.Exists(dirPath))
                        {
                            throw new ArgumentException("Directory doesn't exist.");
                        }
                        DirectoryInfo dir = new DirectoryInfo(dirPath);
                        await CreateTaskAsync(dir);
                        break;

                    case "status":
                        int taskNumber = int.Parse(args[1]);
                        await GetTaskStatusAsync(taskNumber);
                        break;

                    default:
                        ErrorMessages.PrintIncorrectInput();
                        break;
                }
            }
            // Возможные ошибки отлавливаются.
            catch (Exception ex)
            {
                if (args[0] == "scan" && (ex is ArgumentNullException || ex is ArgumentException 
                    || ex is System.Security.SecurityException || ex is PathTooLongException))
                {
                    ErrorMessages.PrintIncorrectDirectory();
                }
                else if (args[0] == "status" && (ex is ArgumentNullException 
                    || ex is FormatException || ex is OverflowException))
                {
                    ErrorMessages.PrintIncorrectTaskNumber();
                }
                else
                {
                    Console.WriteLine("Unexpected exception handled.");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Отправляет запрос на создание новой задачи в scan_service.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static async Task CreateTaskAsync(DirectoryInfo dir)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string uri = $"{serverAddress}/create-new-task?path-to-dir={dir.FullName}";
                    var response = await client.GetAsync(uri);
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
                }
            }
            catch (HttpRequestException) { ErrorMessages.PrintServerIsNotRunning(); }
        }

        /// <summary>
        /// Отправляет запрос на получение статуса задачи в scan_service.
        /// </summary>
        /// <param name="taskNumber"></param>
        /// <returns></returns>
        private static async Task GetTaskStatusAsync(int taskNumber)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string uri = $"{serverAddress}/get-task-status?task-index={taskNumber}";
                    var response = await client.GetAsync(uri);
                    var result = await response.Content.ReadAsStringAsync();
                    ParseAndPrintTaskStatus(result);
                }
            }
            catch (HttpRequestException) { ErrorMessages.PrintServerIsNotRunning(); }
        }

        /// <summary>
        /// Парсит из json и выводит на консоль статус задачи.
        /// </summary>
        /// <param name="serialised"></param>
        private static void ParseAndPrintTaskStatus(string serialised)
        {
            dynamic taskStatus = JsonConvert.DeserializeObject<dynamic>(serialised);
            if (taskStatus.status == "ok")
            {
                // Вывода соответствует формату, указанному в условии.
                DirScanStatus result = taskStatus.result.ToObject<DirScanStatus>();
                Console.WriteLine("====== Scan result ======");
                Console.WriteLine();
                Console.WriteLine($"Processed files: {result.ProcessedFilesCount}");
                Console.WriteLine($"JS detects: {result.StatusesCount[FileScanStatus.JsDetected]}");
                Console.WriteLine($"rm -rf detects: {result.StatusesCount[FileScanStatus.RmrfDetected]}");
                Console.WriteLine($"Rundll32 detects: {result.StatusesCount[FileScanStatus.RundllDetected]}");
                Console.WriteLine($"Errors: {result.StatusesCount[FileScanStatus.Error]}");
                Console.WriteLine($"Exection time: {result.ExecutionTime:hh\\:mm\\:ss}");
                Console.WriteLine("=========================");
            }
            else
            {
                Console.WriteLine(taskStatus.status);
            }
        }
    }
}
