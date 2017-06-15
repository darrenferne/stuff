using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wickes
{
    public class Version
    {
        public Version(string version)
        {
            if (string.IsNullOrEmpty(version))
                throw new ArgumentException("Version required");

            string[] versionNumbers = version.Split('.', ',', ':', ';', '-');
            ushort versionNumber;

            if (versionNumbers.Length == 0)
                throw new ArgumentException("Version numbers could not be separated");

            if (versionNumbers.Length > 0)
            {
                if (ushort.TryParse(versionNumbers[0], out versionNumber))
                    MajorVersion = versionNumber;
                else
                    throw new ArgumentException("Invalid major version number");
            }

            if (versionNumbers.Length > 1)
            {
                if (ushort.TryParse(versionNumbers[1], out versionNumber))
                    MinorVersion = versionNumber;
                else
                    throw new ArgumentException("Invalid minor version number");
            }

            if (versionNumbers.Length > 2)
            {
                if (ushort.TryParse(versionNumbers[2], out versionNumber))
                    Revision = versionNumber;
                else
                    throw new ArgumentException("Invalid revision number");
            }

            if (versionNumbers.Length > 3)
            {
                if (ushort.TryParse(versionNumbers[3], out versionNumber))
                    Build = versionNumber;
                else
                    throw new ArgumentException("Invalid build number");
            }
            }

        public Version(ushort majorVersion = 1, ushort minorVersion = 0, ushort revision = 0, ushort build = 0)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            Revision = revision;
            Build = build;
        }

        public ushort MajorVersion { get; set; }
        public ushort MinorVersion { get; set; }
        public ushort Revision { get; set; }
        public ushort Build { get; set; }

        public override string ToString()
        {
            return $"{ MajorVersion }.{ MinorVersion }.{ Revision }.{ Build }");
        }
    }
}
