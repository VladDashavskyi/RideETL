using CsvHelper.Configuration;
using CsvHelper;
using RideETL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        string connectionString = "Data Source=DESKTOP-E2MTC8P;" +
                                  "Initial Catalog=Intergration;" +
                                  "User id=ContactManager;" +
                                  "Password=123;";
        RunETL(connectionString);
    }

    static void RunETL(string connectionString)
    {
        try
        {
            Console.WriteLine("Start proccess...");
            IEnumerable<dynamic> extractRecords = Extract(@"in\sample-cab-data.csv");

            Tuple<List<RideDataRecord>, List<RideDataRecord>> tuple = Transform(extractRecords);

            SaveDuplicateToCsv(tuple.Item2);

            Load(tuple.Item1,connectionString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during running, error message: {ex.Message}");
        }
    }

    static IEnumerable<dynamic> Extract(string filePath)
    {
        Console.WriteLine("Start Extracting.");
        if (File.Exists(filePath))
        {
            using StreamReader reader = new(filePath);
            using CsvReader csv = new(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,

            });

            IEnumerable<dynamic> records = csv.GetRecords<dynamic>().ToList();
            Console.WriteLine($"Records were extracted, records count:{records.Count()}");
            return records;
        }
        else
        {
            Console.WriteLine("File not exist");
            return null;
        }
    }

    static Tuple<List<RideDataRecord>, List<RideDataRecord>> Transform(IEnumerable<dynamic> extractRecords)
    {
        Console.WriteLine("Start Transform.");
        List<RideDataRecord> proccessedRecords = new();
        List<RideDataRecord> duplicateRecords = new();
        HashSet<string> hashSetRecords = new();
        dynamic rec = null;

        try
        {
            foreach (dynamic record in extractRecords)
            {
                rec = record;
                RideDataRecord recordData = new()
                {
                    PickUpDateTime = DateTime.Parse(record.tpep_pickup_datetime).ToUniversalTime(),
                    DropOffDateTime = DateTime.Parse(record.tpep_dropoff_datetime).ToUniversalTime(),
                    PassengerCount = string.IsNullOrEmpty(record.passenger_count) ? 0:  int.Parse(record.passenger_count),
                    TripDistance = float.Parse(record.trip_distance),
                    StoreAndFwdFlag = record.store_and_fwd_flag.Trim() == "N" || string.IsNullOrEmpty(record.store_and_fwd_flag.Trim()) ? "No" : "Yes",
                    PULocationId = int.Parse(record.PULocationID),
                    DOLocationId = int.Parse(record.DOLocationID),
                    FareAmount = decimal.Parse(record.fare_amount),
                    TipAmount = decimal.Parse(record.tip_amount)
                };
                string key = $"{recordData.PickUpDateTime}|{recordData.DropOffDateTime}|{recordData.PassengerCount}";

                if (hashSetRecords.Contains(key))
                {
                    duplicateRecords.Add(recordData);
                }
                else
                {
                    hashSetRecords.Add(key);
                    proccessedRecords.Add(recordData);
                }
            }
            return new Tuple<List<RideDataRecord>, List<RideDataRecord>>(proccessedRecords, duplicateRecords);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during parsing record: {JsonConvert.SerializeObject(rec)}");
        }
    }

    static void Load(List<RideDataRecord> rideDataRecords, string connectionString)
    {
        Console.WriteLine("Start Loading");

        using SqlConnection conection = new(connectionString);
        conection.Open();

        using SqlBulkCopy bulkCopy = new(conection)
        {
            DestinationTableName = "RideData"
        };


        DataTable dataTable = new();
        dataTable.Columns.Add("PickUpDateTime", typeof(DateTime));
        dataTable.Columns.Add("DropOffDateTime", typeof(DateTime));
        dataTable.Columns.Add("PassengerCount", typeof(int));
        dataTable.Columns.Add("TripDistance", typeof(float));
        dataTable.Columns.Add("StoreAndFwdFlag", typeof(string));
        dataTable.Columns.Add("PULocationId", typeof(int));
        dataTable.Columns.Add("DOLocationId", typeof(int));
        dataTable.Columns.Add("FareAmount", typeof(decimal));
        dataTable.Columns.Add("TipAmount", typeof(decimal));


        foreach (RideDataRecord record in rideDataRecords)
        {

            dataTable.Rows.Add(record.PickUpDateTime,
            record.DropOffDateTime,
            record.PassengerCount,
            record.TripDistance,
            record.StoreAndFwdFlag,
            record.PULocationId,
            record.DOLocationId,
            record.FareAmount,
            record.TipAmount);

        }
        
            bulkCopy.WriteToServer(dataTable);
            Console.WriteLine("Data was succsesfully loaded to SQL Server.");        
    }

    static void SaveDuplicateToCsv(List<RideDataRecord> rideDatasDuplicates)
    {
        Console.WriteLine("Start writing duplicates.");
        using StreamWriter streamWriter = new(@"in\duplicates.csv");
        using CsvWriter csvWriter = new(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture));
        csvWriter.WriteRecords(rideDatasDuplicates);

        Console.WriteLine($"Duplicates were saved, duplicates count:{rideDatasDuplicates.Count()}");
    }
}