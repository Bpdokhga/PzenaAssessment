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
                }

                ZipFile.ExtractToDirectory(request.ZipPath, directory);


                Console.WriteLine($"Downloaded {request.ZipPath} succesfully from {request.Url}");



                //Insert_Data(Get_CsvFile_Data(request.CsvFilePath));
                Get_CsvFile_Data(request.CsvFilePath, tableName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} : DOWNLOAD FAILED");
                Console.WriteLine($"{DateTime.Now} : Error occurrred while downloading the file {request.ZipPath} ");
                Console.WriteLine($"{DateTime.Now} : {ex.Message}");
            }
        }

        // Method to get the .csv data as DataTable
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
                    //csvReader.Close();
                    //csvReader.Dispose();
                }
                //using (var reader = new StreamReader(filePath))
                //using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                //{
                //    //reader.ReadLine();
                //    using (var dr = new CsvDataReader(csv))
                //    {
                //        tickerData.Load(dr);
                //    }
                //    //while (csv.Read())
                //    //{
                //    //    var stockDataItem = new StockData
                //    //    {
                //    //        TableName = csv.GetField(0),
                //    //        Permaticker = csv.GetField(1),
                //    //        Ticker = csv.GetField(2),
                //    //        Name = csv.GetField(3),
                //    //        Exchange = csv.GetField(4),
                //    //        IsDelisted = Char.Parse(csv.GetField(5)) != null ? Char.Parse(csv.GetField(5)) : '\0',
                //    //        Category = csv.GetField(6) != null ? csv.GetField(6).Trim() : null,
                //    //        Cusips = csv.GetField(7) != null ? csv.GetField(7).Trim() : null,
                //    //        SicCode = csv.GetField(8) != null ? csv.GetField(8).Trim() : null,
                //    //        SicSector = csv.GetField(9) != null ? csv.GetField(9).Trim() : null,
                //    //        SicIndustry = csv.GetField(10) != null ? csv.GetField(10).Trim() : null,
                //    //        FamaSector = csv.GetField(11) != null ? csv.GetField(11).Trim() : null,
                //    //        FamaIndustry = csv.GetField(12) != null ? csv.GetField(12).Trim() : null,
                //    //        Sector = csv.GetField(13) != null ? csv.GetField(13).Trim() : null,
                //    //        Industry = csv.GetField(14) != null ? csv.GetField(14).Trim() : null,
                //    //        ScaleMarketCap = csv.GetField(15) != null ? csv.GetField(15).Trim() : null,
                //    //        ScaleRevenue = csv.GetField(16) != null ? csv.GetField(16).Trim() : null,
                //    //        RelatedTickers = csv.GetField(17) != null ? csv.GetField(17).Trim() : null,
                //    //        Currency = csv.GetField(18) != null ? csv.GetField(18).Trim() : null,
                //    //        Location = csv.GetField(19) != null ? csv.GetField(19).Trim() : null,
                //    //        LastUpdated = DateTime.Parse(csv.GetField(20)) != null? DateTime.TryParse(csv.GetField(20), DateTime.de) : null,
                //    //        FirstAdded = DateTime.Parse(csv.GetField(21)) != null? DateTime.Parse(csv.GetField(21)) : null,
                //    //        FirstPriceDate = DateTime.Parse(csv.GetField(22)) != null ? DateTime.Parse(csv.GetField(22)) : null,
                //    //        LastPriceDate = DateTime.Parse(csv.GetField(23)) != null ? DateTime.Parse(csv.GetField(23)) : null,
                //    //        FirstQuarter = DateTime.Parse(csv.GetField(24)) != null ? DateTime.Parse(csv.GetField(24)) : null,
                //    //        LastQuarter = DateTime.Parse(csv.GetField(25)) != null ? DateTime.Parse(csv.GetField(25)) : null,
                //    //        SecFilings = csv.GetField(26) != null ? csv.GetField(26).Trim() : null,
                //    //        CompanySite = csv.GetField(27) != null ? csv.GetField(27).Trim() : null,
                //    //    };
                //    //    stockDataFromFile.Add(stockDataItem);
                //    //}
                //}
                if (tableName != "dbo.Prices")
                    Insert_DataToSQL(tickerData, tableName, 5000);
                else
                    Insert_DataToSQL(tickerData, 10000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} : DATA EXTRACT FAILED");
                Console.WriteLine($"{DateTime.Now} : Error occurrred reading CSV file contents to prepare for SQL Load.");
                Console.WriteLine($"{DateTime.Now} : {ex.Message}");
            }
        }

        // Method to get the DataTable into the SQL Table
        private void Insert_DataToSQL(DataTable csvTable, string tableName, int batchSize = 10000)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();

                try
                {
                    using (SqlBulkCopy bulk = new SqlBulkCopy(dbConnection))
                    {
                        //bulk.DestinationTableName = "dbo.Stock_Data";
                        bulk.DestinationTableName = tableName;
                        bulk.BatchSize = batchSize;
                        //foreach (var column in csvTable.Columns)
                        //{
                        //    bulk.ColumnMappings.Add(column.ToString(), column.ToString());
                        //}
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

        private void Insert_DataToSQL(DataTable csvTable, int batchSize = 10000)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();

                try
                {
                    using (SqlBulkCopy bulk = new SqlBulkCopy(dbConnection))
                    {
                        //bulk.DestinationTableName = "dbo.Stock_Data";
                        bulk.DestinationTableName = "dbo.Stock_Data";
                        bulk.BatchSize = batchSize;
                        //foreach (var column in csvTable.Columns)
                        //{
                        //    bulk.ColumnMappings.Add(column.ToString(), column.ToString());
                        //}
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

                    command.Parameters.AddWithValue("@TickerSymbol", tickerSymbol);
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
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{DateTime.Now} : No data returned from the stored procedure.");
                        }
                    }
                }
            }
        }


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
