using Akka.Actor;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionProcessorUser : IActionProcessorUser
    {
        private readonly IActionProcessorAuthorisation _authorisation;
        private readonly string _userName;
        public ActionProcessorUser(IActionProcessorAuthorisation authorisation, string userName)
        {
            _authorisation = authorisation;
            _userName = userName;
        }

        public string UserName => _userName;

        public bool CanPerform(string actionName)
        {
            return _authorisation?.CanPerform(_userName, actionName) ?? true;
        }
    }
}