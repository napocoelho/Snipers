using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snipers
{
    public class DatabaseProcess
    {
        public int Spid { get; set; }
        public string Database { get; set; }
        public string HostName { get; set; }

        public override int GetHashCode()
        {
            return Spid.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            DatabaseProcess other = obj as DatabaseProcess;

            if (other == null)
                return false;

            if (this.Spid == other.Spid)
                return true;

            return false;
        }

        public override string ToString()
        {
            return this.Spid.ToString();
        }
    }
}
