using AhorroLand.Domain.Categorias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AhorroLand.Infrastructure.Persistence.Command.Configurations.Configurations
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("categoria");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(e => e.Nombre)
            .HasColumnName("nombre")
            .HasColumnType("varchar")
            .HasMaxLength(100)
            .IsRequired();

            builder.Property(e => e.Descripcion)
            .HasColumnName("descripcion")
            .HasColumnType("varchar")
            .HasMaxLength(200)
            .IsRequired(false);

            builder.Property(e => e.IdUsuario)
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
