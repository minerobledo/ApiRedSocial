using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.ResponPattern
{
    public class Response<T> 
    {
        public bool? succeeded { get; set; }
        public string? message { get; set; }
        public List<Exception>? errors { get; set; }
        public T? data { get; set; }

        public Response()
        {

        }

        public Response(T data, string message)
        {
            succeeded = true;
            this.message = message;
            this.data = data;
        }
        public Response(string message)
        {
            succeeded = false;
            this.message = message;
        }

    }
}
