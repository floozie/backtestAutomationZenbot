using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace backtestAutomationZenbot
{
    public static class KeywordExtractor
    {
        public static string getPercent(string keyword, string stdOut)
        {
            try{
            int keywordPos = stdOut.IndexOf(keyword);
            string KeywordToEndString = stdOut.Substring(keywordPos);
            int openBracketPos = KeywordToEndString.IndexOf("(");
            string openBracketToEndString = KeywordToEndString.Substring(openBracketPos+1);
            int percentPos = openBracketToEndString.IndexOf("%");
            return openBracketToEndString.Substring(0,percentPos);
            }
            catch{
                return "0.0";
            }
        }
    }
}