using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;


namespace backtestAutomationZenbot
{

    public class Backtest
    {
        private  string zenbotDir = "/home/floozie/crypto/zenbot";
        private  string zenbotAppName = "zenbot";
        private  string selector = "binance.BTC-USDT";

        private string resultFolder;
        //zenbot backfill binance.BTC-USDT --start="1367272800000" --end="1601140378000"
        //30.April 2013 bitcoin start 1367272800
        // today 1601140378
        public Backtest(string zenbotDir,string zenbotAppName,string selector, string resultFolder)
        {
            this.selector = selector;
            this.zenbotAppName = zenbotAppName;
            this.zenbotDir = zenbotDir;
            this.resultFolder = resultFolder;

        }
        // run a simulation on backfilled data
            /*Options:
                --conf <path>                         path to optional conf overrides file
                --strategy <name>                     strategy to use (default: "trend_ema")
                --order_type <type>                   order type to use (maker/taker) 
                                                        (default: "maker")
                --reverse                             use this and all your signals(buy/sell) 
                                                        will be switch! TAKE CARE! (default: 
                                                        false)
                --filename <filename>                 filename for the result output (ex: 
                                                        result.html). "none" to disable
                --start <datetime>                    start ("YYYYMMDDhhmm")
                --end <datetime>                      end ("YYYYMMDDhhmm")
                --days <days>                         set duration by day count (default: 14)
                --currency_capital <amount>           amount of start capital in currency 
                                                        (default: 1000)
                --asset_capital <amount>              amount of start capital in asset 
                                                        (default: 0)
                --avg_slippage_pct <pct>              avg. amount of slippage to apply to 
                                                        trades (default: 0.045)
                --buy_pct <pct>                       buy with this % of currency balance 
                                                        (default: 99)
                --sell_pct <pct>                      sell with this % of asset balance 
                                                        (default: 99)
                --markdown_buy_pct <pct>              % to mark down buy price (default: 0)
                --markup_sell_pct <pct>               % to mark up sell price (default: 0)
                --order_adjust_time <ms>              adjust bid/ask on this interval to keep 
                                                        orders competitive (default: 5000)
                --order_poll_time <ms>                poll order status on this interval 
                                                        (default: 5000)
                --sell_cancel_pct <pct>               cancels the sale if the price is 
                                                        between this percentage (for more or 
                                                        less)
                --sell_stop_pct <pct>                 sell if price drops below this % of 
                                                        bought price (default: 0)
                --buy_stop_pct <pct>                  buy if price surges above this % of 
                                                        sold price (default: 0)
                --profit_stop_enable_pct <pct>        enable trailing sell stop when reaching 
                                                        this % profit (default: 0)
                --profit_stop_pct <pct>               maintain a trailing stop this % below 
                                                        the high-water mark of profit (default: 
                                                        1)
                --max_sell_loss_pct <pct>             avoid selling at a loss pct under this 
                                                        float (default: 99)
                --max_buy_loss_pct <pct>              avoid buying at a loss pct over this 
                                                        float (default: 99)
                --max_slippage_pct <pct>              avoid selling at a slippage pct above 
                                                        this float (default: 5)
                --symmetrical                         reverse time at the end of the graph, 
                                                        normalizing buy/hold to 0 (default: 
                                                        false)
                --rsi_periods <periods>               number of periods to calculate RSI at 
                                                        (default: 14)
                --exact_buy_orders                    instead of only adjusting maker buy 
                                                        when the price goes up, adjust it if 
                                                        price has changed at all
                --exact_sell_orders                   instead of only adjusting maker sell 
                                                        when the price goes down, adjust it if 
                                                        price has changed at all
                --disable_options                     disable printing of options
                --quarentine_time <minutes>           For loss trade, set quarentine time for 
                                                        cancel buys
                --enable_stats                        enable printing order stats
                --backtester_generation <generation>  creates a json file in simulations with 
                                                        the generation number (default: -1)
                --verbose                             print status lines on every period
                --silent                              only output on completion (can speed up 
                                                        sim)
                -h, --help                            output usage information
            */


        public BacktestResult start(string strategy, string OrderType, DateTime start, DateTime end,double sell_stop_pct, int quarentine_time, string strategyDependentParams)
        {
            BacktestResult bt = new BacktestResult();
            if(!resultFolder.EndsWith("/"))
            {
                resultFolder = resultFolder +"/";
            }
            string resultFile = resultFolder+strategy+OrderType+"-"+start.ToString("yyyyMMddHHmm")+"-"+end.ToString("yyyyMMddHHmm")+"sell_stop_pct"+sell_stop_pct.ToString().Replace(".","_")+"-quarentine_time"+quarentine_time.ToString()+strategyDependentParams.Replace(" ","-").Replace(".","_")+".html";

            using (Process process = new Process())
            {
                string agruments = "sim "+selector+" --strategy "+strategy+" --OrderType "+OrderType+" --start "+start.ToString("yyyyMMddHHmm")+" --end "+end.ToString("yyyyMMddHHmm")+" --filename "+resultFile;
                if(sell_stop_pct!=0.0)
                {
                    agruments+=" --sell_stop_pct "+sell_stop_pct.ToString();
                }
                if(quarentine_time!=0)
                {
                    agruments+=" --quarentine_time "+quarentine_time.ToString();
                }
                if(!string.IsNullOrEmpty(strategyDependentParams))
                {
                    agruments+=" "+strategyDependentParams;
                }
                
                
                
                process.StartInfo.FileName = zenbotAppName;
                process.StartInfo.WorkingDirectory = zenbotDir;
                process.StartInfo.Arguments = agruments;
                
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                Console.WriteLine(zenbotAppName+" " +process.StartInfo.Arguments);
                process.Start();

                // Synchronously read the standard output of the spawned process.
                StreamReader reader = process.StandardOutput;
                
                
                
                //string stream = reader.ReadToEnd();
                string stream ="";
                string line ="";
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    stream += line;
                }
                //Console.WriteLine(stream);
               
                bt.resultFile = resultFile;
                bt.buyHold = KeywordExtractor.getPercent("buy hold:",stream);
                bt.endBalance = KeywordExtractor.getPercent("end balance:",stream);

               
                process.WaitForExit();
            }
            return bt;
        }
        
    }
}