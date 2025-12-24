using ApiVendas.Domain;
using ApiVendas.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVendas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Iniciar um novo pedido
        // POST: api/pedidos
        [HttpPost]
        public async Task<IActionResult> IniciarPedido()
        {
            var pedido = new Pedido();
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPedido), new { id = pedido.Id }, pedido);
        }

        // 2. Adicionar produtos ao pedido
        // POST: api/pedidos/{id}/produtos
        [HttpPost("{id}/produtos")]
        public async Task<IActionResult> AdicionarProduto(Guid id, [FromBody] AdicionarProdutoDto dto)
        {
            // Carregamos o pedido incluindo os produtos (necessário para validações)
            var pedido = await _context.Pedidos
                .Include(p => p.Produtos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound("Pedido não encontrado.");

            try
            {
                // O método do domínio vai validar se o pedido está fechado
                pedido.AdicionarProduto(dto.Nome, dto.Preco);
                await _context.SaveChangesAsync();
                return Ok(pedido);
            }
            catch (InvalidOperationException ex)
            {
                // Retorna 400 Bad Request se violar a regra de negócio (Ex: Pedido fechado)
                return BadRequest(new { erro = ex.Message });
            }
        }

        // 3. Remover produtos do pedido
        // DELETE: api/pedidos/{id}/produtos/{produtoId}
        [HttpDelete("{id}/produtos/{produtoId}")]
        public async Task<IActionResult> RemoverProduto(Guid id, Guid produtoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Produtos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound("Pedido não encontrado.");

            try
            {
                pedido.RemoverProduto(produtoId);
                await _context.SaveChangesAsync();
                return NoContent(); // 204 No Content é padrão para delete com sucesso
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        // 4. Fechar o pedido
        // PATCH: api/pedidos/{id}/fechar
        [HttpPatch("{id}/fechar")]
        public async Task<IActionResult> FecharPedido(Guid id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Produtos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();

            try
            {
                pedido.FecharPedido();
                await _context.SaveChangesAsync();
                return Ok(new { mensagem = "Pedido fechado com sucesso.", pedido.EstaFechado });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        // 5. Obter um pedido pelo ID
        // GET: api/pedidos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPedido(Guid id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Produtos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        // 6. Listar pedidos (Com Paginação e Filtro de Status opcionais)
        // GET: api/pedidos?status=true&pagina=1&tamanho=10
        [HttpGet]
        public async Task<IActionResult> ListarPedidos(
            [FromQuery] bool? statusFechado = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            var query = _context.Pedidos.AsQueryable();

            // Filtro opcional por status
            if (statusFechado.HasValue)
            {
                query = query.Where(p => p.EstaFechado == statusFechado.Value);
            }

            // Paginação
            var total = await query.CountAsync();
            var pedidos = await query
                .Include(p => p.Produtos)
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync();

            return Ok(new
            {
                Total = total,
                Pagina = pagina,
                Tamanho = tamanho,
                Dados = pedidos
            });
        }
    }

    // DTO: Objeto simples para receber o JSON do produto
    public record AdicionarProdutoDto(string Nome, decimal Preco);
}