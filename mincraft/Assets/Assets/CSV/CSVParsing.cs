using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace CSVData {
    public static partial class CSV {
        
        public static List<List<string>> Parse(string rawData) {
            
            string pattern1 = @"(?<=^|\n|\r\n)";
            string pattern2 = @"(?:(?:""(?:(?:[^""]|"""")*)"")|(?:[^"",\r\n]*))";
            string pattern = $@"({pattern1}(?:{pattern2},)*{pattern2})";

            var rows= 
                Regex.Matches(rawData, pattern)
                    .Select(row => row.Value)
                    .Where(row => !string.IsNullOrWhiteSpace(row))
                    .ToList();
            
            return Parse(rows);
        }

        public static List<List<string>> Parse(List<string> rowDatas) {
            
            string pattern = @"(?:^|,)(?:(?:""(?<quoted>(?:[^""]|"""")*)"")|(?<unquoted>[^,""\r\n]*))";
            
            List<List<string>> result = new();
            foreach (var row in rowDatas) {
                
                var newRow = new List<String>();
                var matchs = Regex.Matches(row, pattern);

                foreach (Match match in matchs) {

                    string value = match.Groups["quoted"].Success
                        ? match.Groups["quoted"].Value.Replace(@"""""", @"""")
                        : match.Groups["unquoted"].Value;

                    newRow.Add(value);
                    //Console.WriteLine($"*{value}*");
                }

                result.Add(newRow);
            }

            return result;
        }

    }
}