
using System;
using Xunit;
using RSC.Classes;

namespace RSC.Tests
{
    public class PriceBenchmarkTests
    {
        private readonly PriceBenchmark benchmark = new PriceBenchmark();

        // A valid set of values used as the starting point
        private const string ValidCity = "Copenhagen";
        private const string ValidPrivateRent = "225.00";
        private const string ValidSocialRent = "90.00";
        private const string ValidDate = "2026-07-01";
        private const string ValidSource = "Danmarks Statistik";

        private string Validate(
            string city = ValidCity,
            string privateRent = ValidPrivateRent,
            string socialRent = ValidSocialRent,
            string dataDate = ValidDate,
            string sourceReference = ValidSource)
        {
            return benchmark.Valid(
                city,
                privateRent,
                socialRent,
                dataDate,
                sourceReference);
        }

        // -----------------------------------------
        // VALID INPUT
        // -----------------------------------------

        [Fact]
        public void Valid_ReturnsEmptyString_WhenAllValuesAreCorrect()
        {
            string result = Validate();

            Assert.Equal("", result);
        }

        [Theory]
        [InlineData("Copenhagen")]
        [InlineData("Aarhus")]
        [InlineData("Odense")]
        [InlineData("Aalborg")]
        public void Valid_AcceptsAllAllowedCities(string city)
        {
            string result = Validate(city: city);

            Assert.Equal("", result);
        }

        [Theory]
        [InlineData("copenhagen")]
        [InlineData("AARHUS")]
        [InlineData("Odense")]
        public void Valid_AcceptsDifferentCityCapitalisation(string city)
        {
            string result = Validate(city: city);

            Assert.Equal("", result);
        }

        // -----------------------------------------
        // CITY VALIDATION
        // -----------------------------------------

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Berlin")]
        [InlineData("London")]
        [InlineData("123")]
        public void Valid_ReturnsError_WhenCityIsInvalid(string city)
        {
            string result = Validate(city: city);

            Assert.Contains("City", result);
        }

        // -----------------------------------------
        // PRIVATE RENT VALIDATION
        // -----------------------------------------

        [Theory]
        [InlineData("0")]
        [InlineData("-0.01")]
        [InlineData("-100")]
        public void Valid_ReturnsError_WhenPrivateRentIsNotPositive(
            string privateRent)
        {
            string result = Validate(privateRent: privateRent);

            Assert.Contains("Private rent must be greater than zero.", result);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("12abc")]
        [InlineData("one hundred")]
        public void Valid_ReturnsError_WhenPrivateRentIsNotANumber(
            string privateRent)
        {
            string result = Validate(privateRent: privateRent);

            Assert.Contains("Private rent must be a valid number.", result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Valid_AllowsBlankPrivateRent(string privateRent)
        {
            string result = Validate(privateRent: privateRent);

            Assert.Equal("", result);
        }

        [Theory]
        [InlineData("0.01")]
        [InlineData("0.02")]
        [InlineData("225.00")]
        [InlineData("99999999.99")]
        public void Valid_AcceptsPositivePrivateRent(string privateRent)
        {
            string result = Validate(privateRent: privateRent);

            Assert.Equal("", result);
        }

        // -----------------------------------------
        // SOCIAL RENT VALIDATION
        // -----------------------------------------

        [Theory]
        [InlineData("0")]
        [InlineData("-0.01")]
        [InlineData("-100")]
        public void Valid_ReturnsError_WhenSocialRentIsNotPositive(
            string socialRent)
        {
            string result = Validate(socialRent: socialRent);

            Assert.Contains("Social rent must be greater than zero.", result);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("12abc")]
        [InlineData("one hundred")]
        public void Valid_ReturnsError_WhenSocialRentIsNotANumber(
            string socialRent)
        {
            string result = Validate(socialRent: socialRent);

            Assert.Contains("Social rent must be a valid number.", result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Valid_AllowsBlankSocialRent(string socialRent)
        {
            string result = Validate(socialRent: socialRent);

            Assert.Equal("", result);
        }

        [Theory]
        [InlineData("0.01")]
        [InlineData("0.02")]
        [InlineData("90.00")]
        [InlineData("99999999.99")]
        public void Valid_AcceptsPositiveSocialRent(string socialRent)
        {
            string result = Validate(socialRent: socialRent);

            Assert.Equal("", result);
        }

        // -----------------------------------------
        // DATE VALIDATION
        // -----------------------------------------

        [Fact]
        public void Valid_ReturnsError_WhenDateIsBlank()
        {
            string result = Validate(dataDate: "");

            Assert.Contains("Data date", result);
        }

        [Theory]
        [InlineData("not-a-date")]
        [InlineData("2026-13-01")]
        [InlineData("01-07-2026")]
        [InlineData("2026/07/01")]
        public void Valid_ReturnsError_WhenDateFormatIsInvalid(string date)
        {
            string result = Validate(dataDate: date);

            Assert.Contains("Data date", result);
        }

        [Fact]
        public void Valid_ReturnsError_WhenDateIsInFuture()
        {
            string futureDate = DateTime.Today.AddDays(1)
                .ToString("yyyy-MM-dd");

            string result = Validate(dataDate: futureDate);

            Assert.Contains("cannot be in the future", result);
        }

        [Fact]
        public void Valid_AcceptsTodaysDate()
        {
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            string result = Validate(dataDate: today);

            Assert.Equal("", result);
        }

        [Fact]
        public void Valid_AcceptsPastDate()
        {
            string result = Validate(dataDate: "2020-01-01");

            Assert.Equal("", result);
        }

        // -----------------------------------------
        // SOURCE REFERENCE VALIDATION
        // -----------------------------------------

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Valid_ReturnsError_WhenSourceReferenceIsBlank(
            string source)
        {
            string result = Validate(sourceReference: source);

            Assert.Contains("Source reference is required.", result);
        }

        [Fact]
        public void Valid_AcceptsSourceReferenceOf199Characters()
        {
            string source = new string('A', 199);

            string result = Validate(sourceReference: source);

            Assert.Equal("", result);
        }

        [Fact]
        public void Valid_AcceptsSourceReferenceOf200Characters()
        {
            string source = new string('A', 200);

            string result = Validate(sourceReference: source);

            Assert.Equal("", result);
        }

        [Fact]
        public void Valid_ReturnsError_WhenSourceReferenceExceeds200Characters()
        {
            string source = new string('A', 201);

            string result = Validate(sourceReference: source);

            Assert.Contains("200 characters or less", result);
        }

        // -----------------------------------------
        // MULTIPLE VALIDATION ERRORS
        // -----------------------------------------

        [Fact]
        public void Valid_ReturnsAllErrors_WhenMultipleValuesAreInvalid()
        {
            string result = Validate(
                city: "",
                privateRent: "abc",
                socialRent: "0",
                dataDate: "",
                sourceReference: "");

            Assert.Contains("City is required.", result);
            Assert.Contains("Private rent must be a valid number.", result);
            Assert.Contains("Social rent must be greater than zero.", result);
            Assert.Contains("Data date", result);
            Assert.Contains("Source reference is required.", result);
        }
    }
}
