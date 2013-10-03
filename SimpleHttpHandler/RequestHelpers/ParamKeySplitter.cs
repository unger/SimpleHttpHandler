using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpHandler.RequestHelpers
{
    public class ParamKeySplitter
    {
        public string[] SplitKey(string key)
        {
            var result = key.Split(new string[] { "[", "." }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = result[i].Trim(new[] { '[', ']' });
            }

            return result;
        }
    }
}
