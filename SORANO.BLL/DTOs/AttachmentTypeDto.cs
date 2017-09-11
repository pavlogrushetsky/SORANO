namespace SORANO.BLL.Dtos
{
    public class AttachmentTypeDto : BaseDto
    {
        public string Name { get; set; }

        public string Comment { get; set; }

        public string Extensions { get; set; }

        public int AttachmentsCount { get; set; }
    }
}
