
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
    public string Valid()
{
    if (string.IsNullOrWhiteSpace(City))
        return "City is required.";

    if (PrivateRentPerM2.HasValue && PrivateRentPerM2.Value <= 0)
        return "Private rent must be greater than zero.";

    if (SocialRentPerM2.HasValue && SocialRentPerM2.Value <= 0)
        return "Social rent must be greater than zero.";

    if (DataDate == default)
        return "Data date is required.";

    if (DataDate.Date > DateTime.Today)
        return "Data date cannot be in the future.";

    if (string.IsNullOrWhiteSpace(SourceReference))
        return "Source reference is required.";

    return string.Empty;
}
}
}
