using DataServiceDesigner.Domain;
using DataServiceDesigner.Templating.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.Templating
{
    public class TemplateGenerator
    {
        public void GenerateDomainProject(DomainDataService dataService, string outputPath)
        {
            var session = new Dictionary<string, object>();
            session.Add("DomainDataService", dataService);

            DomainProject projectTemplate = new DomainProject();
            projectTemplate.Session = session;
            projectTemplate.Initialize();
            string content = projectTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputPath, $"Brady.{dataService.Name}.Domain.csproj"), content);

            DomainConstants constantsTemplate = new DomainConstants();
            constantsTemplate.Session = session;
            constantsTemplate.Initialize();
            content = constantsTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputPath, $"{dataService.Name}Constants.cs"), content);
            if (!(dataService.Schemas is null))
            {
                foreach (var domainSchema in dataService.Schemas)
                {
                    session["CurrentSchema"] = domainSchema;
                    foreach (var domainObject in domainSchema.Objects)
                    {
                        session["CurrentObject"] = domainObject;
                        
                        DomainModel modelTemplate = new DomainModel();
                        modelTemplate.Session = session;
                        modelTemplate.Initialize(); 
                        content = modelTemplate.TransformText();
                        File.WriteAllText(Path.Combine(outputPath, $"{domainObject.ObjectName}.cs"), content);

                        DomainModelMetadata metadataTemplate = new DomainModelMetadata();
                        metadataTemplate.Session = session;
                        metadataTemplate.Initialize();
                        content = metadataTemplate.TransformText();
                        File.WriteAllText(Path.Combine(outputPath, $"{domainObject.ObjectName}Metadata.cs"), content);
                    }
                }
            }

            DomainMetadataBundle bundleTemplate = new DomainMetadataBundle();
            bundleTemplate.Session = session;
            bundleTemplate.Initialize();
            content = bundleTemplate.TransformText();
            File.WriteAllText(Path.Combine(outputPath, $"{dataService.Name}MetadataBundle.cs"), content);
        }
    }
}
