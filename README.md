# RideETL CLI Application
This project implements a simple ETL (Extract, Transform, Load) process for handling ride data. The application reads ride data from a CSV file, processes it to identify duplicate entries, and loads the cleaned data into a SQL Server database.
# Prerequisites
Software Requirements
.NET 8.0 
Microsoft SQL Server (local or remote)
Visual Studio or any compatible .NET IDE
NuGet Dependencies
The project uses the following NuGet packages:
CsvHelper for reading CSV files.
Newtonsoft.Json for JSON serialization.

# Input CSV File
Place your input CSV file (sample-cab-data.csv) in the in directory relative to the executable. Ensure it has the following columns:

# Comments
Number of rows from file: 30000.
Duplicate: 111 rows.
Number of rows in SQL table after running the program: 29889
