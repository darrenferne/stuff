using BWF.DataServices.Core.Interfaces.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.DataService
{
    public interface IObservableRecordType<TId, TItem> : IChangeableRecordType<TId, TItem>
            where TItem : class, IHaveId<TId>
    {
        IObservable<ChangeSet<TId, TItem>> OnPreChange { get; }
        IObservable<ChangeSet<TId, TItem>> OnPostChange { get; }
    }
}
