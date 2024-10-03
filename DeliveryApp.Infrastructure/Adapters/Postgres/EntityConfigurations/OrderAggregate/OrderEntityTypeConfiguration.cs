using DeliveryApp.Core.Domain.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.OrderAggregate
{
    internal class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entityBuilder)
        {
            entityBuilder.ToTable("orders");

            entityBuilder.HasKey(entity => entity.Id);

            entityBuilder
                .Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .IsRequired();

            entityBuilder
                .Property(entity => entity.CourierId)
                .HasColumnName("courier_id")
                .IsRequired(false);

            entityBuilder
                .HasOne(entity => entity.Status)
                .WithMany()
                .IsRequired()
                .HasForeignKey("status_id");

            entityBuilder.OwnsOne(entity => entity.Location, 
                                  builder =>
                                  {
                                      builder.Property(x => x.X).HasColumnName("location_x").IsRequired();
                                      builder.Property(y => y.Y).HasColumnName("location_y").IsRequired();
                                      builder.WithOwner();
                                  });
        }
    }

}
