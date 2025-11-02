using AhorroLand.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AhorroLand.Infrastructure.Persistence.Command.Configurations.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuario");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(e => e.Correo)
            .HasColumnName("username")
            .HasColumnType("varchar")
            .HasMaxLength(100)
            .IsRequired();

            builder.Property(e => e.ContrasenaHash)
            .HasColumnName("password")
            .HasColumnType("varchar")
            .HasMaxLength(200)
            .IsRequired();

            builder.Property(e => e.TokenConfirmacion)
            .HasColumnName("token_confirmacion")
            .HasColumnType("varchar")
            .HasMaxLength(200)
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
