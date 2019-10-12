using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Core.Interfaces.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.DataService
{
    public abstract class ObservableRecordType<TItem, TId, TBatchValidator>
                : ChangeableRecordType<TItem, TId, TBatchValidator>,
                  IObservableRecordType<TId, TItem>
                where TItem : class, IHaveId<TId>
                where TBatchValidator : class, IBatchValidator<TId, TItem>, IRequireCrudingDataServiceRepository
    {
        Subject<ChangeSet<TId, TItem>> _preChangeSubject;
        Subject<ChangeSet<TId, TItem>> _postChangeSubject;

        public ObservableRecordType() : base()
        {
            _preChangeSubject = new Subject<ChangeSet<TId, TItem>>();
            _postChangeSubject = new Subject<ChangeSet<TId, TItem>>();
        }

        public IObservable<ChangeSet<TId, TItem>> OnPreChange => _preChangeSubject;
        public IObservable<ChangeSet<TId, TItem>> OnPostChange => _postChangeSubject;

        public override Action<ChangeSet<TId, TItem>, BatchSaveContext<TId, TItem>, string> PreSaveAction
        {
            get
            {
                return (changeSet, context, username) =>
                {

                    base.PreSaveAction(changeSet, context, username);

                    if (_preChangeSubject.HasObservers)
                        _preChangeSubject.OnNext(changeSet);
                };
            }
        }
        public override Action<ChangeSet<TId, TItem>, BatchSaveContext<TId, TItem>, string> PostSavePostCommitAction
        {
            get
            {
                return (changeSet, context, username) =>
                {
                    base.PostSavePostCommitAction(changeSet, context, username);

                    if (_postChangeSubject.HasObservers)
                        _postChangeSubject.OnNext(changeSet);
                };
            }
        }
    }
}
