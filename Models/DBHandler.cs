﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace DO_Arbetsprov.Models
{
    public class DBHandler
    {
        //Get the path to db
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        //Gets all unique catalog entry codes
        public List<string> GetAllProducts()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = conn.CreateCommand())
            {
                conn.Open();
                //DISTINCT to remove duplicates
                command.CommandText = "SELECT DISTINCT CatalogEntryCode FROM prices";
                using (var reader = command.ExecuteReader())
                {
                    List<string> codes = new List<string>();
                    while (reader.Read())
                    {
                        codes.Add(reader.GetString(0));
                    }
                    return codes;
                }
            }
        }

        //Gets all market-currency pairs for a given catalog entry code
        public List<MarketCurrencyPair> GetProductMarketCurrencyPairs(string entryCode)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = conn.CreateCommand())
            {
                conn.Open();
                //DISTINCT to remove duplicates
                command.CommandText = "SELECT DISTINCT MarketId, CurrencyCode FROM prices WHERE CatalogEntryCode = @Code ORDER BY UnitPrice DESC";
                command.Parameters.AddWithValue("Code", entryCode);
                using (var reader = command.ExecuteReader())
                {
                    List<MarketCurrencyPair> markets = new List<MarketCurrencyPair>();
                    while (reader.Read())
                    {
                        markets.Add(new MarketCurrencyPair{
                            MarketId = reader.GetString(0),
                            CurrencyCode = reader.GetString(1)
                        });
                    }
                    return markets;
                }
            }
        }

        //Gets all price entries for a given catalog entry code
        public List<Price> GetProductPrices(string entryCode)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = conn.CreateCommand())
            {
                conn.Open();
                command.CommandText = "SELECT * FROM prices WHERE CatalogEntryCode = @Code ORDER BY UnitPrice DESC";
                command.Parameters.AddWithValue("Code", entryCode);
                using (var reader = command.ExecuteReader())
                {
                    List<Price> prices = new List<Price>();
                    while (reader.Read())
                    {
                        //Convert to a price object
                        Price price = new Price
                        {
                            PriceValueId = reader.GetInt32(0),
                            Created = reader.GetDateTime(1),
                            Modified = reader.GetDateTime(2),
                            CatalogEntryCode = reader.GetString(3),
                            MarketId = reader.GetString(4),
                            CurrencyCode = reader.GetString(5),
                            ValidFrom = reader.GetDateTime(6),
                            ValidUntil = reader.GetValue(7) as DateTime?,
                            UnitPrice = reader.GetDecimal(8)
                        };
                        prices.Add(price);
                    }
                    return prices;
                }
            }
        }
    }
}