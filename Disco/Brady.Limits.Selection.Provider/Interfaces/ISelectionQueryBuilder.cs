namespace Brady.Limits.DataService.Nancy.Modules
{
    public interface ISelectionQueryBuilder
    {
        string GetQuery(Domain.Models.Selection selection);
    }
}