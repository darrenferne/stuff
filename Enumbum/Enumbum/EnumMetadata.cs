using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enumbum
{
    public class EnumMetadata<T> where T : struct, IConvertible
    {
        private Dictionary<T, string> _abbreviations;
        private Dictionary<T, string> _displayNames;
        private Dictionary<T, string> _descriptions;

        public EnumMetadata()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Generic type T must be an enumerated type");

            _abbreviations = new Dictionary<T, string>();
            _displayNames = new Dictionary<T, string>();
            _descriptions = new Dictionary<T, string>();
        }

        public void AddAbbreviatedName(T value, string abbreviation)
        {
            if (_abbreviations.ContainsKey(value))
                _abbreviations[value] = abbreviation;
            else
                _abbreviations.Add(value, abbreviation);
        }

        public void AddDisplayName(T value, string displayName)
        {
            if (_displayNames.ContainsKey(value))
                _displayNames[value] = displayName;
            else
                _displayNames.Add(value, displayName);
        }

        public void AddDescription(T value, string description)
        {
            if (_descriptions.ContainsKey(value))
                _descriptions[value] = description;
            else
                _descriptions.Add(value, description);
        }
    }
}
