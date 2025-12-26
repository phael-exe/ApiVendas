using Xunit;
using ApiVendas.Domain;
using System;
using System.Linq; // Necessário para usar .First() e .Count()

namespace ApiVendas.Tests
{
    public class PedidoTests
    {
        // TESTE 1: Caminho Feliz - Adicionar item
        [Fact]
        public void Deve_Adicionar_Produto_Com_Sucesso()
        {
            var pedido = new Pedido();
            pedido.AdicionarProduto("Monitor", 800.00m);

            Assert.Single(pedido.Produtos); // Verifica se tem exatamente 1 item
            Assert.Contains(pedido.Produtos, p => p.Nome == "Monitor");
        }

        // TESTE 2: Caminho Feliz - Remover item
        [Fact]
        public void Deve_Remover_Produto_Com_Sucesso()
        {
            // Arrange
            var pedido = new Pedido();
            pedido.AdicionarProduto("Mouse", 50.00m);
            var idProduto = pedido.Produtos.First().Id; // Pega o ID do produto criado

            // Act
            pedido.RemoverProduto(idProduto);

            // Assert
            Assert.Empty(pedido.Produtos); // A lista deve estar vazia
        }

        // TESTE 3: Caminho Feliz - Fechar Pedido
        [Fact]
        public void Deve_Fechar_Pedido_Com_Sucesso_Se_Tiver_Produtos()
        {
            // Arrange
            var pedido = new Pedido();
            pedido.AdicionarProduto("Teclado", 150.00m);

            // Act
            pedido.FecharPedido();

            // Assert
            Assert.True(pedido.EstaFechado);
        }

        // TESTE 4: Regra de Negócio - Não fechar vazio
        [Fact]
        public void Nao_Deve_Fechar_Pedido_Sem_Produtos()
        {
            var pedido = new Pedido();

            var erro = Assert.Throws<InvalidOperationException>(() => pedido.FecharPedido());

            Assert.Equal("O pedido não pode ser fechado pois não possui produtos.", erro.Message);
        }

        // TESTE 5: Regra de Negócio - Não adicionar em fechado
        [Fact]
        public void Nao_Deve_Adicionar_Produto_Em_Pedido_Fechado()
        {
            // Arrange
            var pedido = new Pedido();
            pedido.AdicionarProduto("Cadeira", 500.00m);
            pedido.FecharPedido(); // Fechamos o pedido

            // Act & Assert
            // Tentar adicionar MAIS UM produto deve falhar
            var erro = Assert.Throws<InvalidOperationException>(() =>
                pedido.AdicionarProduto("Mesa", 200.00m)
            );

            Assert.Equal("Não é possível adicionar produtos a um pedido fechado.", erro.Message);
        }

        // TESTE 6: Regra de Negócio - Não remover em fechado
        [Fact]
        public void Nao_Deve_Remover_Produto_Em_Pedido_Fechado()
        {
            // Arrange
            var pedido = new Pedido();
            pedido.AdicionarProduto("Cabo HDMI", 20.00m);
            var idProduto = pedido.Produtos.First().Id;
            pedido.FecharPedido(); // Fechamos o pedido

            // Act & Assert
            var erro = Assert.Throws<InvalidOperationException>(() =>
                pedido.RemoverProduto(idProduto)
            );

            Assert.Equal("Não é possível remover produtos de um pedido fechado.", erro.Message);
        }
    }
}