using DataServiceDesigner.Domain;
using DataServiceDesigner.Templating.DataService;
using DataServiceDesigner.Templating.DataService.Host;
using DataServiceDesigner.Templating.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.Templating
{
    public class TemplateGenerator
    {
        private void ReplaceDirectories(params string[] paths)
        {
            foreach (var path in paths)
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
        }
        public void GenerateDomainProject(DomainDataService dataService, string outputFolder)
        {
            outputFolder = Path.Combine(outputFolder, $"{dataService.Solution.NamespacePrefix}.{dataService.Name}.Domain");
            var constantsFolder = Path.Combine(outputFolder, "Constants");
            var modelsFolder = Path.Combine(outputFolder, "Models");
            var metadataFolder = Path.Combine(outputFolder, "Metadata");

            ReplaceDirectories(constantsFolder, modelsFolder, metadataFolder);

            var session = new Dictionary<string, object>();
            session.Add("DomainDataService", dataService);

            DomainProjectTemplate projectTemplate = new DomainProjectTemplate();
            projectTemplate.Session = session;
            projectTemplate.Initialize();
            string content = projectTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputFolder, $"{dataService.Solution.NamespacePrefix}.{dataService.Name}.Domain.csproj"), content);

            ConstantsTemplate constantsTemplate = new ConstantsTemplate();
            constantsTemplate.Session = session;
            constantsTemplate.Initialize();
            content = constantsTemplate.TransformText();
            File.WriteAllText(Path.Combine(constantsFolder, $"{dataService.Name}Constants.cs"), content);

            if (!(dataService.Schemas is null))
            {
                foreach (var domainObject in dataService.Schemas.SelectMany(s => s.Objects))
                {
                    session["CurrentObject"] = domainObject;

                    ModelTemplate modelTemplate = new ModelTemplate();
                    modelTemplate.Session = session;
                    modelTemplate.Initialize(); 
                    content = modelTemplate.TransformText();
                    File.WriteAllText(Path.Combine(modelsFolder, $"{domainObject.ObjectName}.cs"), content);

                    MetadataTemplate metadataTemplate = new MetadataTemplate();
                    metadataTemplate.Session = session;
                    metadataTemplate.Initialize();
                    content = metadataTemplate.TransformText();
                    File.WriteAllText(Path.Combine(metadataFolder, $"{domainObject.ObjectName}Metadata.cs"), content);
                }
            }

            MetadataBundleTemplate bundleTemplate = new MetadataBundleTemplate();
            bundleTemplate.Session = session;
            bundleTemplate.Initialize();
            content = bundleTemplate.TransformText();
            File.WriteAllText(Path.Combine(metadataFolder, $"{dataService.Name}MetadataBundle.cs"), content);
        }

        public void GenerateDataServiceProject(DomainDataService dataService, string outputFolder)
        {
            outputFolder = Path.Combine(outputFolder, $"{dataService.Solution.NamespacePrefix}.{dataService.Name}.DataService");
            var interfacesFolder = Path.Combine(outputFolder, "Interfaces");
            var mappingsFolder = Path.Combine(outputFolder, "Mappings");
            var recordTypesFolder = Path.Combine(outputFolder, "RecordTypes");
            var validatorsFolder = Path.Combine(outputFolder, "Validators");

            ReplaceDirectories(interfacesFolder, mappingsFolder, recordTypesFolder, validatorsFolder);
            
            var session = new Dictionary<string, object>();
            session.Add("DomainDataService", dataService);

            DataServiceProjectTemplate projectTemplate = new DataServiceProjectTemplate();
            projectTemplate.Session = session;
            projectTemplate.Initialize();
            string content = projectTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputFolder, $"{dataService.Solution.NamespacePrefix}.{dataService.Name}.DataService.csproj"), content);

            IRepositoryTemplate repositoryInterfaceTemplate = new IRepositoryTemplate();
            repositoryInterfaceTemplate.Session = session;
            repositoryInterfaceTemplate.Initialize();
            content = repositoryInterfaceTemplate.TransformText();
            File.WriteAllText(Path.Combine(interfacesFolder, $"I{dataService.Name}DataServiceRespository.cs"), content);

            IDataServiceTemplate dataServiceInterfaceTemplate = new IDataServiceTemplate();
            dataServiceInterfaceTemplate.Session = session;
            dataServiceInterfaceTemplate.Initialize();
            content = dataServiceInterfaceTemplate.TransformText();
            File.WriteAllText(Path.Combine(interfacesFolder, $"I{dataService.Name}DataService.cs"), content);
            
            RepositoryTemplate repositoryTemplate = new RepositoryTemplate();
            repositoryTemplate.Session = session;
            repositoryTemplate.Initialize();
            content = repositoryTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputFolder, $"{dataService.Name}DataServiceRespository.cs"), content);

            DataServiceTemplate dataServiceTemplate = new DataServiceTemplate();
            dataServiceTemplate.Session = session;
            dataServiceTemplate.Initialize();
            content = dataServiceTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputFolder, $"{dataService.Name}DataService.cs"), content);

            KernelManipulationsTemplate kernelManipulationsTemplate = new KernelManipulationsTemplate();
            kernelManipulationsTemplate.Session = session;
            kernelManipulationsTemplate.Initialize();
            content = kernelManipulationsTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputFolder, $"{dataService.Name}KernelManipulations.cs"), content);

            StartupTaskTemplate startupTaskTemplate = new StartupTaskTemplate();
            startupTaskTemplate.Session = session;
            startupTaskTemplate.Initialize();
            content = startupTaskTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputFolder, $"{dataService.Name}Startup.cs"), content);

            if (!(dataService.Schemas is null))
            {
                foreach (var domainObject in dataService.Schemas.SelectMany(s => s.Objects))
                {
                    session["CurrentObject"] = domainObject;

                    MappingTemplate mappingTemplate = new MappingTemplate();
                    mappingTemplate.Session = session;
                    mappingTemplate.Initialize();
                    content = mappingTemplate.TransformText();
                    File.WriteAllText(Path.Combine(mappingsFolder, $"{domainObject.ObjectName}Mapping.cs"), content);

                    RecordTypeTemplate recordTypeTemplate = new RecordTypeTemplate();
                    recordTypeTemplate.Session = session;
                    recordTypeTemplate.Initialize();
                    content = recordTypeTemplate.TransformText();
                    File.WriteAllText(Path.Combine(recordTypesFolder, $"{domainObject.ObjectName}RecordType.cs"), content);

                    if (domainObject.SupportsIHaveId())
                    {
                        var modelValidatorsFolder = Path.Combine(validatorsFolder, domainObject.ObjectName);
                        ReplaceDirectories(modelValidatorsFolder);

                        ValidatorTemplate validatorTemplate = new ValidatorTemplate();
                        validatorTemplate.Session = session;
                        validatorTemplate.Initialize();
                        content = validatorTemplate.TransformText();
                        File.WriteAllText(Path.Combine(modelValidatorsFolder, $"{domainObject.ObjectName}Validator.cs"), content);

                        DeleteValidatorTemplate deleteValidatorTemplate = new DeleteValidatorTemplate();
                        deleteValidatorTemplate.Session = session;
                        deleteValidatorTemplate.Initialize();
                        content = deleteValidatorTemplate.TransformText();
                        File.WriteAllText(Path.Combine(modelValidatorsFolder, $"{domainObject.ObjectName}DeleteValidator.cs"), content);

                        BatchValidatorTemplate batchValidatorTemplate = new BatchValidatorTemplate();
                        batchValidatorTemplate.Session = session;
                        batchValidatorTemplate.Initialize();
                        content = batchValidatorTemplate.TransformText();
                        File.WriteAllText(Path.Combine(modelValidatorsFolder, $"{domainObject.ObjectName}BatchValidator.cs"), content);
                    }
                }
            }
        }

        public void GenerateDataServiceHostProject(DataServiceSolution solution, string outputFolder)
        {
            outputFolder = Path.Combine(outputFolder, $"{solution.NamespacePrefix}.{solution.Name}.DataService.Host");
            var setupFolder = Path.Combine(outputFolder, "Setup");

            ReplaceDirectories(setupFolder);

            var session = new Dictionary<string, object>();
            session.Add("DataServiceSolution", solution);

            HostProjectTemplate projectTemplate = new HostProjectTemplate();
            projectTemplate.Session = session;
            projectTemplate.Initialize();
            string content = projectTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputFolder, $"{solution.NamespacePrefix}.{solution.Name}.DataService.Host.csproj"), content);

            ProgramTemplate programTemplate = new ProgramTemplate();
            programTemplate.Session = session;
            programTemplate.Initialize();
            content = programTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputFolder, $"Program.cs"), content);

            ConstantsTemplate constantsTemplate = new ConstantsTemplate();
            constantsTemplate.Session = session;
            constantsTemplate.Initialize();
            content = constantsTemplate.TransformText();
            File.WriteAllText(Path.Combine(setupFolder, $"Constants.cs"), content);


            ConfigurationTemplate configurationTemplate = new ConfigurationTemplate();
            configurationTemplate.Session = session;
            configurationTemplate.Initialize();
            content = configurationTemplate.TransformText();
            File.WriteAllText(Path.Combine(setupFolder, $"Configuration.cs"), content);

            AvailableCulturesTemplate culturesTemplate = new AvailableCulturesTemplate();
            culturesTemplate.Session = session;
            culturesTemplate.Initialize();
            content = culturesTemplate.TransformText();
            File.WriteAllText(Path.Combine(setupFolder, $"AvailableCultures.cs"), content);
        }

        public void GenerateSolution(DataServiceSolution solution, string outputFolder)
        {
            ReplaceDirectories(outputFolder);

            foreach (var dataService in solution.DataServices)
            {
                GenerateDomainProject(dataService, outputFolder);
                GenerateDataServiceProject(dataService, outputFolder);
            }
            GenerateDataServiceHostProject(solution, outputFolder);
            
            var session = new Dictionary<string, object>();
            session.Add("DataServiceSolution", solution);

            SolutionTemplate solutionTemplate = new SolutionTemplate();
            solutionTemplate.Session = session;
            solutionTemplate.Initialize();
            string content = solutionTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputFolder, $"{solution.NamespacePrefix}.{solution.Name}.sln"), content);

        }

        public void GenerateScripts(DataServiceSolution solution, string outputFolder)
        {
            outputFolder = Path.Combine(outputFolder, $"DatabaseScripts");
            var sqlScriptsBaseFolder = Path.Combine(outputFolder, "SqlServer");
            var oracleScriptsBaseFolder = Path.Combine(outputFolder, "Oracle");

            ReplaceDirectories(outputFolder, sqlScriptsBaseFolder, oracleScriptsBaseFolder);

            foreach (var dataService in solution.DataServices)
            {
                var session = new Dictionary<string, object>();
                session.Add("DomainDataService", dataService);

                if (!(dataService.Schemas is null))
                {
                    var sqlScriptsFolder = Path.Combine(sqlScriptsBaseFolder, dataService.Name);
                    var oracleScriptsFolder = Path.Combine(oracleScriptsBaseFolder, dataService.Name);
                    foreach (var domainSchema in dataService.Schemas)
                    {
                        session["CurrentSchema"] = domainSchema;

                        SqlServerScriptTemplate sqlScriptTemplate = new SqlServerScriptTemplate();
                        sqlScriptTemplate.Session = session;
                        sqlScriptTemplate.Initialize();
                        var content = sqlScriptTemplate.TransformText();
                        File.WriteAllText(Path.Combine(sqlScriptsFolder, $"{domainSchema.SchemaName.ToLower()}_1_0_0_1.sql"), content);

                        OracleScriptTemplate oracleScriptTemplate = new OracleScriptTemplate();
                        oracleScriptTemplate.Session = session;
                        oracleScriptTemplate.Initialize();
                        content = oracleScriptTemplate.TransformText();
                        File.WriteAllText(Path.Combine(oracleScriptsFolder, $"{domainSchema.SchemaName.ToLower()}_1_0_0_1.sql"), content);
                    }
                }
            }
        }

        public string GenerateAllAndZip(DataServiceSolution solution, string outputFolder)
        {
            var solutionFolder = Path.Combine(outputFolder, $"{solution.NamespacePrefix}.{solution.Name}");
            var zipFileName = Path.Combine(outputFolder, $"{solution.NamespacePrefix}.{solution.Name}.zip");

            if (File.Exists(zipFileName))
                File.Delete(zipFileName);

            GenerateSolution(solution, solutionFolder);
            GenerateScripts(solution, solutionFolder);

            ZipFile.CreateFromDirectory(solutionFolder, zipFileName);

            return zipFileName;
        }
    }
}
