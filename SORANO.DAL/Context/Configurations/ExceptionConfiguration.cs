using System.Data.Entity.ModelConfiguration;
using SORANO.CORE;

namespace SORANO.DAL.Context.Configurations
{
    internal class ExceptionConfiguration : EntityTypeConfiguration<Exception>
    {
        public ExceptionConfiguration()
        {
            Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(5000);

            Property(e => e.InnerException)
                .IsRequired()
                .HasMaxLength(5000);

            Property(e => e.StackTrace)
                .IsOptional();

            ToTable("Exceptions");
        }
    }
}