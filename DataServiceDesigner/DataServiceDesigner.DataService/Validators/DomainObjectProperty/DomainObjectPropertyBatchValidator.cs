﻿using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectPropertyBatchValidator : BatchValidator<long, DesignerDomainObjectProperty>
    {
        protected override void SetupValidators(ChangeSetItems<long, DesignerDomainObjectProperty> items)
        {
            validator = new DomainObjectPropertyValidator();
            deletionValidator = new EmptyValidator<DesignerDomainObjectProperty>();
        }
    }
}