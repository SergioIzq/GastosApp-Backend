using AhorroLand.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AhorroLand.Infrastructure.Persistence.Command.Configurations.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("clientes"); // 🔧 FIX: Nombre correcto de tabla (plural)
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(e => e.Nombre)
                .HasColumnName("nombre")
                .HasColumnType("varchar")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.UsuarioId)
                .HasColumnName("usuario_id") // 🔧 FIX: Nombre consistente con queries SQL
                .IsRequired();

            builder.Property(e => e.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(e => e.FechaCreacion)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.HasIndex(e => new { e.UsuarioId, e.FechaCreacion })
                .HasDatabaseName("idx_usuario_fecha");

            builder.HasIndex(e => new { e.Nombre, e.UsuarioId })
                .HasDatabaseName("idx_nombre_usuario");

            builder.HasIndex(e => e.UsuarioId)
                .HasDatabaseName("idx_usuario_id_hash");
        }
    }
}
