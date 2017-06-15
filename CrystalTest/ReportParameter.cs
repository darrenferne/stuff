﻿using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    public class ReportParameter
    {
        private string _name;
        private ReportParameterType _parameterType;
        private List<object> _values;

        public ReportParameter()
        {
            _values = new List<object>();
        }

        public ReportParameter(string name, ReportParameterType type, object value = null)
            : this()
        {
            Name = name;
            ParameterType = type;
            AddValue(value);
        }

        public void AddValue(object value)
        {
            _values.Add(value);
        }

        public void ClearValues()
        {
            _values.Clear();
        }

        public void RemoveValue(int index)
        {
            _values.RemoveAt(index);
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ReportParameterType ParameterType
        {
            get { return _parameterType; }
            set { _parameterType = value; }
        }

        public object GetValue(int index)
        {
            return _values[index];
        }

        public void SetValue(int index, object value)
        {
            _values[index] = value;
        }

        public int ValueCount
        {
            get { return _values.Count; }
        }
    }
}