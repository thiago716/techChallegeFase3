using Application.Service.Base;
using Application.Service.Interface;
using Application.ViewModel;
using Core.Entity;
using Microsoft.Extensions.Options;

namespace Application.External
{
    public class DirectDistanceDialingExternalService: BaseService<DirectDistanceDialing>, IDirectDistanceDialingService
    {
        public DirectDistanceDialingExternalService(HttpClient httpClient, IOptions<ApiSettings> options)
            : base(httpClient, options.Value.ContactStorageUrl)
        {
        }
    }

}
