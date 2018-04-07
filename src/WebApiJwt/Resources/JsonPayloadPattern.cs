using Microsoft.AspNetCore.Mvc;

namespace WebApiJwt.Resources
{
    public class JsonPayloadPattern
    {
        public object JsonPayload(bool success, string message, int statusCode, object data)
        {
            var payload = new {
                success = success,
                message = message, 
                statusCode = statusCode, 
                data
            };
            return payload;
        }
    }
}