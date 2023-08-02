namespace Magisterska_backend.Services.Interfaces
{
    public interface IInfluxDbSettings
    {
        string DbUrl { get; set; }
        string DbToken { get; set; }
        string DbBucket { get; set; }
        string DbOrg { get; set; }
    }
}
