using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository.CustomTypes
{
    public class NumericTrueFalse : IUserType
    {
        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x == null ? typeof(bool).GetHashCode() + 473 : x.GetHashCode();
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var obj = NHibernateUtil.Int32.NullSafeGet(rs, names[0]);

            if (obj == null)
                return null;

            var valueAsNumber = (int)obj;

            return valueAsNumber != 0;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {
                var valueAsBool = (bool)value;
                int valueAsNumber = valueAsBool == true ? -1 : 0;
                ((IDataParameter)cmd.Parameters[index]).Value = valueAsNumber;
            }
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public Type ReturnedType
        {
            get { return typeof(bool); }
        }

        public SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.Int32.SqlType }; }
        }
    }
}
