using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Common
{
    public static class CodeGenerator
    {
        public static string CreateNum()
        {
            Random random = new Random();
            int num = random.Next(10);
            return num.ToString();
        }

        public static string CreateBigAbc()
        {
            //A-Z的 ASCII值为65-90
            Random random = new Random();
            int num = random.Next(65, 91);
            string abc = Convert.ToChar(num).ToString();
            return abc;
        }

        public static string GetCode(int len)
        {
            var rand = new Random();
            var code = "";
            for(int i = 0; i < len; i++)
            {
                var t = rand.Next() % 2;
                code += (t == 0 ? CreateBigAbc() : CreateNum());
            }
            return code;
        }
    }
}
