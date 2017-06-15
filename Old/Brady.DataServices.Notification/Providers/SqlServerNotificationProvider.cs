using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class SqlServerNotificationProvider : NotificationProvider<SqlServerNotificationHandler>
    {
        public SqlServerNotificationProvider()
        { }

        public SqlServerNotificationProvider(string connectionString)
            : base(connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
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

        internal override void OnFirstSubscription()
        {
            try
            {
                SqlDependency.Start(base.ConnectionString);
                base.OnFirstSubscription();
            }
            catch (Exception ex)
            {
                if (base._log.IsErrorEnabled)
                    _log.ErrorFormat("Failed to initialise database notification: {0}", ex.Message);
                throw ex;
            }
        }

        internal override void OnFinalUnsubscribe()
        {
            SqlDependency.Stop(base.ConnectionString);
            base.OnFinalUnsubscribe();
        }
    }
}
