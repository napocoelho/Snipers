using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Snipers
{
    public static class Extensions
    {
        public static bool Contains(this RegistryKey rk, string valueName)
        {
            return rk.Contains(valueName, false);
        }

        public static bool Contains(this RegistryKey rk, string valueName, bool caseInsensitive)
        {
            if (caseInsensitive)
            {
                foreach (string name in rk.GetValueNames())
                    if (name.ToLower() == valueName.ToLower())
                        return true;
            }
            else
            {
                foreach (string name in rk.GetValueNames())
                    if (name == valueName)
                        return true;
            }

            return false;
        }

        public static string[] Trim(this string[] array)
        {
            List<string> retorno = new List<string>();

            if (array is null)
                return retorno.ToArray();



            for (int idx = 0; idx < array.Length; idx++)
            {
                if(!string.IsNullOrEmpty( array[idx]) )
                {
                    retorno.Add(array[idx]);
                }
            }

            return retorno.ToArray();            
        }
    }
}