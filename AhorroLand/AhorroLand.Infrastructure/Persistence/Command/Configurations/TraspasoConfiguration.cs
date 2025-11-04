using AhorroLand.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AhorroLand.Infrastructure.Persistence.Command.Configurations.Configurations
{
    public class TraspasoConfiguration : IEntityTypeConfiguration<Traspaso>
    {
        public void Configure(EntityTypeBuilder<Traspaso> builder)
        {
            builder.ToTable("traspaso");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(e => e.Importe)
            .HasColumnName("importe")
            .IsRequired();

            builder.Property(e => e.CuentaOrigen)
            .HasColumnName("id_cuenta_origen")
            .IsRequired();

            builder.Property(e => e.CuentaDestino)
            .HasColumnName("id_cuenta_destino")
            .IsRequired();

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
