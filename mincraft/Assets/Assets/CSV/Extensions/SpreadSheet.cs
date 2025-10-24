using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace CSVData.Extensions {
    public static class SpreadSheet {
        
        public class SpreadSheetType {

            public string range;
            public string majorDimension;
            public List<List<string>> values;
        }
        
        public static List<List<string>> LoadData(string path, string sheet, string key) {
            HttpClient httpClient = new();
            string src = $"https://sheets.googleapis.com/v4/spreadsheets/{path}/values/{sheet}?key={key}";
            var rawData = "";
            try {

                rawData = httpClient.GetStringAsync(src).Result;
            }
            catch {
                throw new Exception($"{src} can't load");
            }

            return JsonConvert.DeserializeObject<SpreadSheetType>(rawData).values;
        }
    }
}