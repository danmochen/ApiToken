using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class StreamHelper
    {
        
        public static string ReadEnd(this Stream source)
        {
            StreamReader reader = new StreamReader(source, Encoding.UTF8);

            return reader.ReadToEnd();

        }
        public static async Task<string> ReadEndAsync(this Stream source)
        {
            StreamReader reader = new StreamReader(source, Encoding.UTF8);

            return await reader.ReadToEndAsync();

        }
    }
}
