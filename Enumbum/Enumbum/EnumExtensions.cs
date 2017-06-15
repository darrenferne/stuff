using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enumbum
{
    public static class EnumExtensions
    {
        public static Dictionary<Type, Dictionary<object, string>> _descriptions;
        public static Dictionary<Type, Dictionary<object, string>> _displayNames;
        public static Dictionary<Type, Dictionary<object, string>> _abbreviations;

        public static void ClearDescriptions<T>() where T : struct, IComparable
        {
            if (_descriptions != null)
            {
                var type = typeof(T);
                if (_descriptions.ContainsKey(type))
                    _descriptions.Remove(type);
            }
        }

        public static void AddDescription<T>(this T value, string description) where T : struct, IComparable
        {
            if (_descriptions == null)
                _descriptions = new Dictionary<Type, Dictionary<object, string>>();

            var type = value.GetType();
            if (!_descriptions.ContainsKey(type))
                _descriptions.Add(type, new Dictionary<object, string>());

            if (!_descriptions[type].ContainsKey(value))
                _descriptions[type].Add(value, description);
            else
                throw new ArgumentException(string.Format("Enum value '{0}' already has a description registered", value.ToString()));
        }

        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            if (_descriptions == null || !_descriptions.ContainsKey(type) || !_descriptions[type].ContainsKey(value))
                throw new ArgumentException(string.Format("There is no description available for Enum value '{0}'", value.ToString()));

            return _descriptions[type][value];
        }

        public static T_enum ParseEnumDescription<T_enum>(string description) where T_enum : struct, IComparable
        {
            var type = typeof(T_enum);
            if (_descriptions == null || !_descriptions.ContainsKey(type))
                throw new ArgumentException(string.Format("There are no descriptions available for the Enum type '{0}'", type.Name));

            var match = _descriptions[type].FirstOrDefault(kvp => string.Compare(kvp.Value, description, true) == 0);

            if (match.Key != null)
                return (T_enum)match.Key;
            else
                throw new ArgumentException(string.Format("Failed to parse enum description '{0}'", description));
        }

        public static void ClearDisplayNames<T>() where T : struct, IComparable
        {
            if (_displayNames != null)
            {
                var type = typeof(T);
                if (_displayNames.ContainsKey(type))
                    _displayNames.Remove(type);
            }
        }

        public static void AddDisplayName<T>(this T value, string displayName) where T : struct, IComparable
        {
            if (_displayNames == null)
                _displayNames = new Dictionary<Type, Dictionary<object, string>>();

            var type = value.GetType();
            if (!_displayNames.ContainsKey(type))
                _displayNames.Add(type, new Dictionary<object, string>());

            if (!_displayNames[type].ContainsKey(value))
                _displayNames[type].Add(value, displayName);
            else
                throw new ArgumentException(string.Format("Enum value '{0}' already has a display name registered", value.ToString()));
        }

        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            if (_displayNames == null || !_displayNames.ContainsKey(type) || !_displayNames[type].ContainsKey(value))
                throw new ArgumentException(string.Format("There is no display name available for Enum value '{0}'", value.ToString()));

            return _displayNames[type][value];
        }

        public static T_enum ParseEnumDisplayName<T_enum>(string displayName) where T_enum : struct, IComparable
        {
            var type = typeof(T_enum);
            if (_displayNames == null || !_displayNames.ContainsKey(type))
                throw new ArgumentException(string.Format("There are no display names available for the Enum type '{0}'", type.Name));

            var match = _displayNames[type].FirstOrDefault(kvp => string.Compare(kvp.Value, displayName, true) == 0);

            if (match.Key != null)
                return (T_enum)match.Key;
            else
                throw new ArgumentException(string.Format("Failed to parse enum display name '{0}'", displayName));
        }
        
        public static void ClearAbbreviations<T>() where T : struct, IComparable
        {
            if (_abbreviations != null)
            {
                var type = typeof(T);
                if (_abbreviations.ContainsKey(type))
                    _abbreviations.Remove(type);
            }
        }

        public static void AddAbbreviation<T>(this T value, string abbreviation) where T : struct, IComparable
        {
            if (_abbreviations == null)
                _abbreviations = new Dictionary<Type, Dictionary<object, string>>();

            var type = value.GetType();
            if (!_abbreviations.ContainsKey(type))
                _abbreviations.Add(type, new Dictionary<object, string>());

            if (!_abbreviations[type].ContainsKey(value))
                _abbreviations[type].Add(value, abbreviation);
            else
                throw new ArgumentException(string.Format("Enum value '{0}' already has an abbreviation registered", value.ToString()));
        }

        public static string GetAbbreviation(this Enum value)
        {
            var type = value.GetType();
            if (_abbreviations == null || !_abbreviations.ContainsKey(type) || !_abbreviations[type].ContainsKey(value))
                throw new ArgumentException(string.Format("There is no abbreviation available for Enum value '{0}'", value.ToString()));

            return _abbreviations[type][value];
        }

        public static T_enum ParseEnumAbbreviation<T_enum>(string abbreviation) where T_enum : struct, IComparable
        {
            var type = typeof(T_enum);
            if (_abbreviations == null || !_abbreviations.ContainsKey(type))
                throw new ArgumentException(string.Format("There are no abbreviations available for the Enum type '{0}'", type.Name));

            var match = _abbreviations[type].FirstOrDefault(kvp => string.Compare(kvp.Value, abbreviation, true) == 0);

            if (match.Key != null)
                return (T_enum)match.Key;
            else
                throw new ArgumentException(string.Format("Failed to parse enum abbreviation '{0}'", abbreviation));
        }
    }
}
