using Aplication.Interfaces.Repository;
using AutoMapper.Configuration.Annotations;
using Domain.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Reports.Query.GetReportByFilter
{
    internal class GetReportByFilterQuerryHandler: IRequestHandler<GetReportByFilterQuerry,Response<List<Report>>>
    {
       private readonly IReportRepository _reportRepository;

        public GetReportByFilterQuerryHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Response<List<Report>>> Handle(GetReportByFilterQuerry request, CancellationToken cancellationToken)
        {
            try
            {
                Dictionary<string, object> Truefilter = new Dictionary<string, object>();
                foreach (var filter in request.Filter)
                {
                    if(filter.Value != null)
                    {
                        switch (filter.Key)
                        {
                            case "State":
                                // code block
                                Truefilter.Add(filter.Key,filter.Value);
                                break;
                            case "Type":
                                // code block
                                Truefilter.Add(filter.Key, filter.Value);
                                break;

                            case "ReporterProfileName":
                                Truefilter.Add(filter.Key, filter.Value);
                                break;
                            case "ReportedProfileName":
                                Truefilter.Add(filter.Key, filter.Value);
                                break;

                            default:
                                // code block
                            
                                break;
                        }
                    }
                }
                   

                var a = await _reportRepository.GetReportByFilterAsync(Truefilter, request.StartAfter);
                return new Response<List<Report>> { succeeded = true, data = a };
            }catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
                return new Response<List<Report>>
                {
                    succeeded = false,
                    errors = new List<Exception> { ex }
                };
            }
           
        }
    }
}
