
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace StudyUp.Pages
{
    public class PendenciasModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public PendenciasModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<AtividadePendente> Atividades { get; set; } = new();

        public string? MensagemErro { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                await using var conexao = CriarConexao();
                await conexao.OpenAsync();

                const string sql = @"
                    SELECT
                        a.id,
                        a.data_planejada,
                        a.status,
                        d.nome AS disciplina,
                        c.nome AS atividade
                    FROM atividade a
                    INNER JOIN disciplina d
                        ON a.disciplina_id = d.id
                    INNER JOIN catalogo_atividade c
                        ON a.catalogo_id = c.id
                    WHERE a.status = 'Pendente'
                    ORDER BY a.data_planejada, d.nome, c.nome";

                await using var comando =
                    new MySqlCommand(sql, conexao);

                await using var leitor =
                    await comando.ExecuteReaderAsync();

                while (await leitor.ReadAsync())
                {
                    Atividades.Add(new AtividadePendente
                    {
                        Id = leitor.GetInt32("id"),
                        Nome = leitor.GetString("atividade"),
                        Disciplina = leitor.GetString("disciplina"),
                        DataPlanejada = leitor.GetDateTime("data_planejada")
                    });
                }
            }
            catch (MySqlException)
            {
                MensagemErro = "Não foi possível carregar as pendências.";
            }
        }

        public async Task<IActionResult> OnPostConcluirAsync(int id)
        {
            await using var conexao = CriarConexao();
            await conexao.OpenAsync();

            const string sql = @"
                UPDATE atividade
                SET status = 'Concluída'
                WHERE id = @id AND status = 'Pendente'";

            await using var comando =
                new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@id", id);

            await comando.ExecuteNonQueryAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemarcarAsync(
            int id, DateTime novaData)
        {
            if (novaData == default)
            {
                return BadRequest("Informe uma data válida.");
            }

            await using var conexao = CriarConexao();
            await conexao.OpenAsync();

            const string sql = @"
                UPDATE atividade
                SET data_planejada = @novaData
                WHERE id = @id AND status = 'Pendente'";

            await using var comando =
                new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@novaData", novaData.Date);
            comando.Parameters.AddWithValue("@id", id);

            await comando.ExecuteNonQueryAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExcluirAsync(int id)
        {
            await using var conexao = CriarConexao();
            await conexao.OpenAsync();

            const string sql = @"
        DELETE FROM atividade
        WHERE id = @id";

            await using var comando =
                new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@id", id);

            await comando.ExecuteNonQueryAsync();

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

        public class AtividadePendente
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Disciplina { get; set; } = string.Empty;
            public DateTime DataPlanejada { get; set; }

            public bool EstaAtrasada =>
                DataPlanejada.Date < DateTime.Today;
        }
    }
}
