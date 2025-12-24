using System;
using System.Collections.Generic;
using System.Linq; // Importante para usar o .Any() e .FirstOrDefault()

namespace API_Vendas.Domain
{
    public class Pedido
    {
        public Guid Id { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public bool EstaFechado { get; private set; }

        // Lista interna (privada) onde podemos mexer à vontade
        private readonly List<Produto> _produtos = new();

        // Lista pública (somente leitura) para quem for ler os dados
        public IReadOnlyCollection<Produto> Produtos => _produtos.AsReadOnly();

        public Pedido()
        {
            Id = Guid.NewGuid();
            DataCriacao = DateTime.UtcNow;
            EstaFechado = false;
        }

        // REGRA: Produtos não podem ser adicionados em pedidos fechados
        public void AdicionarProduto(string nome, decimal preco)
        {
            if (EstaFechado)
            {
                // Equivalente ao ValueError do Python
                throw new InvalidOperationException("Não é possível adicionar produtos a um pedido fechado.");
            }

            _produtos.Add(new Produto(nome, preco));
        }

        // REGRA: Produtos não podem ser removidos em pedidos fechados
        public void RemoverProduto(Guid produtoId)
        {
            if (EstaFechado)
            {
                throw new InvalidOperationException("Não é possível remover produtos de um pedido fechado.");
            }

            var produto = _produtos.FirstOrDefault(p => p.Id == produtoId);
            if (produto != null)
            {
                _produtos.Remove(produto);
            }
        }

        // REGRA: Um pedido só pode ser fechado caso contenha ao menos um produto
        public void FecharPedido()
        {
            if (!_produtos.Any())
            {
                throw new InvalidOperationException("O pedido não pode ser fechado pois não possui produtos.");
            }

            EstaFechado = true;
        }
    }
}