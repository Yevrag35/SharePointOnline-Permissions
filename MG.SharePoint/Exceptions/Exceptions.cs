using Microsoft.SharePoint.Client;
using System;

namespace MG.SharePoint
{

    #region CONNECTION EXCEPTIONS
    public class ContextNotSetException : NullReferenceException
    {
        private const string MSG = "No existing connection to SharePoint has been made.  Make a connection first, then retry.";

        public ContextNotSetException(string msg = MSG)
            : base(msg) { }
    }

    #endregion

    #region Read-Only Exception

    public class ReadOnlyCollectionException : NotSupportedException
    {
        private protected const string defMsg = "This collection object is read-only.";
        public ReadOnlyCollectionException()
            : base(defMsg)
        {
        }
    }

    #endregion

    #region Permission Exception Interface

    public interface IPermissionException
    {
        string Message { get; }
        object OffendingId { get; }
    }

    #endregion

    #region Permission Exceptions

    public class InvalidBreakInheritanceException : ClientRequestException, IPermissionException
    {
        private protected const string defMsg = "{0} already has unique permissions; inheritance is already broken!";
        public object OffendingId { get; }

        public InvalidBreakInheritanceException()
            : base(string.Format(defMsg, "This object"))
        {
        }

        public InvalidBreakInheritanceException(object id)
            : base(string.Format(defMsg, Convert.ToString(id))) => OffendingId = OffendingId;

        public InvalidBreakInheritanceException(Guid offendingId)
            : base(string.Format(defMsg, offendingId.ToString())) => OffendingId = offendingId;
    }
    public class InvalidResetInheritanceException : ClientRequestException, IPermissionException
    {
        private protected const string defMsg = "{0} is already inheriting permissions from its parent folder!";
        public object OffendingId { get; }

        public InvalidResetInheritanceException()
            : base(string.Format(defMsg, "This object"))
        {
        }

        public InvalidResetInheritanceException(object id)
            : base(string.Format(defMsg, Convert.ToString(id))) => OffendingId = id;

        public InvalidResetInheritanceException(Guid offendingId)
            : base(string.Format(defMsg, offendingId.ToString())) => OffendingId = offendingId;
    }
    public class NoForceBreakException : ClientRequestException, IPermissionException
    {
        private protected const string defMsg = "{0} still inherits its permissions!  Break inheritance first or specify the \"forceBreak\" parameter.";
        public object OffendingId { get; }

        public NoForceBreakException()
            : base(string.Format(defMsg, "This object"))
        {
        }

        public NoForceBreakException(object id)
            : base(string.Format(defMsg, Convert.ToString(id))) => OffendingId = id;

        public NoForceBreakException(Guid offendingId)
            : base(string.Format(defMsg, offendingId.ToString())) => OffendingId = offendingId;
    }

    #endregion
}
