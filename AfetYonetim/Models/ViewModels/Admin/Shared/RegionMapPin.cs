namespace AfetYonetim.Models.ViewModels.Admin.Shared
{
    public class RegionMapPin
    {
        public string Name { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string RiskLevel { get; set; } = string.Empty;  // Dusuk/Orta/Yuksek/Kritik
    }
}