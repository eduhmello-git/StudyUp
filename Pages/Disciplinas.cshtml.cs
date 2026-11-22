
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.ComponentModel.DataAnnotations;

namespace StudyUp.Pages
{
    public class DisciplinasModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DisciplinasModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Disciplina> Disciplinas { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Informe o nome da disciplina.")]
        public string Nome { get; set; } = string.Empty;

        public string? MensagemErro { get; set; }

        public async Task OnGetAsync()
        {
            await CarregarDisciplinasAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CarregarDisciplinasAsync();
                return Page();
            }

            try
            {
                await using var conexao = CriarConexao();
                await conexao.OpenAsync();

                const string sql =
                    "INSERT INTO disciplina (nome) VALUES (@nome)";

                await using var comando =
                    new MySqlCommand(sql, conexao);

                comando.Parameters.AddWithValue(
                    "@nome", Nome.Trim());

                await comando.ExecuteNonQueryAsync();

                return RedirectToPage();
            }
            catch (MySqlException)
            {
                MensagemErro = "Não foi possível salvar a disciplina.";
                await CarregarDisciplinasAsync();
                return Page();
            }
        }

        private async Task CarregarDisciplinasAsync()
        {
            try
            {
                await using var conexao = CriarConexao();
                await conexao.OpenAsync();

                const string sql =
                    "SELECT id, nome FROM disciplina ORDER BY nome";

                await using var comando =
                    new MySqlCommand(sql, conexao);

                await using var leitor =
                    await comando.ExecuteReaderAsync();

                while (await leitor.ReadAsync())
                {
                    Disciplinas.Add(new Disciplina
                    {
                        Id = leitor.GetInt32("id"),
                        Nome = leitor.GetString("nome")
                    });
                }
            }
            catch (MySqlException)
            {
                MensagemErro = "Não foi possível carregar as disciplinas.";
            }
        }


        public async Task<IActionResult> OnPostExcluirAsync(int id)
        {
            string connectionString =
                _configuration.GetConnectionString("StudyUp")
                ?? throw new InvalidOperationException(
                    "Conexão StudyUp não configurada.");

            await using var conexao =
                new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            const string sql = @"
        DELETE FROM disciplina
        WHERE id = @id
          AND NOT EXISTS (
              SELECT 1
              FROM atividade
              WHERE disciplina_id = @id
          )";

            await using var comando =
                new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@id", id);

            int linhasExcluidas = await comando.ExecuteNonQueryAsync();

            if (linhasExcluidas == 0)
            {
                TempData["MensagemExclusao"] =
                    "Não foi possível excluir a disciplina. " +
                    "Ela pode possuir atividades vinculadas.";
            }
            else
            {
                TempData["MensagemExclusao"] =
                    "Disciplina excluída com sucesso!";
            }

            return RedirectToPage();
        }

        private MySqlConnection CriarConexao()
        {
            string connectionString =
                _configuration.GetConnectionString("StudyUp")
                ?? throw new InvalidOperationException(
                    "Conexão StudyUp não configurada.");

            return new MySqlConnection(connectionString);
        }

        public class Disciplina
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
        }
    }
}
