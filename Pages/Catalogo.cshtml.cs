
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.ComponentModel.DataAnnotations;

namespace StudyUp.Pages
{
    public class CatalogoModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CatalogoModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<ItemCatalogo> Atividades { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Informe o nome da atividade.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        public string? MensagemErro { get; set; }

        public async Task OnGetAsync()
        {
            await CarregarAtividadesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Nome = Nome?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Nome))
            {
                ModelState.AddModelError(
                    nameof(Nome),
                    "Informe o nome da atividade.");
            }

            if (!ModelState.IsValid)
            {
                await CarregarAtividadesAsync();
                return Page();
            }

            try
            {
                await using var conexao = CriarConexao();
                await conexao.OpenAsync();

                const string sql =
                    "INSERT INTO catalogo_atividade (nome) VALUES (@nome)";

                await using var comando =
                    new MySqlCommand(sql, conexao);

                comando.Parameters.AddWithValue("@nome", Nome);

                await comando.ExecuteNonQueryAsync();

                return RedirectToPage();
            }
            catch (MySqlException)
            {
                MensagemErro = "Não foi possível salvar a atividade.";
                await CarregarAtividadesAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostExcluirAsync(int id)
        {
            await using var conexao = CriarConexao();
            await conexao.OpenAsync();

            const string sql = @"
        DELETE FROM catalogo_atividade
        WHERE id = @id
          AND NOT EXISTS (
              SELECT 1
              FROM atividade
              WHERE catalogo_id = @id
          )";

            await using var comando =
                new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@id", id);

            int linhasExcluidas = await comando.ExecuteNonQueryAsync();

            if (linhasExcluidas == 0)
            {
                TempData["MensagemExclusao"] =
                    "Não foi possível excluir o tipo de atividade. " +
                    "Ele pode estar vinculado a atividades agendadas.";
            }
            else
            {
                TempData["MensagemExclusao"] =
                    "Tipo de atividade excluído com sucesso!";
            }

            return RedirectToPage();
        }

        private async Task CarregarAtividadesAsync()
        {
            try
            {
                await using var conexao = CriarConexao();
                await conexao.OpenAsync();

                const string sql =
                    "SELECT id, nome FROM catalogo_atividade ORDER BY nome";

                await using var comando =
                    new MySqlCommand(sql, conexao);

                await using var leitor =
                    await comando.ExecuteReaderAsync();

                while (await leitor.ReadAsync())
                {
                    Atividades.Add(new ItemCatalogo
                    {
                        Id = leitor.GetInt32("id"),
                        Nome = leitor.GetString("nome")
                    });
                }
            }
            catch (MySqlException)
            {
                MensagemErro = "Não foi possível carregar o catálogo.";
            }
        }

        private MySqlConnection CriarConexao()
        {
            string connectionString =
                _configuration.GetConnectionString("StudyUp")
                ?? throw new InvalidOperationException(
                    "Conexão StudyUp não configurada.");

            return new MySqlConnection(connectionString);
        }

        public class ItemCatalogo
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
        }
    }
}
