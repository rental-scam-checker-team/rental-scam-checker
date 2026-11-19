

using System;
using System.Collections.Generic;
using System.Globalization;

namespace RSC.Classes
{
    public class PriceBenchmark
    {
        public int BenchmarkId { get; set; }

        public string City { get; set; } = string.Empty;

        public decimal? PrivateRentPerM2 { get; set; }

        public decimal? SocialRentPerM2 { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DataDate { get; set; }

        public string SourceReference { get; set; } = string.Empty;

        /// <summary>
        /// Validates the values entered in the form.
        /// Returns an empty string when all values are valid.
        /// Otherwise, returns all validation errors.
        /// </summary>
        public string Valid(
            string city,
            string privateRent,
            string socialRent,
            string dataDate,
            string sourceReference)
        {
            var errors = new List<string>();

            // Allowed cities
            string[] allowedCities =
            {
                "Copenhagen",
                "Aarhus",
                "Odense",
                "Aalborg"
            };

            // City: required and must be an allowed city
            city = (city ?? "").Trim();

            if (city.Length == 0)
            {
                errors.Add("City is required.");
            }
            else if (!Array.Exists(
                allowedCities,
                c => string.Equals(
                    c,
                    city,
                    StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add(
                    "City must be Copenhagen, Aarhus, Odense or Aalborg.");
            }

            // Private rent: optional, but must be a positive number
            // when provided
            privateRent = (privateRent ?? "").Trim();

            if (privateRent.Length > 0)
            {
                if (!decimal.TryParse(
                    privateRent,
                    NumberStyles.AllowDecimalPoint |
                    NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture,
                    out decimal privateValue))
                {
                    errors.Add("Private rent must be a valid number.");
                }
                else if (privateValue <= 0)
                {
                    errors.Add("Private rent must be greater than zero.");
                }
            }

            // Social rent: optional, but must be a positive number
            // when provided
            socialRent = (socialRent ?? "").Trim();

            if (socialRent.Length > 0)
            {
                if (!decimal.TryParse(
                    socialRent,
                    NumberStyles.AllowDecimalPoint |
                    NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture,
                    out decimal socialValue))
                {
                    errors.Add("Social rent must be a valid number.");
                }
                else if (socialValue <= 0)
                {
                    errors.Add("Social rent must be greater than zero.");
                }
            }

            // Data date: required, must use yyyy-MM-dd,
            // and cannot be in the future
            dataDate = (dataDate ?? "").Trim();

            if (!DateTime.TryParseExact(
                dataDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
            {
                errors.Add("Data date must be a valid date (yyyy-MM-dd).");
            }
            else if (parsedDate.Date > DateTime.Today)
            {
                errors.Add("Data date cannot be in the future.");
            }

            // Source reference: required, maximum 200 characters
            sourceReference = (sourceReference ?? "").Trim();

            if (sourceReference.Length == 0)
            {
                errors.Add("Source reference is required.");
            }
            else if (sourceReference.Length > 200)
            {
               errors.Add("Source reference must be 200 characters or less.");
            }

            return string.Join(" ", errors);
        }
    }
}
