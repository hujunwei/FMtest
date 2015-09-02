namespace FMtest
{
    public class Facility
    {
        public string FacilityShortCode { get; set; }
        public AzureGeo AzureGeo { get; set; }
        public AzureRegion AzureRegion { get; set; }
    }

    public class AzureGeo
    {
        public string Name { get; set; }
    }

    public class AzureRegion
    {
        public string Name { get; set; }
    }
}
