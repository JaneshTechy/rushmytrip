using Newtonsoft.Json;

using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TravelWeb.Datamodel;
using TravelWeb.Models;
using TravelWeb.Services;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TravelWeb.Helper
{
    public class AplicationHelper
    {
        //    private static string BasePath =>
        //    Path.Combine(Directory.GetCurrentDirectory(), "SearchStore");
        //    private static readonly string MarkupRulesFile =
        //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "markup_rules.json");

        //    private static readonly string AllowRulesFile =
        //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "allow_rules.json");


        //    private static readonly string BlockRulesFile =
        //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "block_rules.json");


        private static string BasePath =>
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SearchStore");

        private static readonly string MarkupRulesFile =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SearchStore", "markup_rules", "Rush_FS", "markup_rules.json");

        private static readonly string AllowRulesFile =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SearchStore", "markup_rules", "Rush_FS", "allow_rules.json");

        private static readonly string BlockRulesFile =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SearchStore", "markup_rules", "Rush_FS", "block_rules.json");

        //public static string BuildSearchKey(string o, string d, DateTime fromDate, DateTime? toDate, int a, int c, int i, int tripType)
        //{
        //    return $"{o}|{d}|{fromDate:yyyyMMdd}|{toDate:yyyyMMdd}|{a}|{c}|{i}|{tripType}".ToLower();
        //}
        public static string BuildSearchKey(
    string o, string d,
    DateTime fromDate, DateTime? toDate,
    int a, int c, int i,
    int tripType, int flightclass)
        {
            var origin = (o ?? "NA").Trim().ToUpperInvariant();
            var dest = (d ?? "NA").Trim().ToUpperInvariant();

            var dep = fromDate.ToString("yyyyMMdd");
            var ret = toDate.HasValue
                ? toDate.Value.ToString("yyyyMMdd")
                : "OW";

            return $"{origin}|{dest}|{dep}|{ret}|{a}|{c}|{i}|{tripType}|{flightclass}".ToLowerInvariant();
        }

        public static string GetSearchFolder(string searchId)
        {
            string folderPath = Path.Combine(BasePath, $"search_{searchId}");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        public static T Get<T>(string searchId, string fileName)
        {
            string filePath = Path.Combine(
                BasePath,
                $"search_{searchId}",
                $"{fileName}.txt"
            );

            if (!File.Exists(filePath))
                return default;

            try
            {
                string json = ReadAllTextSafe(filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return default;

                return System.Text.Json.JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Log("SearchStore Get error " + fileName + ": " + ex.Message);
                return default;
            }
        }

        public static void Save<T>(string searchId, string fileName, T data)
        {
            string folder = GetSearchFolder(searchId);
            string filePath = Path.Combine(folder, $"{fileName}.txt");

            string json = System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            try
            {
                WriteAllTextSafe(filePath, json);
            }
            catch (Exception ex)
            {
                Log("SearchStore Save error " + fileName + ": " + ex.Message);
            }
        }

        // (memory cache removed — result load must not depend on cache)
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, object> _fileLocks =
            new System.Collections.Concurrent.ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        private static object GetFileLock(string filePath)
            => _fileLocks.GetOrAdd(Path.GetFullPath(filePath), _ => new object());

        /// <summary>
        /// Write via temp file + replace, with retries, to avoid IOException when another
        /// request is reading/writing the same SearchStore file.
        /// </summary>
        public static void WriteAllTextSafe(string filePath, string contents)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var gate = GetFileLock(filePath);
            lock (gate)
            {
                Exception last = null;
                for (var attempt = 0; attempt < 8; attempt++)
                {
                    try
                    {
                        var tmp = filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
                        File.WriteAllText(tmp, contents ?? string.Empty);
                        try
                        {
                            if (File.Exists(filePath))
                                File.Replace(tmp, filePath, null);
                            else
                                File.Move(tmp, filePath);
                        }
                        catch (IOException)
                        {
                            // Fallback when Replace/Move race on Windows
                            File.Copy(tmp, filePath, overwrite: true);
                            try { File.Delete(tmp); } catch { /* ignore */ }
                        }
                        return;
                    }
                    catch (IOException ex)
                    {
                        last = ex;
                        System.Threading.Thread.Sleep(40 + (attempt * 40));
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        last = ex;
                        System.Threading.Thread.Sleep(40 + (attempt * 40));
                    }
                }

                if (last != null)
                    throw last;
            }
        }

        public static string ReadAllTextSafe(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var gate = GetFileLock(filePath);
            lock (gate)
            {
                Exception last = null;
                for (var attempt = 0; attempt < 8; attempt++)
                {
                    try
                    {
                        using (var fs = new FileStream(
                            filePath,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite))
                        using (var reader = new StreamReader(fs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    catch (IOException ex)
                    {
                        last = ex;
                        System.Threading.Thread.Sleep(40 + (attempt * 40));
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        last = ex;
                        System.Threading.Thread.Sleep(40 + (attempt * 40));
                    }
                }

                if (last != null)
                    throw last;
                return null;
            }
        }

        //public static void SaveMarkupRulesToFile(List<MarkupRuleDto> rules)
        //{
        //    try
        //    {
        //        var Jsonconvertdata = JsonSerializer.Serialize(rules);
        //        var dir = Path.GetDirectoryName(MarkupRulesFile);
        //        if (!Directory.Exists(dir))
        //            Directory.CreateDirectory(dir);

        //        var json = JsonSerializer.Serialize(rules);
        //        File.WriteAllText(MarkupRulesFile, json);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error saving markup rules file: " + ex.Message);
        //    }
        //}
        public static void SaveMarkupRulesToFile(List<MarkupRuleDto> rules, string campId)
        {
            try
            {
                Log("SaveMarkupRulesToFile started");

                if (string.IsNullOrWhiteSpace(campId))
                {
                    throw new Exception("CampId is required");
                }

                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "SearchStore",
                    "markup_rules",
                    campId.Trim(),
                    "markup_rules.json"
                );

                var dir = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    Log("Directory created: " + dir);
                }

                var json = JsonConvert.SerializeObject(rules, Formatting.Indented);

                File.WriteAllText(filePath, json);

                Log($"File saved successfully for {campId}. Rules count: {rules?.Count}");
            }
            catch (Exception ex)
            {
                Log("Error saving markup rules file: " + ex.Message);
                Console.WriteLine("Error saving markup rules file: " + ex.Message);
            }
        }

        public static void SaveMarkupRulesToFileV1(List<MarkupRuleDto> rules, string campId)
        {
            try
            {
                Log("SaveMarkupRulesToFileV1 started");

                if (string.IsNullOrWhiteSpace(campId))
                {
                    throw new Exception("CampId is required");
                }
                string filePath;

                if (campId.Equals("Rush", StringComparison.OrdinalIgnoreCase))
                {
                    //filePath = Path.Combine(
                    //    Directory.GetCurrentDirectory(),
                    //    "wwwroot",
                    //    "SearchStore",
                    //    "markup_rules",
                    //    campId,
                    //    "markup_rules.json"
                    //);
                    filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "Rules",
                        "markup_rules",
                        campId,
                        "markup_rules.json"
                    );
                }
                else
                {
                    filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "Rules",
                        "markup_rules",
                        campId,
                        "markup_rules.json"
                    );
                }

                var dir = Path.GetDirectoryName(filePath);


                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    Log("Directory created: " + dir);
                }

                var json = JsonConvert.SerializeObject(rules, Formatting.Indented);

                File.WriteAllText(filePath, json);

                Log($"File saved successfully for {campId}. Rules count: {rules?.Count}");
            }
            catch (Exception ex)
            {
                Log("Error saving markup rules file: " + ex.Message);
                Console.WriteLine("Error saving markup rules file: " + ex.Message);
            }
        }
        public static List<MarkupRuleDto> LoadMarkupRulesFromFile(string campId)
        {
            try
            {
                Log("LoadMarkupRulesFromFile started");

                //var filePath = GetMarkupRulesFile(campId);
                var filePath = GetMarkupRulesFileV1(campId);
                Log("File path: " + filePath);

                if (!File.Exists(filePath))
                {
                    Log("File not found");
                    return new List<MarkupRuleDto>();
                }

                var json = File.ReadAllText(filePath);

                Log("File read successfully");

                var entityList = JsonConvert.DeserializeObject<List<MarkupRuleDto>>(json)
                                 ?? new List<MarkupRuleDto>();

                Log($"Rules loaded: {entityList.Count}");

                return entityList;
            }
            catch (Exception ex)
            {
                Log("Error reading markup rules file: " + ex.Message);
                Console.WriteLine("Error reading markup rules file: " + ex.Message);

                return new List<MarkupRuleDto>();
            }
        }
        public static List<MarkupRuleDto> LoadMarkupRulesFromFileV1(string campId)
        {
            try
            {
                Log("LoadMarkupRulesFromFile started");

                var filePath = GetMarkupRulesFileV1(campId);

                Log("File path: " + filePath);

                if (!File.Exists(filePath))
                {
                    Log("File not found");
                    return new List<MarkupRuleDto>();
                }

                var json = File.ReadAllText(filePath);

                Log("File read successfully");

                var entityList = JsonConvert.DeserializeObject<List<MarkupRuleDto>>(json)
                                 ?? new List<MarkupRuleDto>();

                Log($"Rules loaded: {entityList.Count}");

                return entityList;
            }
            catch (Exception ex)
            {
                Log("Error reading markup rules file: " + ex.Message);
                Console.WriteLine("Error reading markup rules file: " + ex.Message);

                return new List<MarkupRuleDto>();
            }
        }

        public static List<MarkupRuleDto> LoadMarkupRulesFromFileV3(string campId)
        {
            try
            {
                Log("LoadMarkupRulesFromFile started");

                var filePath = GetMarkupRulesFileV3(campId);

                Log("File path: " + filePath);

                if (!File.Exists(filePath))
                {
                    Log("File not found");
                    return new List<MarkupRuleDto>();
                }

                var json = File.ReadAllText(filePath);

                Log("File read successfully");

                var entityList = JsonConvert.DeserializeObject<List<MarkupRuleDto>>(json)
                                 ?? new List<MarkupRuleDto>();

                Log($"Rules loaded: {entityList.Count}");

                return entityList;
            }
            catch (Exception ex)
            {
                Log("Error reading markup rules file: " + ex.Message);
                Console.WriteLine("Error reading markup rules file: " + ex.Message);

                return new List<MarkupRuleDto>();
            }
        }

        private static string GetMarkupRulesFile(string campId)
        {
            var path = Path.Combine(
       Directory.GetCurrentDirectory(),
       "wwwroot",
       "SearchStore",
       "markup_rules",
       campId,
       "markup_rules.json"
   );

            Log("Resolved path: " + path);
            return path;
        }
        private static string GetMarkupRulesFileV1(string campId)
        { //{   if(campId!= "Rush_FC")
        //        campId = "Rush_FS";


            var path = Path.Combine(
       Directory.GetCurrentDirectory(),
       "wwwroot",
       "Rules",
       "markup_rules",
       campId,
       "markup_rules.json"
   );

            Log("Resolved path: " + path);
            return path;
        }
        private static string GetMarkupRulesFileV3(string campId)
        {
            var path = Path.Combine(
       Directory.GetCurrentDirectory(),
       "wwwroot",
       "Rules",
       "markup_rules",
       campId,
       "markup_rules.json"
   );

            Log("Resolved path: " + path);
            return path;
        }

        //public static List<MarkupRuleDto> LoadMarkupRulesFromFile()
        //{
        //    try
        //    {
        //        if (!File.Exists(MarkupRulesFile))
        //            return new List<MarkupRuleDto>();

        //        var json = File.ReadAllText(MarkupRulesFile);

        //        return System.Text.Json.JsonSerializer
        //            .Deserialize<List<MarkupRuleDto>>(json)
        //            ?? new List<MarkupRuleDto>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error reading markup rules file: " + ex.Message);
        //        return new List<MarkupRuleDto>();
        //    }
        //}

        public static void SaveAllowRulesToFile(List<AirlineRoute> rules, string campId)
        {
            if (campId != null)
            {
                try
                {
                    var filePath = GetAllowRulesFile(campId);

                    var dir = Path.GetDirectoryName(filePath);

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var json = System.Text.Json.JsonSerializer.Serialize(rules);

                    File.WriteAllText(filePath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving allow rules: " + ex.Message);
                }
            }

        }


        public static List<AllowAirlineRuleDto> LoadAllowRulesFromFile(string campId)
        {
            try
            {
                var filePath = GetAllowRulesFile(campId);

                if (!File.Exists(filePath))
                    return new List<AllowAirlineRuleDto>();

                var json = File.ReadAllText(filePath);

                var entityList = JsonConvert.DeserializeObject<List<AirlineRoute>>(json)
                                 ?? new List<AirlineRoute>();

                var parsed = new List<AllowAirlineRuleDto>();

                foreach (var r in entityList)
                {
                    if (r.Status != true) 
                        continue;

                    parsed.Add(ParseAllowRule(r));
                }

                return parsed;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading allow rules file: " + ex.Message);
                return new List<AllowAirlineRuleDto>();
            }
        }
        private static AllowAirlineRuleDto ParseAllowRule(AirlineRoute r)
        {
            return new AllowAirlineRuleDto
            {
                RoutesId = r.RoutesId,
                Origins = ParseCsvSet(r.Origin),
                Destinations = ParseCsvSet(r.Destination),
                Status = r.Status == true,
                CreatedAt = r.CreatedAt,
                Company = r.Company ?? "",
                CampID = r.CampId ?? "",
                UpdatedBy = r.UpdatedBy ?? "",
                UpdatedAt = r.UpdatedAt
            };
        }
        private static HashSet<string> ParseCsvSet(string input)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(input)) return set;
            foreach (var p in input.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                set.Add(p.Trim().ToUpperInvariant());
            return set;
        }

        private static string GetAllowRulesFile(string campId)
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "SearchStore",
                "allow_rules",
                campId,
                "allow_rules.json"
            );
        }

        //public static void SaveBlockRulesToFile(List<BlockAirlineRuleDto> rules)
        //{
        //    try
        //    {
        //        var dir = Path.GetDirectoryName(BlockRulesFile);
        //        if (!Directory.Exists(dir))
        //            Directory.CreateDirectory(dir);

        //        var json = System.Text.Json.JsonSerializer.Serialize(rules);
        //        File.WriteAllText(BlockRulesFile, json);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error saving block rules file: " + ex.Message);
        //    }
        //}
        public static void SaveBlockRulesToFile(string campId, TravelWeb.Datamodel.RSFLY _context)
        {
            try
            {
                if (campId != null)
                {
                    var rules = _context.BlockAirlineRules
                                    .Where(x => x.CampId == campId)
                                    .ToList();

                    var filePath = GetBlockRulesFile(campId);

                    var directory = Path.GetDirectoryName(filePath);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);


                    if (File.Exists(filePath))
                        File.Delete(filePath);


                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                    var json = JsonSerializer.Serialize(rules, options);


                    File.WriteAllText(filePath, json);
                }

            }
            catch (Exception ex)
            {
                Log("SaveBlockRulesToFile " + ex.Message);

            }
        }

        public static List<BlockAirlineRuleDto> LoadBlockRulesFromFile(string campId)
        {
            try
            {
                var filePath = GetBlockRulesFile(campId);

                if (!File.Exists(filePath))
                    return new List<BlockAirlineRuleDto>();

                var json = File.ReadAllText(filePath);

                // JSON -> Entity list
                var entityList = JsonConvert.DeserializeObject<List<BlockAirlineRule>>(json)
                                 ?? new List<BlockAirlineRule>();

                // Entity -> DTO
                var parsed = new List<BlockAirlineRuleDto>();
                foreach (var r in entityList)
                    parsed.Add(ParseBlockRule(r));

                return parsed;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading block rules file: " + ex.Message);
                return new List<BlockAirlineRuleDto>();
            }
        }

        public static string GetBlockRulesFile(string campId)
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "SearchStore",
                "block_rules",
                campId,
                "block_rules.json"
            );
        }
        private static BlockAirlineRuleDto ParseBlockRule(BlockAirlineRule r)
        {
            return new BlockAirlineRuleDto
            {
                RuleId = r.RuleId,
                Origins = ParseCsvSet(r.Origin),
                Destinations = ParseCsvSet(r.Destination),
                CabinClassOB = r.CabinClassOb?.Trim(),
                CabinClassIB = r.CabinClassIb?.Trim(),
                TimeOBFrom = ParseTimeInt(r.TimeOb?.ToString()),
                TimeOBTo = ParseTimeInt(r.TimeOb?.ToString()),
                TimeIBFrom = ParseTimeInt(r.TimeIb?.ToString()),
                TimeIBTo = ParseTimeInt(r.TimeIb?.ToString()),
                AdvanceDays = r.AdvanceDays,
                DateFrom = r.DateFrom,
                DateTo = r.DateTo,
                BlockDates = ParseDateCsv(r.BlockDates),
                BlockAirlines = ParseCsvSet(r.AirlineCodes),
                Status = Convert.ToBoolean(r.Status)
            };
        }

        private static int? ParseTimeInt(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (int.TryParse(input.Trim(), out var v)) return v;
            return null;
        }
        private static HashSet<DateTime> ParseDateCsv(string input)
        {
            var set = new HashSet<DateTime>();
            if (string.IsNullOrWhiteSpace(input)) return set;
            foreach (var s in input.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (DateTime.TryParse(s.Trim(), out var d))
                    set.Add(d.Date);
            }
            return set;
        }
        public static string GetApiCachePath(string key)
        {
            var safeKey = key.Replace("|", "_");

            var root = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "api-cache"
            );

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            return Path.Combine(root, safeKey + ".json");
        }
        public static bool IsApiCacheValid(string path)
        {
            if (!File.Exists(path)) return false;

            return (DateTime.UtcNow - File.GetLastWriteTimeUtc(path)).TotalHours <= 2;
        }
        //public static void SaveApiResponse(string path, ApiResponse apiResponse)
        //{
        //    var json = JsonConvert.SerializeObject(apiResponse);
        //    File.WriteAllText(path, json);
        //}
        public static async Task SaveApiResponse(string path, ApiResponse data)
        {
            using var fs = File.Create(path);
            await JsonSerializer.SerializeAsync(fs, data);
        }

        public static ApiResponse LoadApiResponse(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<ApiResponse>(json);
        }
        public static string GetApiRequestCachePath(string key)
        {
            var safeKey = key.Replace("|", "_");

            var root = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "api-cache-request"
            );

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            return Path.Combine(root, safeKey + ".json");
        }
        public static async Task SaveApiRequest(string path, string url)
        {
            await File.WriteAllTextAsync(path, url);
        }


        public static void Log(string message)
        {
            var logPath = Path.Combine(AppContext.BaseDirectory, "logs");

            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            var filePath = Path.Combine(logPath, $"log_{DateTime.Now:yyyyMMdd}.txt");

            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

            File.AppendAllText(filePath, logMessage + Environment.NewLine);
        }

    }
}
