using System.Collections.Generic;

namespace QnABot
{
    public class ItemClass
    {
        public List<Investment> items { get; set; }
    }

    public class ItemDetails
    {
        public Investment items { get; set; }
    }

    public class Investment
    {
        public string id { get; set; }
        public string name { get; set; }
        public string prgElig { get; set; }
        public string prdTyp { get; set; }
        public string subTyp { get; set; }
        public string strtTyp { get; set; }
        public string strtStyle { get; set; }
        public string isAlts { get; set; }
        public string isMuni { get; set; }
        public string rsk { get; set; }
        public string rskScore { get; set; }
        public string minInvAmt { get; set; }
        public string trlRtnYTD { get; set; }
        public string trlRtn1MO { get; set; }
        public string trlRtn3MO { get; set; }
        public string trlRtn1Y { get; set; }
        public string trlRtn3Y { get; set; }
        public string trlRtn5Y { get; set; }
        public string trlRtn10Y { get; set; }
        public string trlRtnInc { get; set; }
        public List<Product> products { get; set; }


    }

    public class Product
    {
        public string id { get; set; }
        public string name { get; set; }
        public string alloc { get; set; }
        public string asset_cls_lvl1 { get; set; }
        public string asset_cls_lvl2 { get; set; }
        public string asset_cls_lvl3 { get; set; }
    }


    public class Stockobject
    {
        public string ticker { get; set; }
        public int queryCount { get; set; }
        public int resultsCount { get; set; }
        public bool adjusted { get; set; }
        public Result[] results { get; set; }
        public string status { get; set; }
        public string request_id { get; set; }
        public int count { get; set; }
    }

    public class Result
    {
        public string T { get; set; }
        public long v { get; set; }
        public float vw { get; set; }
        public float o { get; set; }
        public float c { get; set; }
        public float h { get; set; }
        public float l { get; set; }
        public long t { get; set; }
        public int n { get; set; }
    }


    public class Tickerobject
    {
        public TResults results { get; set; }
        public string status { get; set; }
        public string request_id { get; set; }
    }

    public class TResults
    {
        public string ticker { get; set; }
        public string name { get; set; }
        public string market { get; set; }
        public string locale { get; set; }
        public string primary_exchange { get; set; }
        public string type { get; set; }
        public bool active { get; set; }
        public string currency_name { get; set; }
        public string cik { get; set; }
        public string composite_figi { get; set; }
        public string share_class_figi { get; set; }
        public long market_cap { get; set; }
        public string phone_number { get; set; }
        public Address address { get; set; }
        public string description { get; set; }
        public string sic_code { get; set; }
        public string sic_description { get; set; }
        public string ticker_root { get; set; }
        public string homepage_url { get; set; }
        public int total_employees { get; set; }
        public string list_date { get; set; }
        public Branding branding { get; set; }
        public long share_class_shares_outstanding { get; set; }
        public long weighted_shares_outstanding { get; set; }
    }


    public class Address
    {
        public string address1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
    }

    public class Branding
    {
        public string logo_url { get; set; }
        public string icon_url { get; set; }
    }


    public class StockDisplay
    {
        public string symbol { get; set; }
        public string companyName { get; set; }
        public string open { get; set; }
        public string close { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string latestPrice { get; set; }
        public string latestUpdate { get; set; }
    }


}
