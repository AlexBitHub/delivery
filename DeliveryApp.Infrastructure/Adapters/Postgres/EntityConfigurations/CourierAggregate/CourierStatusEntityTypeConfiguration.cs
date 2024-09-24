using DeliveryApp.Core.Domain.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate
{
    internal class CourierStatusEntityTypeConfiguration : IEntityTypeConfiguration<Courier>
    {
        public void Configure(EntityTypeBuilder<Courier> builder)
        {
            builder.ToTable("courier_statuses");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever()
                   .HasColumnName("id")
                   .IsRequired();

            builder.Property(x => x.Id)                   
                   .HasColumnName("name")
                   .IsRequired();
        }
    }
}
