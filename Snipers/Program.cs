using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snipers
{
    static class Program
    {
        public static bool OcultarProcessosDosDesenvolvedores { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Program.OcultarProcessosDosDesenvolvedores = !System.Diagnostics.Debugger.IsAttached;

            
            foreach (string item in args)
            {
                if (item.ToUpper().Trim() == "OCULTAR")
                {
                    Program.OcultarProcessosDosDesenvolvedores = false;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
