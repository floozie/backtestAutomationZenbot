using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;


namespace backtestAutomationZenbot
{

    public class Backfill
    {
        private  string zenbotDir = "/home/floozie/crypto/zenbot";
        private  string zenbotAppName = "zenbot";
        private  string selector = "binance.BTC-USDT";
        //zenbot backfill binance.BTC-USDT --start="1367272800000" --end="1601140378000"
        //30.April 2013 bitcoin start 1367272800
        // today 1601140378
        public Backfill(string zenbotDir,string zenbotAppName,string selector)
        {
            this.selector = selector;
            this.zenbotAppName = zenbotAppName;
            this.zenbotDir = zenbotDir;

        }
        public static string DateTimeToUnixTimestamp(DateTime dateTime)
        {
            string unixTime = (TimeZoneInfo.ConvertTimeToUtc(dateTime) - 
                new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds.ToString();
                unixTime = unixTime.Substring(0,unixTime.IndexOf("."));
                return unixTime;
        }
        public void start()
        {
            Process process = new Process();
            process.StartInfo.FileName = zenbotAppName;
            process.StartInfo.WorkingDirectory = zenbotDir;
            process.StartInfo.Arguments = "backfill "+selector+" --start=\"1367272800000\" --end=\""+DateTimeToUnixTimestamp(System.DateTime.Now)+"\"";
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
        }
    }
}