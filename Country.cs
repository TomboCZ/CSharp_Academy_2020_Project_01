using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;

namespace Project_01
{
    /// <summary>
    /// One imported country and its VAT
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Country" /> class
        /// </summary>
        /// <param name="countryCode">countryCode (e.g. "UK")</param>
        /// <param name="jsonString">json data of this country</param>
        public Country(string countryCode, string jsonString)
        {
            this.TryImport(countryCode, jsonString);
        }

        /// <summary>
        /// Gets or sets country's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets country's standard VAT rate
        /// </summary>
        public double StandardRate { get; set; }

        /// <summary>
        /// Get whether the imported data are valid
        /// </summary>
        /// <returns>true == object has valid data, false == object is invalid</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Name) && !double.IsNaN(this.StandardRate);
        }

        /// <summary>
        /// Prints country's name + VAT
        /// </summary>
        public void Print()
        {
            if (this.StandardRate == double.MinValue)
            {
                Console.WriteLine($"{this.Name,-15}: N/A");
            }
            else
            {
                Console.WriteLine($"{this.Name,-15}: {this.StandardRate:N2}");
            }
        }

        /// <summary>
        /// Tries to import a new country data if possible
        /// </summary>
        /// <param name="countryCode">countryCode (eg.. "UK")</param>
        /// <param name="jsonString">json data of this country</param>
        private void TryImport(string countryCode, string jsonString)
        {
            try
            {
                JsonTemplate countryData = JsonConvert.DeserializeObject<JsonTemplate>(jsonString);
                this.Name = countryData.Country;

                // some countries do not have some rates applicable (there is a "false" string instead of a decimal value) 
                // in this case the value will be recognised as a "minimal" (but nonzero) rather than wrong one
                this.StandardRate = countryData.StandardRate.ToLower() == "false" ? double.MinValue : Convert.ToDouble(countryData.StandardRate, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                this.Name = null;
                this.StandardRate = double.NaN;
                Console.WriteLine($"Country {countryCode} was not imported. ({ex.Message})");
            }
        }

        /// <summary>
        /// Struct template for Json deserializer
        /// </summary>
        private struct JsonTemplate
        {
            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("standard_rate")]
            public string StandardRate { get; set; }
        }
    }
}
