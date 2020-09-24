using System.Threading.Tasks;
using ApiTemplate.Values.PublicApi.Models;

namespace ApiTemplate.Values.PublicApi.HttpClients
{
    public interface IValuesApiClient
    {
        Task<ValueItem> GetValueItem(string key);
        Task PutValueItem(string key, ValueItem item);

        Task PostValueItem(ValueItem item);
    }
}
