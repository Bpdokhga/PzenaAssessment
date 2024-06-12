using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using PzenaAssessment.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PzenaAssessment.Models
{
    public class DownloadRepository : IDownloadRepository
    {
        private readonly HttpClient _httpClient;

        private readonly string _connectionString;

        public DownloadRepository(string connectionString)
        {
            _httpClient = new HttpClient();
            _connectionString = connectionString;
        }



        // Method to get Download the files
        // Seperate Tickers Download
        public async Task DownloadFileAsync(DownloadRequest request, string tableName, CancellationToken? cancelToken)
        {
            try
            {
                string directory = Path.GetDirectoryName(request.ZipPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(request.ZipPath))
                {
                    File.Delete(request.ZipPath);
                    Console.WriteLine($"{DateTime.Now} : Deleted exisiting ZIP file from: {request.ZipPath} ");
                }

                if (File.Exists(request.CsvFilePath))
                {
                    File.Delete(request.CsvFilePath);
                    Console.WriteLine($"{DateTime.Now} : Deleted exisiting CSV file from: {request.CsvFilePath} ");
                }

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(request.Url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        // Check for successful response status code (assuming authorized access)
                        if (response.IsSuccessStatusCode)
                        {
                            using (var fileStream = new FileStream(request.ZipPath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fileStream);
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"{DateTime.Now} : Failed to download file: {response.StatusCode}");
                            // Handle unsuccessful response (e.g., authorization error, unavailable resource)
                        }
                    }
                    httpClient.Dispose();
                }

                ZipFile.ExtractToDirectory(request.ZipPath, directory);


                Console.WriteLine($"Downloaded {request.ZipPath} succesfully from {request.Url}");

                Get_CsvFile_Data(request.CsvFilePath, tableName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} : DOWNLOAD FAILED");
                Console.WriteLine($"{DateTime.Now} : Error occurrred while downloading the file {request.ZipPath} ");
                Console.WriteLine($"{DateTime.Now} : {ex.Message}");
            }
        }

        // Seperate Prices Download
        public async Task Download_Prices(DownloadRequest request, CancellationToken? cancelToken)
        {
            try
            {
                string directory = Path.GetDirectoryName(request.ZipPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(request.ZipPath))
                {
                    File.Delete(request.ZipPath);
                    Console.WriteLine($"{DateTime.Now} : Deleted exisiting ZIP file from: {request.ZipPath} ");
                }

                if (File.Exists(request.CsvFilePath))
                {
                    File.Delete(request.CsvFilePath);
                    Console.WriteLine($"{DateTime.Now} : Deleted exisiting CSV file from: {request.CsvFilePath} ");
                }

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(request.Url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        // Check for successful response status code (assuming authorized access)
                        if (response.IsSuccessStatusCode)
                        {
                            using (var fileStream = new FileStream(request.ZipPath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fileStream);
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"{DateTime.Now} : Failed to download file: {response.StatusCode}");
                        }
                    }
                    httpClient.Dispose();
                }

                ZipFile.ExtractToDirectory(request.ZipPath, directory);


                Console.WriteLine($"Downloaded {request.ZipPath} succesfully from {request.Url}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} : DOWNLOAD FAILED");
                Console.WriteLine($"{DateTime.Now} : Error occurrred while downloading the file {request.ZipPath} ");
                Console.WriteLine($"{DateTime.Now} : {ex.Message}");
            }
        }

        // Method to get the .csv data as DataTable
        // Very costly as DataTable is in-memory
        private void Get_CsvFile_Data(string filePath, string tableName)
        {
            DataTable tickerData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(filePath))
                {
                    csvReader.SetDelimiters(",");
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        tickerData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        tickerData.Rows.Add(fieldData);
                    }
                    csvReader.Close();
                    csvReader.Dispose();
                }
                Insert_DataToSQL(tickerData, tableName, 5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} : DATA EXTRACT FAILED");
                Console.WriteLine($"{DateTime.Now} : Error occurrred reading CSV file contents to prepare for SQL Load.");
                Console.WriteLine($"{DateTime.Now} : {ex.Message}");
            }
        }

        // Working for Pirces
        // Using a CustomCsvReader for Prices specifically
        // Loads a List and sens that list to 
        // be insertd into Database
        // Re-writting to Ommit the TextFieldParser
        //public async Task Read_Prices_Async_Csv_By_Chunk(string filePath, int chunkSize = 1000)
        //{
        //    List<Price> priceList = new List<Price>();

        //    using (TextFieldParser parser = new TextFieldParser(filePath))
        //    {
        //        parser.TextFieldType = FieldType.Delimited;
        //        parser.SetDelimiters(",");
        //        parser.HasFieldsEnclosedInQuotes = true;

        //        // Skip the header row
        //        parser.ReadLine();

        //        while (!parser.EndOfData)
        //        {
        //            string[] fields = parser.ReadFields();

        //            Price stockData = new Price
        //            {
        //                Ticker = fields[0],
        //                Date = DateTime.TryParse(fields[1], out DateTime date) ? date : (DateTime?)null,
        //                OpenPrice = double.TryParse(fields[2], out double open) ? open : (double?)null,
        //                HighPrice = double.TryParse(fields[3], out double high) ? high : (double?)null,
        //                LowPrice = double.TryParse(fields[4], out double low) ? low : (double?)null,
        //                ClosePrice = double.TryParse(fields[5], out double close) ? close : (double?)null,
        //                Volume = double.TryParse(fields[6], out double volume) ? volume : (double?)null,
        //                CloseAdj = double.TryParse(fields[7], out double closeadj) ? closeadj : (double?)null,
        //                CloseUnadj = double.TryParse(fields[8], out double closeunadj) ? closeunadj : (double?)null,
        //                LastUpdated = DateTime.TryParse(fields[9], out DateTime lastupdated) ? lastupdated : (DateTime?)null
        //            };

        //            priceList.Add(stockData);

        //            if (priceList.Count % chunkSize == 0)
        //            {
        //                await InsertDataToSQLAsync(priceList);
        //                priceList.Clear();
        //            }
        //        }

        //        if (priceList.Count > 0)
        //        {
        //            await InsertDataToSQLAsync(priceList);
        //        }
        //        parser.Close();
        //        parser.Dispose();
        //    }
        //}
        public async Task Read_Prices_Async_Csv_By_Chunk(string filePath, int chunkSize = 1000)
        {
            List<Price> priceList = new List<Price>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                // Skip the header row
                await reader.ReadLineAsync();

                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    string[] fields = ParseCsvLine(line);

                    Price stockData = new Price
                    {
                        Ticker = fields[0],
                        Date = DateTime.TryParse(fields[1], out DateTime date) ? date : (DateTime?)null,
                        OpenPrice = double.TryParse(fields[2], out double open) ? open : (double?)null,
                        HighPrice = double.TryParse(fields[3], out double high) ? high : (double?)null,
                        LowPrice = double.TryParse(fields[4], out double low) ? low : (double?)null,
                        ClosePrice = double.TryParse(fields[5], out double close) ? close : (double?)null,
                        Volume = double.TryParse(fields[6], out double volume) ? volume : (double?)null,
                        CloseAdj = double.TryParse(fields[7], out double closeadj) ? closeadj : (double?)null,
                        CloseUnadj = double.TryParse(fields[8], out double closeunadj) ? closeunadj : (double?)null,
                        LastUpdated = DateTime.TryParse(fields[9], out DateTime lastupdated) ? lastupdated : (DateTime?)null
                    };

                    priceList.Add(stockData);

                    if (priceList.Count % chunkSize == 0)
                    {
                        await InsertDataToSQLAsync(priceList);
                        priceList.Clear();
                    }
                }

                if (priceList.Count > 0)
                {
                    await InsertDataToSQLAsync(priceList);
                }
            }
        }

        // NEEDS ParseCSVLine Method
        private string[] ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            bool inQuotes = false;
            string field = string.Empty;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"' && (i == 0 || line[i - 1] != '\\'))
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(field);
                    field = string.Empty;
                }
                else
                {
                    field += c;
                }
            }

            fields.Add(field);
            return fields.ToArray();
        }



        // Working for Prices
        // Insert method for inserting Prices data into SQL.
        // Takes a List<Price> and inserts
        public async Task InsertDataToSQLAsync(List<Price> priceList)
        {
            string connectionString = $"{_connectionString}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "dbo.Price";

                    using (var reader = new PriceDataReader(priceList))
                    {
                        await bulkCopy.WriteToServerAsync(reader);
                    }
                }
                connection.Close();
                connection.Dispose();
            }
        }

        // Takes a dataTable
        // Inserts it into SQL
        // In efficient; possible corruption issues
        // Prices insert would take too lon
        private void Insert_DataToSQL(DataTable csvTable, string tableName, int batchSize = 10000)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();

                try
                {
                    using (SqlBulkCopy bulk = new SqlBulkCopy(dbConnection))
                    {
                        bulk.DestinationTableName = tableName;
                        bulk.BatchSize = batchSize;

                        bulk.WriteToServer(csvTable);
                        bulk.Close();
                    }
                    dbConnection.Close();
                    dbConnection.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} : Error in the transaction.");
                    Console.WriteLine($"{DateTime.Now} : {ex.Message}");
                    //transaction.Rollback();
                }
            }
        }

        // Execute Stored procedure
        // Requires a Ticker symbol as parameter to get the statistics for.
        public void Execute_Storedprocedure(string tickerSymbol)
        {
            // Create a SqlConnection object
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Open the connection
                connection.Open();

                // Create a SqlCommand object for the stored procedure
                using (SqlCommand command = new SqlCommand("dbo.CalculateTickerStatistics", connection))
                {
                    // Specify that it's a stored procedure
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Ticker", tickerSymbol);
                    // Execute the stored procedure
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if the result set has rows
                        if (reader.HasRows)
                        {
                            // Loop through the result set and write each row to the console
                            while (reader.Read())
                            {
                                // Example: Write the first column value to the console
                                Console.WriteLine($"{DateTime.Now} : {reader[0]}");
                                Console.WriteLine($"{DateTime.Now} : {reader[1]}");
                                Console.WriteLine($"{DateTime.Now} : {reader[2]}");
                                Console.WriteLine($"{DateTime.Now} : {reader[3]}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{DateTime.Now} : No data returned from the stored procedure.");
                        }
                    }

                }
                connection.Close();
                connection.Dispose();
            }
        }








        // COMMENTED OUT; PREVIOUS ITERATIONS

        // Works but only 10 rows at a time

        //public async Task InsertDataToSQLAsync(List<Price> priceList, int batchSize = 10)
        //{
        //    string connectionString = $"{_connectionString}";

        //    StringBuilder insertQuery = new StringBuilder();
        //    insertQuery.AppendLine("INSERT INTO dbo.Price ([Ticker], [Date], [Open], [High], [Low], [Close], [Volume], [CloseAdj], [CloseUnadj], [LastUpdated])");
        //    insertQuery.AppendLine("VALUES");

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        await connection.OpenAsync();

        //        int count = 0;
        //        foreach (Price price in priceList)
        //        {
        //            insertQuery.AppendLine($"(@Ticker{count}, @Date{count}, @Open{count}, @High{count}, @Low{count}, @Close{count}, @Volume{count}, @CloseAdj{count}, @CloseUnadj{count}, @LastUpdated{count})");

        //            count++;

        //            if (count % batchSize == 0 || count == priceList.Count)
        //            {
        //                using (SqlCommand command = new SqlCommand(insertQuery.ToString(), connection))
        //                {
        //                    for (int i = 0; i < count; i++)
        //                    {
        //                        Price currentPrice = priceList[i];

        //                        command.Parameters.AddWithValue($"@Ticker{i}", currentPrice.Ticker);
        //                        command.Parameters.AddWithValue($"@Date{i}", currentPrice.Date);
        //                        command.Parameters.AddWithValue($"@Open{i}", currentPrice.OpenPrice ?? (object)DBNull.Value);
        //                        command.Parameters.AddWithValue($"@High{i}", currentPrice.HighPrice ?? (object)DBNull.Value);
        //                        command.Parameters.AddWithValue($"@Low{i}", currentPrice.LowPrice ?? (object)DBNull.Value);
        //                        command.Parameters.AddWithValue($"@Close{i}", currentPrice.ClosePrice ?? (object)DBNull.Value);
        //                        command.Parameters.AddWithValue($"@Volume{i}", currentPrice.Volume ?? (object)DBNull.Value);
        //                        command.Parameters.AddWithValue($"@CloseAdj{i}", currentPrice.CloseAdj ?? (object)DBNull.Value);
        //                        command.Parameters.AddWithValue($"@CloseUnadj{i}", currentPrice.CloseUnadj ?? (object)DBNull.Value);
        //                        command.Parameters.AddWithValue($"@LastUpdated{i}", currentPrice.LastUpdated);
        //                    }

        //                    await command.ExecuteNonQueryAsync();
        //                }

        //                // Reset the string builder for the next batch
        //                insertQuery.Clear();
        //                insertQuery.AppendLine("INSERT INTO dbo.Price ([Ticker], [Date], [Open], [High], [Low], [Close], [Volume], [CloseAdj], [CloseUnadj], [LastUpdated])");
        //                insertQuery.AppendLine("VALUES");
        //            }
        //            else
        //            {
        //                insertQuery.AppendLine(",");
        //            }
        //        }
        //    }
        //}


        // WORKS 1 at a time I believe however//
        //public async Task InsertDataToSQLAsync(List<Price> priceList, int batchSize = 1000)
        //{
        //    string connectionString = $"{_connectionString}";

        //    string insertQuery = @"INSERT INTO dbo.Price ([Ticker], [Date], [Open], [High], [Low], [Close], Volume, CloseAdj, CloseUnadj, LastUpdated) 
        //                   VALUES (@Ticker, @Date, @Open, @High, @Low, @Close, @Volume, @CloseAdj, @CloseUnadj, @LastUpdated)";

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        for (int i = 0; i < priceList.Count; i += batchSize)
        //        {
        //            List<Price> batch = priceList.GetRange(i, Math.Min(batchSize, priceList.Count - i));

        //            using (SqlCommand command = new SqlCommand(insertQuery, connection))
        //            {
        //                foreach (Price price in batch)
        //                {
        //                    command.Parameters.Clear();
        //                    command.Parameters.AddWithValue("@Ticker", price.Ticker);
        //                    command.Parameters.AddWithValue("@Date", price.Date);
        //                    command.Parameters.AddWithValue("@Open", price.OpenPrice ?? (object)DBNull.Value);
        //                    command.Parameters.AddWithValue("@High", price.HighPrice ?? (object)DBNull.Value);
        //                    command.Parameters.AddWithValue("@Low", price.LowPrice ?? (object)DBNull.Value);
        //                    command.Parameters.AddWithValue("@Close", price.ClosePrice ?? (object)DBNull.Value);
        //                    command.Parameters.AddWithValue("@Volume", price.Volume ?? (object)DBNull.Value);
        //                    command.Parameters.AddWithValue("@CloseAdj", price.CloseAdj ?? (object)DBNull.Value);
        //                    command.Parameters.AddWithValue("@CloseUnadj", price.CloseUnadj ?? (object)DBNull.Value);
        //                    command.Parameters.AddWithValue("@LastUpdated", price.LastUpdated);

        //                    await command.ExecuteNonQueryAsync();
        //                }
        //            }
        //        }
        //    }
        //}
        //public async Task Read_Async_Csv_By_Chunk(string filePath, int chunkSize = 1000)
        //{
        //    DataTable dataTable = new DataTable();
        //    dataTable.Columns.AddRange(new[]
        //    {
        //        new DataColumn("ticker", typeof(string)),
        //        new DataColumn("date", typeof(DateTime)),
        //        new DataColumn("open", typeof(decimal)),
        //        new DataColumn("high", typeof(decimal)),
        //        new DataColumn("low", typeof(decimal)),
        //        new DataColumn("close", typeof(decimal)),
        //        new DataColumn("volume", typeof(decimal)),
        //        new DataColumn("closeadj", typeof(decimal)),
        //        new DataColumn("closeunadj", typeof(decimal)),
        //        new DataColumn("lastupdated", typeof(DateTime))
        //    });

        //    using (TextFieldParser parser = new TextFieldParser(filePath))
        //    {
        //        parser.TextFieldType = FieldType.Delimited;
        //        parser.SetDelimiters(",");
        //        parser.HasFieldsEnclosedInQuotes = true;

        //        // Skip the header row
        //        parser.ReadLine();

        //        while (!parser.EndOfData)
        //        {
        //            string[] fields = parser.ReadFields();
        //            DataRow row = dataTable.NewRow();

        //            row["ticker"] = fields[0];
        //            row["date"] = DateTime.TryParse(fields[1], out DateTime date) ? (object)date : DBNull.Value;
        //            row["open"] = decimal.TryParse(fields[2], out decimal open) ? (object)open : DBNull.Value;
        //            row["high"] = decimal.TryParse(fields[3], out decimal high) ? (object)high : DBNull.Value;
        //            row["low"] = decimal.TryParse(fields[4], out decimal low) ? (object)low : DBNull.Value;
        //            row["close"] = decimal.TryParse(fields[5], out decimal close) ? (object)close : DBNull.Value;
        //            row["volume"] = decimal.TryParse(fields[6], out decimal volume) ? (object)volume : DBNull.Value;
        //            row["closeadj"] = decimal.TryParse(fields[7], out decimal closeadj) ? (object)closeadj : DBNull.Value;
        //            row["closeunadj"] = decimal.TryParse(fields[8], out decimal closeunadj) ? (object)closeunadj : DBNull.Value;
        //            row["lastupdated"] = DateTime.TryParse(fields[9], out DateTime lastupdated) ? (object)lastupdated : DBNull.Value;

        //            dataTable.Rows.Add(row);

        //            if (dataTable.Rows.Count % 10000 == 0)
        //            {
        //                await InsertDataToSQLAsync(dataTable);
        //                dataTable.Clear();
        //            }
        //        }

        //        if (dataTable.Rows.Count > 0)
        //        {
        //            await InsertDataToSQLAsync(dataTable);
        //        }
        //    }
        //}
        // Method to get the DataTable into the SQL Table

        //public async Task Load_StockData(string filePath)
        //{
        //    if (!File.Exists(filePath))
        //    {
        //        throw new FileNotFoundException($"TICKERS.zip not found at {filePath}");
        //    }

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        using (var streamReader = new StreamReader(filePath, Encoding.UTF8))
        //        {
        //            // Assuming your CSV file has headers and a specific format
        //            // You might need to adjust this based on your actual CSV structure
        //            string headerLine = await streamReader.ReadLineAsync();
        //            string dataLine;
        //            while ((dataLine = await streamReader.ReadLineAsync()) != null)
        //            {
        //                string[] parts = dataLine.Split(','); // Assuming comma-separated values

        //                // Build your SQL INSERT statement based on the CSV data
        //                string sql = $"INSERT INTO [dbo].[Stock_Data] ([table_name], " +
        //                    $"                                  [permaticker], " +
        //                    $"                                  [ticker]," +
        //                    $"                                  [name]," +
        //                    $"                                  [exchange]," +
        //                    $"                                  [isdelisted]," +
        //                    $"                                  [category]," +
        //                    $"                                  [cusips]," +
        //                    $"                                  [siccode]," +
        //                    $"                                  [sicsector]," +
        //                    $"                                  [sicindustry], " +
        //                    $"                                  [famasector]," +
        //                    $"                                  [famaindustry]," +
        //                    $"                                  [sector]," +
        //                    $"                                  [industry]," +
        //                    $"                                  [scalemarketcap]," +
        //                    $"                                  [scalerevenue]," +
        //                    $"                                  [relatedtickers]," +
        //                    $"                                  [currency]," +
        //                    $"                                  [location]," +
        //                    $"                                  [lastupdated]," +
        //                    $"                                  [firstadded]," +
        //                    $"                                  [firstpricedate]," +
        //                    $"                                  [lastpricedate]," +
        //                    $"                                  [firstquarter]," +
        //                    $"                                  [lastquarter]," +
        //                    $"                                  [secfilings]," +
        //                    $"                                  [companysite]" +
        //                    $"                                  ) VALUES ('{parts[0]}', " +
        //                    $"                                            '{parts[1]}', " +
        //                    $"                                            '{parts[2]}', " +
        //                    $"                                            '{parts[3]}', " +
        //                    $"                                            '{parts[4]}', " +
        //                    $"                                            '{parts[5]}', " +
        //                    $"                                            '{parts[6]}', " +
        //                    $"                                            '{parts[7]}', " +
        //                    $"                                            '{parts[8]}', " +
        //                    $"                                            '{parts[9]}', " +
        //                    $"                                            '{parts[10]}', " +
        //                    $"                                            '{parts[11]}', " +
        //                    $"                                            '{parts[12]}', " +
        //                    $"                                            '{parts[13]}', " +
        //                    $"                                            '{parts[14]}', " +
        //                    $"                                            '{parts[15]}', " +
        //                    $"                                            '{parts[16]}', " +
        //                    $"                                            '{parts[17]}', " +
        //                    $"                                            '{parts[18]}', " +
        //                    $"                                            '{parts[19]}', " +
        //                    $"                                            '{parts[20]}', " +
        //                    $"                                            '{parts[21]}', " +
        //                    $"                                            '{parts[22]}', " +
        //                    $"                                            '{parts[23]}', " +
        //                    $"                                            '{parts[24]}', " +
        //                    $"                                            '{parts[25]}', " +
        //                    $"                                            '{parts[26]}', " +
        //                    $"                                            '{parts[27]}' " +
        //                    $"                                             )";

        //                using (var command = new SqlCommand(sql, connection))
        //                {
        //                    await command.ExecuteNonQueryAsync();
        //                }
        //            }
        //        }

        //        await connection.CloseAsync();
        //        Debug.WriteLine("Loaded tickers data into SQL table");

        //    }
        //}

        //public async Task InsertCsvData(string filePath)
        //{
        //    try
        //    {
        //        using (var reader = new StreamReader(filePath))
        //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();
        //            var records = new List<dynamic>();
        //            const int batchSize = 1000; // Adjust batch size as needed

        //            while (await csv.ReadAsync())
        //            {
        //                var record = csv.GetRecord<dynamic>();
        //                records.Add(record);

        //                if (records.Count >= batchSize)
        //                {
        //                    await InsertBatchIntoDatabase(records, connection);
        //                    records.Clear();
        //                }
        //            }

        //            if (records.Count > 0)
        //            {
        //                await InsertBatchIntoDatabase(records, connection);
        //            }
        //            reader.Close();
        //            connection.Close();
        //            csv.Dispose();
        //            connection.Dispose();
        //            connection.Dispose();
        //        }
        //        Console.WriteLine($"{DateTime.Now} : Inserted data from {filePath} into the database; SUCCESS");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"{DateTime.Now} : ERROR INSERTING INTO DATABASE; FAILED");
        //        Console.WriteLine($"{DateTime.Now} : {ex.Message}");
        //    }
        //}

        //private async Task InsertBatchIntoDatabase(List<dynamic> records, SqlConnection connection)
        //{
        //    try
        //    {
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            foreach (var record in records)
        //            {
        //                // Example: Adjust based on your table structure
        //                var query = "INSERT INTO YourTable (Column1, Column2, ...) VALUES (@Value1, @Value2, ...)";
        //                using (var command = new SqlCommand(query, connection, transaction))
        //                {
        //                    // Set parameters from CSV record
        //                    command.Parameters.AddWithValue("@Value1", record.Column1);
        //                    command.Parameters.AddWithValue("@Value2", record.Column2);
        //                    // Add more parameters as needed

        //                    await command.ExecuteNonQueryAsync();
        //                }
        //            }

        //            transaction.Commit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"{DateTime.Now} : ERROR INSERTING BATCH INTO DATABASE; FAILED");
        //        Console.WriteLine($"{DateTime.Now} : {ex.Message}");
        //    }
        //}
    }
}
