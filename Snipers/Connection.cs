using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConnectionManagerDll;

namespace Snipers
{
    public static class Connection
    {
        public static ThreadSafeConnection Connect(string nomeServidor, string nomeBaseDados = "master")
        {

            List<String> CommandsList;
            string StrConexao;


            CommandsList = new List<String>();
            CommandsList.Add("SET LANGUAGE 'Português (Brasil)'");
            CommandsList.Add("SET LOCK_TIMEOUT 5000");

            nomeBaseDados = nomeBaseDados.Trim();
            nomeServidor = nomeServidor.Trim();

            StrConexao = "Initial Catalog=" + nomeBaseDados + ";" +
                         "Data Source=" + nomeServidor + ";" +
                         "User ID=sistema;" +
                         "Password=schwer_wissen;" +
                         "Connect Timeout=2;" +
                         "Application Name='Snipers'";


            ConnectionManager.CreateInstance(StrConexao, CommandsList);
            return ConnectionManager.GetConnection;
        }
    }
}