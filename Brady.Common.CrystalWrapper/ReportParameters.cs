using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    [Guid("AA5A3C62-AF3C-452B-8A6A-A0BAB22F8875")]
    public class ReportParameters : IEnumerable<ReportParameter>
    {
        private List<ReportParameter> _parameters;

        public ReportParameters()
        {
            _parameters = new List<ReportParameter>();
        }

        public void Add(string name, ReportParameterType type, object value)
        {
            _parameters.Add(new ReportParameter(name, type, value));
        }

        public void Clear()
        {
            _parameters.Clear();
        }

        public void Remove(int index)
        {
            _parameters.RemoveAt(index);
        }

        public ReportParameter this[int index]
        {
            get { return _parameters[index]; }
        }

        public IEnumerator<ReportParameter> GetEnumerator()
        {
            return ((IEnumerable<ReportParameter>)_parameters).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ReportParameter>)_parameters).GetEnumerator();
        }
    }
}