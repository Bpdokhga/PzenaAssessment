using PzenaAssessment.Models;
using System.IO;

public class Program
{
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
            await repository.DownloadFileAsync(tickersRequest, "dbo.Stock_Data", cancelTokenForTickers.Token);
            Console.WriteLine($"{DateTime.Now} : TICKERS DOWNLOADED & STORED SUCCESSFULLY");


            // Prices Process
            await repository.DownloadFileAsync(pricesRequest, "dbo.Prices", null);
            Console.WriteLine($"{DateTime.Now} : PRICES DOWNLOADED & STORED SUCCESSFULLY");
            //await repository.DownloadFileAsync(pricesRequest, cancelTokenForTickers.Token);

            Console.WriteLine("Please enter ticker symbol for which you want to receive statistics for: ");
            string tickerSymbol = Console.ReadLine();

            await repository.Execute_Storedprocedure(tickerSymbol);


            //await Task.WhenAll
            //await repository.DownloadFileAsync(pricesRequest, cancelTokenForPrices.Token);
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


    private static DownloadRequest CreateRequest(string url, string fileName, string newFileName)
    {
        DownloadRequest request = new DownloadRequest();

        string downloadDirectory = @"C:\DownloadedFilesPzena";

        request.Url = url;
        request.ZipPath = Path.Combine(downloadDirectory, fileName);
        request.CsvFilePath = Path.Combine(downloadDirectory, newFileName);
        return request;
    }
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