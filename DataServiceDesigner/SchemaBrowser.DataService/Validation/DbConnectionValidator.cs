using BWF.DataServices.Support.NHibernate.Abstract;
using FluentValidation;
using SchemaBrowser.Domain;
using System.Linq;

namespace SchemaBrowser.DataService
{
    public class DbConnectionValidator : Validator<DbConnection>
    {
        SchemaBrowserUtils _utils;
        ISchemaBrowserRepository _repository;
        public DbConnectionValidator(ISchemaBrowserRepository repository)
        {
            _repository = repository;
            _utils = new SchemaBrowserUtils();

            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0L);

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1, 64)
                .Must(BeUnique)
                    .WithMessage("The 'Name' must be unique");

            RuleFor(x => x.DataSource)
                .NotEmpty()
                .Length(1, 1000);

            RuleFor(x => x.InitialCatalog)
                .Length(0, 64)
                .Must(OnlyBeSuppliedForSQLServer)
                    .WithMessage("An 'Initial Catalog' is required for SQL Server connections");

            RuleFor(x => x.Username)
                .Length(0, 64)
                .Must(OnlyBeSuppliedForNonIntegratedSecurity)
                    .WithMessage("A 'User Name' is required");

            RuleFor(x => x.Password)
                .Length(0, 64)
                .Must(OnlyBeSuppliedForNonIntegratedSecurity)
                    .WithMessage("A 'Password' is required");

            RuleFor(x => x.ConnectionString)
                .NotEmpty()
                .Length(1, 2000);

            //Custom(p => MustHaveAValidConnectionString(p));
        }

        private bool BeUnique(DbConnection connection, string name)
        {
            return !_repository.GetWhere<DbConnection>(c => c.Name.ToLower() == name.ToLower() && c.Id != connection.Id).Any();
        }

        private bool OnlyBeSuppliedForSQLServer(DbConnection connection, string initialCatalog)
        {
            return (connection.DatabaseType == DatabaseType.SqlServer && !string.IsNullOrEmpty(initialCatalog)) ||
                   (connection.DatabaseType == DatabaseType.Oracle && string.IsNullOrEmpty(initialCatalog));
        }

        private bool OnlyBeSuppliedForNonIntegratedSecurity(DbConnection connection, string value)
        {
            return connection.UseIntegratedSecurity ? string.IsNullOrEmpty(value) : !string.IsNullOrEmpty(value);
        }

        //private ValidationFailure MustHaveAValidConnectionString(DbConnection connection)
        //{
        //    string message;
        //    if (string.IsNullOrEmpty(connection.ConnectionString) || _utils.TestConnection(connection.DatabaseType, connection.ConnectionString, out message))
        //        return null;
        //    else
        //        return new ValidationFailure("ConnectionString", $"Invalid connection string:{message}");
        //}
    }
}