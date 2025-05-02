using Application.Service.Base;
using Application.Service.Interface;
using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.External
{
    public class DirectDistanceDialingExternalService: BaseService<DirectDistanceDialing>, IDirectDistanceDialingService
    {
        public DirectDistanceDialingExternalService(HttpClient httpClient) : base(httpClient, "https://api.exemplo.com/DirectDistanceDialing")
        {
        }
    }

}
