using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace DO_Arbetsprov.App_Start
{
    public class DBInit
    {
        private static readonly string dbDir = HostingEnvironment.MapPath("~/App_Data/");
        private static readonly string dbFilePath = dbDir + "/DefaultDB.db";
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static void InitializeDatabase()
        {
            if (File.Exists(dbFilePath)) return;
            string[] csvFilePaths = Directory.GetFiles(dbDir).Where(file => file.EndsWith(".csv")).ToArray();
            if (csvFilePaths.Length == 0) throw new Exception("No CSV files found in App_Data");
            SQLiteConnection.CreateFile(dbFilePath);

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = conn.CreateCommand())
            {
                conn.Open();
                command.CommandText = "CREATE TABLE prices (PriceValueId integer, Created DateTime, Modified DateTime, CatalogEntryCode nvarchar, MarketId nvarchar, CurrencyCode nvarchar, ValidFrom DateTime, ValidUntil DateTime, UnitPrice decimal)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO prices VALUES (@PriceValueId, @Created, @Modified, @CatalogEntryCode, @MarketId, @CurrencyCode, @ValidFrom, @ValidUntil, @UnitPrice)";
                using (TextFieldParser csvParser = new TextFieldParser(csvFilePaths[0]))
                {
                    csvParser.SetDelimiters(new string[] { "\t" });
                    csvParser.HasFieldsEnclosedInQuotes = true;

                    //Skip the row with the column names
                    csvParser.ReadLine();

                    List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                    while (!csvParser.EndOfData)
                    {
                        //Get fields on the current row
                        string[] fields = csvParser.ReadFields();

                        //Handle validUntil separately as it is nullable
                        SQLiteParameter validUntil = new SQLiteParameter("@ValidUntil", DbType.DateTime);
                        if (fields[7] != "NULL") validUntil.Value = DateTime.Parse(fields[7]);

                        paramList.Add(new SQLiteParameter[] {
                            new SQLiteParameter("@PriceValueId", int.Parse(fields[0])),
                            new SQLiteParameter("@Created", DateTime.Parse(fields[1])),
                            new SQLiteParameter("@Modified", DateTime.Parse(fields[2])),
                            new SQLiteParameter("@CatalogEntryCode", fields[3]),
                            new SQLiteParameter("@MarketId", fields[4]),
                            new SQLiteParameter("@CurrencyCode", fields[5]),
                            new SQLiteParameter("@ValidFrom", DateTime.Parse(fields[6])),
                            validUntil,
                            new SQLiteParameter("@UnitPrice", Decimal.Parse(fields[8]))
                        });
                    }
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (SQLiteParameter[] parameters in paramList)
                        {
                            command.Parameters.AddRange(parameters);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }
        }
    }
}