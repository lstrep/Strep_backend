using Magisterska_backend.Services.Interfaces;

namespace Magisterska_backend.Services
{
    public class InfluxDbSettings : IInfluxDbSettings
    {
        public string DbUrl { get; set; }
        public string DbToken { get; set; }
        public string DbBucket { get; set; }
        public string DbOrg { get; set; }
    }
}
