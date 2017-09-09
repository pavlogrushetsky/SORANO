using SORANO.BLL.Properties;

namespace SORANO.BLL.Services
{
    public abstract class ServiceResponse<T>
    {
        public ServiceResponseStatusType Status { get; protected set; }

        public T Result { get; internal set; }

        public string Message { get; protected set; }
    }

    public class SuccessResponse<T> : ServiceResponse<T>
    {
        public SuccessResponse()
        {
            Status = ServiceResponseStatusType.Success;
            Message = string.Empty;
        }

        public SuccessResponse(T result) : this()
        {
            Result = result;
        }        
    }

    public class FailResponse<T> : ServiceResponse<T>
    {
        public FailResponse(string message)
        {
            Status = ServiceResponseStatusType.Fail;
            Message = message;
            Result = default(T);
        }        
    }

    public class AccessDeniedResponse<T> : ServiceResponse<T>
    {
        public AccessDeniedResponse()
        {
            Status = ServiceResponseStatusType.AccessDenied;
            Message = Resource.UserNotFoundMessage;
            Result = default(T);
        }
    }
}