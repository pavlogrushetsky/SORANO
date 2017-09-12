namespace SORANO.BLL.Services
{
    public enum ServiceResponseStatus
    {
        Success,
        NotFound,
        InvalidOperation
    }

    public class ServiceResponse<T>
    {
        public ServiceResponseStatus Status { get; internal set; }

        public T Result { get; internal set; }

        public ServiceResponse(ServiceResponseStatus status, T result = default(T))
        {
            Status = status;
            Result = result;
        }
    }

    public class SuccessResponse<T> : ServiceResponse<T>
    {
        public SuccessResponse(T result = default(T)) : base(ServiceResponseStatus.Success, result)
        {           
        }
    }
}