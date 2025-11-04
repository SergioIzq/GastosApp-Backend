using AhorroLand.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AhorroLand.Infrastructure.Persistence.Command.Configurations.Configurations
{
    public class GastoConfiguration : IEntityTypeConfiguration<Gasto>
    {
        public void Configure(EntityTypeBuilder<Gasto> builder)
        {
            builder.ToTable("gasto");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(e => e.Importe)
            .HasColumnName("importe")
            .IsRequired();

            builder.Property(e => e.CuentaId)
            .HasColumnName("id_cuenta")
            .IsRequired();

            builder.Property(e => e.CategoriaId)
            .HasColumnName("id_categoria")
            .IsRequired();

            builder.Property(e => e.ProveedorId)
            .HasColumnName("id_proveedor")
            .IsRequired(false);

            builder.Property(e => e.FormaPagoId)
            .HasColumnName("id_forma_pago")
            .IsRequired(false);

            builder.Property(e => e.UsuarioId)
            .HasColumnName("id_usuario")
            .IsRequired();

            builder.Property(e => e.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .IsRequired()
            .ValueGeneratedOnAdd();

            builder.Property(e => e.FechaCreacion)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
