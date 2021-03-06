﻿using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectBatchValidator : BatchValidator<long, DomainObject>
    {
        protected override void SetupValidators(ChangeSetItems<long, DomainObject> items)
        {
            validator = new DomainObjectValidator();
            deletionValidator = new EmptyValidator<DomainObject>();
        }
    }
}