using System;

namespace ApiVendas.Domain
{
    public class Produto
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public decimal Preco { get; private set; }

        // Construtor vazio necessário para o Entity Framework
        protected Produto() { }

        public Produto(string nome, decimal preco)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Preco = preco;
        }
    }
}