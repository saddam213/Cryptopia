
namespace Cryptopia.IntegrationService.Implementation
{
    public interface IProcessor
    {
        bool Running { get; }
        void Start();
        void Stop();
    }
}
