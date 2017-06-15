using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enumbum.Tests
{
    [TestClass]
    public class EnumTests
    {
        private string ExceptionMessage(Action action)
        {
            try
            {
                action.Invoke();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
   
        [TestMethod]
        public void AddingAValidDescription()
        {
            EnumExtensions.ClearDescriptions<TestEnum>();

            string description = "The first enumerated value";

            TestEnum.TheFirst.AddDescription(description);

            Assert.AreEqual(TestEnum.TheFirst.GetDescription(), description);
        }     

        [TestMethod]
        public void AddingADuplicateDescription()
        {
            EnumExtensions.ClearDescriptions<TestEnum>();

            string description = "The first enumerated value";

            TestEnum.TheFirst.AddDescription(description);

            string expectedMessage = string.Format("Enum value '{0}' already has a description registered", TestEnum.TheFirst.ToString());
            Assert.AreEqual(ExceptionMessage(() => TestEnum.TheFirst.AddDescription(description)), expectedMessage);
        }
               
        [TestMethod]
        public void AddingAValidDisplayName()
        {
            EnumExtensions.ClearDisplayNames<TestEnum>();

            string displayName = "The First";

            TestEnum.TheFirst.AddDisplayName(displayName);

            Assert.AreEqual(TestEnum.TheFirst.GetDisplayName(), displayName);
        }

        [TestMethod]
        public void AddingADuplicateDisplayName()
        {
            EnumExtensions.ClearDisplayNames<TestEnum>();

            string displayName = "The First";

            TestEnum.TheFirst.AddDisplayName(displayName);

            string expectedMessage = string.Format("Enum value '{0}' already has a display name registered", TestEnum.TheFirst.ToString());
            Assert.AreEqual(ExceptionMessage(() => TestEnum.TheFirst.AddDisplayName(displayName)), expectedMessage);
        }

        [TestMethod]
        public void AddingAValidAbbreviation()
        {
            EnumExtensions.ClearAbbreviations<TestEnum>();

            string abbreviation = "Fst";

            TestEnum.TheFirst.AddAbbreviation(abbreviation);

            Assert.AreEqual(TestEnum.TheFirst.GetAbbreviation(), abbreviation);
        }

        [TestMethod]
        public void AddingADuplicateAbbreviation()
        {
            EnumExtensions.ClearAbbreviations<TestEnum>();

            string abbreviation = "Fst";

            TestEnum.TheFirst.AddAbbreviation(abbreviation);

            string expectedMessage = string.Format("Enum value '{0}' already has an abbreviation registered", TestEnum.TheFirst.ToString());
            Assert.AreEqual(ExceptionMessage(() => TestEnum.TheFirst.AddAbbreviation(abbreviation)), expectedMessage);
        }

        [TestMethod]
        public void ParseDescription()
        {
            EnumExtensions.ClearDescriptions<TestEnum>();

            string description = "The first enumerated value";

            TestEnum.TheFirst.AddDescription(description);

            Assert.AreEqual(description.ParseEnum<TestEnum.TheFirst>(description), TestEnum.TheFirst);
        }
    }
}
