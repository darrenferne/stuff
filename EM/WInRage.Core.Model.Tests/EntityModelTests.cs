using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WInRage.Core.Model.Tests
{
    [TestClass]
    public class EntityModelTests
    {
        [TestMethod]
        public void it_should_be_possible_to_define_a_new_non_generic_entity()
        {
            var model = new Entity();

            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void it_should_be_possible_to_define_a_new_generic_entity_for_a_class()
        {
            var model = new Entity<TestModel>();

            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void it_should_be_possible_to_add_one_or_more_non_generic_properties_to_an_entity()
        {
            var model = new Entity<TestModel>();

            model.Properties.Add(new EntityProperty());
            model.Properties.Add(new EntityProperty());

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Properties);
            Assert.AreEqual(2, model.Properties.Count);
        }

        [TestMethod]
        public void it_should_be_possible_to_add_one_or_more_generic_properties_to_an_entity()
        {
            var model = new Entity<TestModel>();

            model.Properties.Add(new EntityProperty<string>());
            model.Properties.Add(new EntityProperty<int>());

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Properties);
            Assert.AreEqual(2, model.Properties.Count);
        }

        [TestMethod]
        public void it_should_be_possible_to_add_one_or_more_non_generic_attributes_to_an_entity()
        {
            var model = new Entity<TestModel>();

            model.Attributes.Add(new EntityAttribute());
            model.Attributes.Add(new EntityAttribute());

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Attributes);
            Assert.AreEqual(2, model.Attributes.Count);
        }

        [TestMethod]
        public void it_should_be_possible_to_add_one_or_more_generic_attributes_to_an_entity()
        {
            var model = new Entity<TestModel>();

            model.Attributes.Add(new EntityAttribute<string>());
            model.Attributes.Add(new EntityAttribute<int>());

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Attributes);
            Assert.AreEqual(2, model.Attributes.Count);
        }

        [TestMethod]
        public void it_should_be_possible_to_define_a_name_for_a_non_generic_entity()
        {
            var expectedName = "Test Entity";
            var model = new Entity();
            model.Name = expectedName;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedName, model.Name);
        }

        [TestMethod]
        public void it_should_be_possible_to_define_a_name_for_a_generic_entity()
        {
            var expectedName = "Test Entity";
            var model = new Entity<TestModel>();
            model.Name = expectedName;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedName, model.Name);
        }

        [TestMethod]
        public void it_should_be_possible_to_define_a_descriptiop_for_a_non_generic_entity()
        {
            var expectedDescription = "Test Entity";
            var model = new Entity();
            model.Description = expectedDescription;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedDescription, model.Description);
        }

        [TestMethod]
        public void it_should_be_possible_to_define_a_description_for_a_generic_entity()
        {
            var expectedDescription = "Test Entity";
            var model = new Entity<TestModel>();
            model.Description = expectedDescription;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedDescription, model.Description);
        }
    }
}
