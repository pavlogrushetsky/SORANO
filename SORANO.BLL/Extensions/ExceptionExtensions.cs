using SORANO.BLL.Dtos;
using SORANO.CORE;

namespace SORANO.BLL.Extensions
{
    internal static class ExceptionExtensions
    {
        public static ExceptionDto ToDto(this Exception ex)
        {
            return new ExceptionDto
            {
                ID = ex.ID,
                Message = ex.Message,
                InnerException = ex.InnerException,
                StackTrace = ex.StackTrace
            };
        }

        public static Exception ToEntity(this ExceptionDto dto)
        {
            return new Exception
            {
                ID = dto.ID,
                Message = dto.Message,
                InnerException = dto.InnerException,
                StackTrace = dto.StackTrace
            };
        }
    }
}