namespace SORANO.BLL.Dtos
{
    public class ExceptionDto
    {
        public int ID { get; set; }

        public string Message { get; set; }

        public string InnerException { get; set; }

        public string StackTrace { get; set; }
    }
}