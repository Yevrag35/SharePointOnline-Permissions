using Microsoft.SharePoint.Client;
using System;

namespace MG.SharePoint
{
    public interface IPermissionException
    {
        string Message { get; }
        Guid OffendingId { get; }
    }

    public class InvalidBreakInheritanceException : ClientRequestException, IPermissionException
    {
        private protected const string defMsg = "{0} already has unique permissions; inheritance is already broken!";
        public Guid OffendingId { get; }

        public InvalidBreakInheritanceException(Guid offendingId)
            : base(string.Format(defMsg, offendingId.ToString()))
        {
            OffendingId = offendingId;
        }
    }
    public class InvalidResetInheritanceException : ClientRequestException, IPermissionException
    {
        private protected const string defMsg = "{0} is already inherited permissions from its parent folder!";
        public Guid OffendingId { get; }

        public InvalidResetInheritanceException(Guid offendingId)
            : base(string.Format(defMsg, offendingId.ToString()))
        {
            OffendingId = offendingId;
        }
    }
    public class NoForceBreakException : ClientRequestException, IPermissionException
    {
        private protected const string defMsg = "{0} still inherits its permissions!  Break inheritance first or specify the \"forceBreak\" parameter.";
        public Guid OffendingId { get; }

        public NoForceBreakException(Guid offendingId)
            : base(string.Format(defMsg, offendingId.ToString()))
        {
            OffendingId = offendingId;
        }
    }
}
