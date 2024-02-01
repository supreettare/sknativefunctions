using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace sknativefunctions.plugins
{
    public class CityInfo
    {
        public string City { get; set; }

        [KernelFunction, Description("Get the City Name from the given JSON string")]
        public static string GetCityNameFromJson([Description("The JSON string from which City Name needs to be extrected")] string jsonString)
        {
            try
            {
                if (!string.IsNullOrEmpty(jsonString))
                {
                    CityInfo cityInfo = JsonConvert.DeserializeObject<CityInfo>(jsonString);
                    if (cityInfo != null && cityInfo.City != "Unknown")
                    {
                        return cityInfo.City;
                    }
                    else
                    {
                        return "Invalid or unknown city.";
                    }
                }
                else
                {
                    return "Invalid or unknown city.";
                }
            }
            catch (JsonException)
            {
                return "JSON parsing error.";
            }
        }
    }
}
