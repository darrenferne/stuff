using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Data;
using SchemaBrowser.DataService;
using DataServiceDesigner.Templating;
using System.IO;
using DataServiceDesigner.Domain;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SchemaBrowserUtils browser = new SchemaBrowserUtils();
            
            var connectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=dsd-latest;Integrated Security=True;Pooling=False";
            var keys = browser.GetObjectPrimaryKeys(SchemaBrowser.Domain.DbType.SqlServer, connectionString);
            //var schema = connection.GetSchema();
            //var tables = connection.GetSchema("Tables");
            //var fks = connection.GetSchema("ForeignKeys");
            //var ixs = connection.GetSchema("Indexes");
            //var ixColumns = connection.GetSchema("IndexColumns");
        }

        void DoIt()
        {
            var factories = DbProviderFactories.GetFactoryClasses();
            foreach (DataRow factoryRow in factories.Rows)
            {
                Console.WriteLine(factoryRow[0]);
                Console.WriteLine(factoryRow[1]);
                Console.WriteLine(factoryRow[2]);
                Console.WriteLine(factoryRow[3]);
            }
            
        }

        [TestMethod]
        public void MyTestMethod()
        {
            TemplateGenerator generator = new TemplateGenerator();
            DomainDataService dataService = new DomainDataService();
            dataService.Name = "Test";
            dataService.Schemas = new List<DomainSchema>()
            {
                new DomainSchema()
                {
                    SchemaName = "schema1",
                    IsDefault = true,
                    Objects = new List<DomainObject> {
                        new DomainObject() {
                            TableName = "table1",
                            ObjectName = "Object1",
                            DisplayName = "Object 1",
                            Properties = new List<DomainObjectProperty>(){
                                new DomainObjectProperty(){
                                    PropertyName = "Property1",
                                    ColumnName = "prop1",
                                    PropertyType = "long"
                                },
                                new DomainObjectProperty(){
                                    IsPartOfKey = true,
                                    PropertyName = "Property2",
                                    ColumnName = "prop2",
                                    PropertyType = "string"
                                },
                                new DomainObjectProperty(){
                                    PropertyName = "Property3",
                                    ColumnName = "prop3",
                                    PropertyType = "DateTime"
                                },
                                new DomainObjectProperty(){
                                    PropertyName = "property4",
                                    ColumnName = "prop4",
                                    PropertyType = "bool"
                                },
                                new DomainObjectProperty(){
                                    PropertyName = "Property5",
                                    ColumnName = "prop5",
                                    PropertyType = "decimal"
                                }
                            }
                        },
                        new DomainObject() {
                            TableName = "table2",
                            ObjectName = "Object2",
                            DisplayName = "Object 2",
                            Properties = new List<DomainObjectProperty>(){
                                new DomainObjectProperty(){
                                    PropertyName = "Id",
                                    PropertyType = "long",
                                    IsPartOfKey = true
                                },
                                new DomainObjectProperty(){
                                    PropertyName = "Property2",
                                    PropertyType = "string"
                                }
                            }
                        }
                    }
                }
            };

            var outputPath = Path.Combine(Environment.CurrentDirectory, "Test");
            
            generator.GenerateAndZipSolution(dataService, outputPath);
        }
    }
}
