using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper.Configuration.Annotations;
using Domain.Entities.Chats;
using Domain.Entities.Event;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aplication.Features.Event.Command.UpdateEvent
{
    internal class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Response<bool?>>
    {
        private readonly IAuthService _authService;
        private readonly IEventRepocitory _eventRepocitory;

        public UpdateEventCommandHandler(IAuthService authService, IEventRepocitory eventRepocitory)
        {
            _authService = authService;
            _eventRepocitory = eventRepocitory;
        }

        public async Task<Response<bool?>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            
            try
            {
                if (!string.IsNullOrEmpty(request.Id))
                {
                    var a= new Dictionary<string, object>();
                    foreach(var item in request.Parameters)
                    {
                        if (item.Key == "EventName") 
                        {
                           
                            a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); 
                        }
                        if (item.Key == "Description") { a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); }
                        if (item.Key == "Slogan") { a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); }
                        if (item.Key == "Baner") { a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); }
                        if (item.Key == "GuestLimit") { a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); }
                        if (item.Key == "OrganizerName") { a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); }
                        if (item.Key == "OrganizerPhone") { a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); }
                        if (item.Key == "OrganizerEmail") { a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); }
                        if (item.Key == "Location") { a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); }
                        if (item.Key == "State") { a.Add(item.Key, ConvertSimpleJsonElement((JsonElement)item.Value)); }
                    }
                    var result = await _eventRepocitory.UpdateEvent(a,request.Id);
                    return new Response<bool?> { data = result,succeeded = true };
                }
                    return new Response<bool?> { data = false, succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }

                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
        private object ConvertSimpleJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    if (element.TryGetDateTime(out var date)) return date;
                    return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var l)) return l;
                    if (element.TryGetDouble(out var d)) return d;
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
                case JsonValueKind.Null:
                    return null;
            }
            return element.ToString(); // fallback
        }

    }

}
