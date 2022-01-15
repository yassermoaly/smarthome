using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    public class RequesAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public RequesAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;          
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request.Path.Value) || !context.Request.Path.Value.Contains("/api/"))
            {
                await _next(context);
                return;
            }
            string Salt = "d55e7b88-757d-11ec-90d6-0242ac120003";
            context.Request.EnableBuffering();
            using (var requestStream =  _recyclableMemoryStreamManager.GetStream())
            {
                string Hash = context.Request.Headers["AuthorizationHash"];
                await context.Request.Body.CopyToAsync(requestStream);
                string Body = ReadStreamInChunks(requestStream);
                context.Request.Body.Position = 0;
                if (Hash == ComputeSha256Hash(Salt + Body))
                {
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }


            }
        }
        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using (var textWriter = new StringWriter())
            {
                using (var reader = new StreamReader(stream))
                {
                    var readChunk = new char[readChunkBufferLength];
                    int readChunkLength;
                    do
                    {
                        readChunkLength = reader.ReadBlock(readChunk,
                                                           0,
                                                           readChunkBufferLength);
                        textWriter.Write(readChunk, 0, readChunkLength);
                    } while (readChunkLength > 0);
                    return textWriter.ToString();
                }
            }
        }               
    }


}