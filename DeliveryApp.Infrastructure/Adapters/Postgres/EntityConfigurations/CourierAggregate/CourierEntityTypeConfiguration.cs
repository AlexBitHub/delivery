using DeliveryApp.Core.Domain.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate
{
    public class CourierEntityTypeConfiguration : IEntityTypeConfiguration<Courier>
    {
        public void Configure(EntityTypeBuilder<Courier> entityBuilder)
        {
            entityBuilder.ToTable("couriers");
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(entity => entity.Id)
                         .ValueGeneratedNever()
                         .HasColumnName("id")
                         .IsRequired();

            entityBuilder.Property(entity => entity.Name)
                         .HasColumnName("name")
                         .IsRequired();

            entityBuilder.HasOne(entity => entity.Transport)
                         .WithMany()
                         .IsRequired()
                         .HasForeignKey("transport_id");

            entityBuilder.HasOne(entity => entity.Status)
                         .WithMany()
                         .IsRequired()
                         .HasForeignKey("status_id");

            entityBuilder.OwnsOne(entity => entity.Location,
                            builder =>
                            {
                                builder.Property(x => x.X).HasColumnName("location_x").IsRequired();
                                builder.Property(x => x.Y).HasColumnName("location_y").IsRequired();
                                builder.WithOwner();
                            });
        }
    }
}
