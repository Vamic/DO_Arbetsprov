using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DO_Arbetsprov.Models
{
    public class PriceService
    {
        DBHandler dbHandler = new DBHandler();

        private List<Price> CreateTimeline(List<Price> prices)
        {

            //The list of prices that will be displayed
            List<Price> priceHistory = new List<Price>();
            
            Price baseprice = prices.FirstOrDefault(price => price.ValidUntil == null);
            if(baseprice == null)
            {
                baseprice = new Price(prices[0]);
                baseprice.ValidFrom = new DateTime(1970, 1, 1, 0, 0, 0);
                baseprice.ValidUntil = null;
                baseprice.UnitPrice = 0;
            }
            //Add the base price
            priceHistory.Add(baseprice);
            
            if(baseprice.UnitPrice > 0)
            {
                //Remove all prices above the base price as theyd never be the cheapest
                prices.RemoveAll(price => price.UnitPrice >= priceHistory[0].UnitPrice);
            }

            for (int i = 0; i < prices.Count; i++)
            {
                Price newPrice = prices[i];
                //Index of prices to remove from the list as theyre overshadowed by a cheaper one
                List<int> toRemove = new List<int>();
                for (int j = 0; j < priceHistory.Count; j++)
                {
                    Price oldPrice = priceHistory[j];
                    //Create a bunch of bools for readability
                    bool startsBeforeNew = oldPrice.ValidFrom < newPrice.ValidFrom;
                    bool startsDuringNew = startsBeforeNew ? false : oldPrice.ValidFrom < newPrice.ValidUntil;
                    bool endsAfterNew = oldPrice.ValidUntil == null || oldPrice.ValidUntil > newPrice.ValidUntil;
                    bool endsdDuringNew = endsAfterNew ? false : oldPrice.ValidUntil > newPrice.ValidFrom;
                    if (startsBeforeNew && endsAfterNew)
                    {
                        //The old price gets split in two by the new cheaper one
                        Price oldCopy = new Price(oldPrice);
                        //Cut the end of the first piece
                        oldPrice.ValidUntil = newPrice.ValidFrom;
                        //Cut the start of the second piece
                        oldCopy.ValidFrom = (DateTime)newPrice.ValidUntil;
                        priceHistory.Add(oldCopy);
                    }
                    else if (startsBeforeNew && endsdDuringNew)
                    {
                        //Cut the end of the old price to make room for the cheaper one
                        oldPrice.ValidUntil = newPrice.ValidFrom;
                    }
                    else if (endsAfterNew && startsDuringNew)
                    {
                        //Cut the start of the old price to make room for the cheaper one
                        oldPrice.ValidFrom = (DateTime)newPrice.ValidUntil;
                    }
                    else if (startsDuringNew && endsdDuringNew)
                    {
                        //Old price is completely overshadowed
                        toRemove.Add(j);
                    }
                }
                //Remove each price thats overshadowed
                foreach (int ri in toRemove)
                    priceHistory.RemoveAt(ri);
                priceHistory.Add(newPrice);
            }

            //Sort the prices by date
            priceHistory.Sort((a, b) => a.ValidFrom.CompareTo(b.ValidFrom));
            
            return priceHistory;
        }

        //Gets all prices for a product and formats them into a timeline per market
        public List<Price> GetProductPriceHistory(string entryCode)
        {
            //Prices are ordered by price descending
            List<Price> prices = dbHandler.GetProductPrices(entryCode);

            //Gets the market-currency pairs available
            List<MarketCurrencyPair> mcPairs = dbHandler.GetProductMarketCurrencyPairs(entryCode);

            List<Price> timeline = new List<Price>();
            
            foreach(MarketCurrencyPair MCPair in mcPairs)
            {
                //Make timelines for each market-currency pair
                IEnumerable<Price> filtered = prices.Where(price => price.MarketId == MCPair.MarketId && price.CurrencyCode == MCPair.CurrencyCode);
                List<Price> marketTimeline = CreateTimeline(filtered.ToList());
                timeline.AddRange(marketTimeline);
            }
            
            return timeline;
        }
    }
}