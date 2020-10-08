using System.Threading.Tasks;
using Optional;

namespace ApiTemplate.Values.Domain.Proxies
{
    public interface INumbersProxy
    {
        Task<Option<int>> Get();
    }
}