using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace backtestAutomationZenbot
{
  
    public class BacktestResult
    {
        public string resultFile;
        public string endBalance;
        public string buyHold;
    }
    public class Span
    {
        public Span(DateTime start, DateTime end, string spantype)
        {
            this.spantype = spantype;
            this.start = start;
            this.end = end;
        }
        public string spantype;
        public DateTime start;
        public DateTime end;
    }

}