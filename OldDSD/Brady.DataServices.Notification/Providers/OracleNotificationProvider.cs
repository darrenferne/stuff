using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace Brady.DataServices.Notification
{
    public class OracleNotificationProvider : NotificationProvider<OracleNotificationHandler>
    {
        public OracleNotificationProvider()
            : base()
        { }

        public OracleNotificationProvider(string connectionString)
            : base(connectionString)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                }
            }
            catch
            {
                throw new ArgumentException("Invalid connection");
            }
        }
    }
}
