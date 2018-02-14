namespace SORANO.BLL.Dtos
{
    public class AttachmentDto
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FullPath { get; set; }

        public string Extension { get; set; }

        public int AttachmentTypeID { get; set; }

        public AttachmentTypeDto AttachmentType { get; set; }
    }
}
