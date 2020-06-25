using System;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ContinuationActionRequest<TPayload> : ActionRequest<TPayload>, IContinuationRequest
        where TPayload : IActionProcessingPayload
    {
        public ContinuationActionRequest(Guid requestId, string actionName, TPayload payload, params ActionRequestDescriptor[] continuationActions)
            : base(requestId, actionName, payload)
        {
            ContinuationActions = new List<ActionRequestDescriptor>(continuationActions);
        }
        public ContinuationActionRequest(Guid requestId, string actionName, TPayload payload, IList<ActionRequestDescriptor> continuationActions)
            : base(requestId, actionName, payload)
        {
            ContinuationActions = continuationActions;
        }
        public ContinuationActionRequest(string actionName, TPayload payload, params ActionRequestDescriptor[] continuationActions)
            : this(Guid.NewGuid(), actionName, payload, continuationActions)
        { }

        public ContinuationActionRequest(string actionName, TPayload payload, IList<ActionRequestDescriptor> continuationActions)
            : this(Guid.NewGuid(), actionName, payload, continuationActions)
        { }
        
        public IEnumerable<ActionRequestDescriptor> ContinuationActions { get; }

        public ActionRequestDescriptor NextRequest(IActionProcessingState state)
        {
            var nextAction = ContinuationActions.FirstOrDefault();
            var remaining = ContinuationActions.Skip(1);

            if (remaining.Any())
                return new ContinuationRequestDescriptor(nextAction.ActionName, remaining.ToList());
            else
                return nextAction;
        }
    }
}