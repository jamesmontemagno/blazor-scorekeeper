namespace MyScoreBoardShared.Services
{
    public interface IPlatformService
    {
        bool IsWebAssembly { get; }
        bool IsMaui { get; }
    }
}