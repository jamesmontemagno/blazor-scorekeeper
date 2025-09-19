using MyScoreBoardShared.Services;

namespace MyScoreBoard.Services
{
    public class PlatformService : IPlatformService
    {
        public bool IsWebAssembly => true;
        public bool IsMaui => false;
    }
}