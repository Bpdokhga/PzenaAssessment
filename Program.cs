using PzenaAssessment.Models;
using System.IO;

public class Program
{
    // Main 
    public static async Task Main(string[] args)
    {

        var tickersRequest = CreateRequest("https://www.alphaforge.net/A0B1C3/TICKERS.zip", "TICKERS.zip", "TICKERS.csv");
        var pricesRequest = CreateRequest("https://www.alphaforge.net/A0B1C3/PRICES.zip", "PRICES.zip", "PRICES.csv");

        var connectionString = Get_ConnectionString();

        var repository = new DownloadRepository(connectionString);

        var cancelTokenForTickers = new CancellationTokenSource();
        var cancelTokenForPrices = new CancellationTokenSource();


        //cancelTokenForTickers.CancelAfter(TimeSpan.FromSeconds(30));
        //cancelTokenForPrices.CancelAfter(TimeSpan.FromSeconds(90));


        try
        {
            // Tickers Process
            await repository.DownloadFileAsync(tickersRequest, "dbo.Ticker", cancelTokenForTickers.Token);
            Console.WriteLine($"{DateTime.Now} : TICKERS DOWNLOADED & STORED SUCCESSFULLY");


            // Prices Process
            await repository.Download_Prices(pricesRequest, null);
            Console.WriteLine($"{DateTime.Now} : PRICES DOWNLOADED SUCCESSFULLY");

            await repository.Read_Prices_Async_Csv_By_Chunk(pricesRequest.CsvFilePath, 5000);
            Console.WriteLine($"{DateTime.Now} : PRICES READ SUCCESSFULLY");


            Console.WriteLine("Please enter ticker symbol for which you want to receive statistics for: ");
            string tickerSymbol = Console.ReadLine();

            repository.Execute_Storedprocedure(tickerSymbol);


            Console.WriteLine($"{DateTime.Now}: ALL FINISHED!");
        }
        catch(OperationCanceledException opCanceledEx)
        {
            Console.WriteLine($"{DateTime.Now} : EXCEPTION OCCURRED IN MAIN: ");
            Console.WriteLine($"{DateTime.Now} : OPERATION CANCELED");
            Console.WriteLine($"{DateTime.Now} : {opCanceledEx.Message}");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"{DateTime.Now} : EXCEPTION OCCURRED IN MAIN: ");
            Console.WriteLine($"{DateTime.Now} : {ex.Message}");
        }

        Console.ReadKey();
    }

    // Creates a Downloadrequest object
    private static DownloadRequest CreateRequest(string url, string fileName, string newFileName)
    {
        DownloadRequest request = new DownloadRequest();

        string downloadDirectory = @"C:\DownloadedFilesPzena";

        request.Url = url;
        request.ZipPath = Path.Combine(downloadDirectory, fileName);
        request.CsvFilePath = Path.Combine(downloadDirectory, newFileName);
        return request;
    }

    // Retreieves Connections String
    // Ideally; thi would either be an AppSecret
    // set by CI/CD process as environment variable
    // for the server in which it resides; DEV/QA/PROD
    private static string Get_ConnectionString()
    {
        string connectionString = "";
        string secretsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "AppSecrets");
        try
        {

            if (!Directory.Exists(secretsDirectory))
            {
                Directory.CreateDirectory(secretsDirectory);
            }
            connectionString = File.ReadAllText(Path.Combine(secretsDirectory, "ConnectionString.txt")).Trim();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"{DateTime.Now} : EXCEPTION OCCURRED GETTING CONNECTION_STRING: ");
            Console.WriteLine($"{DateTime.Now} : {ex.Message}");
        }
        return connectionString;
    }

}