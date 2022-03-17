using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using ConnectionManagerDll;


namespace Snipers
{
    public class DatabaseProcessKiller
    {
        public BindingList<DatabaseProcess> Processes { get; private set; }
        public ThreadSafeConnection Connection { get; private set; }
        public ColumnSort Sorting { get; private set; }

        /// <summary>
        /// Creates new instance of DatabaseProcessKiller.
        /// </summary>
        /// <param name="connection">A DB connection</param>
        public DatabaseProcessKiller(ThreadSafeConnection connection)
        {
            this.Sorting = new ColumnSort("Spid");
            this.Connection = connection;
            this.Processes = new BindingList<DatabaseProcess>();
        }


        /// <summary>
        /// Load all processes on the database.
        /// </summary>
        public void Load(string filter = "")
        {
            this.Processes.Clear();
            bool hasFilter = (filter != null);
            filter = (filter == null ? null : filter.ToUpper());

            DataTable result = Connection.ExecuteDataTable("sp_who2");

            foreach (DataRow xRow in result.Rows)
            {
                DatabaseProcess record = new DatabaseProcess();
                record.Spid = int.Parse(xRow["SPID"].ToString());
                record.Database = xRow["DBName"].ToString();
                record.HostName = xRow["HostName"].ToString();


                if (record.HostName.Trim() == ".")   // Não exibir processos do servidor (processos do servidor não podem ser mortos):
                {
                    continue;
                }

                if (hasFilter)
                {
                    if (record.Spid.ToString().Contains(filter)
                        || record.HostName.ToUpper().Contains(filter)
                        || record.Database.ToUpper().Contains(filter))
                    {
                        this.Processes.Add(record);
                    }
                }
                else
                {
                    this.Processes.Add(record);
                }
            }
        }

        public void OrderBy(string orderBy = "Spid", SortDirection direction = SortDirection.Ascendant)
        {
            List<DatabaseProcess> list = new List<DatabaseProcess>();

            this.Sorting.ColumnName = orderBy;
            this.Sorting.Direction = direction;

            foreach (DatabaseProcess item in this.Processes)
            {
                list.Add(item);
            }

            if (orderBy.ToUpper() == "HOSTNAME")
            {
                if (direction == SortDirection.Ascendant)
                {
                    list = list.OrderBy(x => x.HostName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HostName).ToList();
                }
            }
            else if (orderBy.ToUpper() == "DATABASE")
            {
                if (direction == SortDirection.Ascendant)
                {
                    list = list.OrderBy(x => x.Database).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Database).ToList();
                }
            }
            else
            {
                if (direction == SortDirection.Ascendant)
                {
                    list = list.OrderBy(x => x.Spid).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Spid).ToList();
                }
            }

            this.Processes.Clear();

            foreach (DatabaseProcess item in list)
            {
                this.Processes.Add(item);
            }
        }


        /// <summary>
        /// Kill the specified SPID, if isn't already killed.
        /// </summary>
        /// <param name="spid">Process ID</param>
        public void Kill(int spid)
        {
            try
            {
                Connection.ExecuteNonQuery("kill " + spid);
            }
            catch (Exception ex)
            { }
            

            // Verificar se o spid está morto:
            DataTable tblProcesses = Connection.ExecuteDataTable("sp_who2");
            bool spidIsDead = true;

            foreach (DataRow xRow in tblProcesses.Rows)
            {
                if (int.Parse(xRow["spid"].ToString()) == spid)
                {
                    spidIsDead = false;
                    break;
                }
            }

            // Se SPID estiver morto, o remove do GridView:
            if (spidIsDead)
            {
                DatabaseProcess proc = this.Processes.Where(x => x.Spid == spid).FirstOrDefault();
                if (proc != null)
                {
                    this.Processes.Remove(proc);
                }
            }
        }

        /// <summary>
        /// Kill the specified SPID, if isn't already killed.
        /// </summary>
        /// <param name="process">A database process</param>
        public void Kill(DatabaseProcess process)
        {
            Kill(process.Spid);
        }

    }
}
