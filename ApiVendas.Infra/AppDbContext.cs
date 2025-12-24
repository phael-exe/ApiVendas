using API_Vendas.Domain;
using ApiVendas.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ApiVendas.Infra
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do Pedido
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Ensina o EF Core a usar o campo privado '_produtos'
                // em vez de tentar usar a propriedade somente leitura.
                entity.Metadata.FindNavigation(nameof(Pedido.Produtos))
                      .SetPropertyAccessMode(PropertyAccessMode.Field);
            });
        }
    }
}