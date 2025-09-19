using MyScoreBoardShared.Services;

namespace MyScoreBoardMaui.Services
{
    public class PlatformService : IPlatformService
    {
        public bool IsWebAssembly => false;
        public bool IsMaui => true;
    }
}