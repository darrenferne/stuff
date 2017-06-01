using System.Collections;
using System.Collections.Generic;

namespace WindowsFormsApplication1
{
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