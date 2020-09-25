using System.Threading.Tasks;
using ApiTemplate.Values.Domain.Entities;
using Optional;

namespace ApiTemplate.Values.Domain.Proxies
{
    public interface INumbersProxy
    {
        Task<Option<int>> Get();
    }
}