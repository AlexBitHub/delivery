using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate
{
    internal class CourierStatusEntityTypeConfiguration : IEntityTypeConfiguration<CourierStatus>
    {
        public void Configure(EntityTypeBuilder<CourierStatus> builder)
        {
            builder.ToTable("courier_statuses");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever()
                   .HasColumnName("id")
                   .IsRequired();

            builder.Property(x => x.Name)                   
                   .HasColumnName("name")
                   .IsRequired();
        }
    }
}
