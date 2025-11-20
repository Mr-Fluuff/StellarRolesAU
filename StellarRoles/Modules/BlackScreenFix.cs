using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace StellarRoles
{
    public class BlackScreenFix
    {
        public static void BeginFix()
        {
            // Construct the path to the settings file
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "InnerSloth\\Among Us\\settings.amogus");

            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read the file content
                    string content = File.ReadAllText(filePath);

                    // Modify the content as needed
                    string modifiedContent = ModifyContent(content);

                    // Write the modified content back to the file
                    File.WriteAllText(filePath, modifiedContent);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur
                Helpers.Log($"An error occurred: {ex.Message}");
            }
        }

        static string ModifyContent(string content)
        {
            // Parse the JSON content
            JObject jsonObject = JObject.Parse(content);

            // Remove the specified properties
            var multiplayer = jsonObject["multiplayer"];
            var gameplay = jsonObject["gameplay"];

            if ((string)multiplayer["normalHostOptions"] != ""
                || (string)multiplayer["normalSearchOptions"] != ""
                || (string)multiplayer["hideNSeekHostOptions"] != ""
                || (string)multiplayer["hideNSeekSearchOptions"] != "")
            {
                multiplayer["normalHostOptions"]?.Replace(new JValue(""));
                multiplayer["normalSearchOptions"]?.Replace(new JValue(""));
                multiplayer["hideNSeekHostOptions"]?.Replace(new JValue(""));
                multiplayer["hideNSeekSearchOptions"]?.Replace(new JValue(""));
                gameplay["screenShake"]?.Replace(new JValue("false"));
            }
            // Serialize the modified JSON object back to a string
            return jsonObject.ToString();
        }
    }
}
