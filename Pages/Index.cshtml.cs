
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace StudyUp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int Pendentes { get; set; }
        public int Concluidas { get; set; }
        public List<AtividadeAgenda> Atividades { get; set; } = new();
        public DateTime DataSelecionada { get; set; }

        public string Visualizacao { get; set; } = "dia";

        public DateTime DataFinal =>
            Visualizacao == "semana"
                ? DataSelecionada.AddDays(6)
                : DataSelecionada;


        public string? MensagemErro { get; set; }

        public async Task OnGetAsync(DateTime? data, string? visualizacao)
        {
            DataSelecionada = data?.Date ?? DateTime.Today;

            Visualizacao = visualizacao == "semana"
                ? "semana"
                : "dia";

            try
            {
                string connectionString =
                    _configuration.GetConnectionString("StudyUp")
                    ?? throw new InvalidOperationException(
                        "Conexão StudyUp não configurada.");

                await using var conexao =
                    new MySqlConnection(connectionString);

                await conexao.OpenAsync();

                const string sqlResumo = @"
                    SELECT
                        COUNT(CASE WHEN status = 'Pendente'
                              THEN 1 END) AS pendentes,
                        COUNT(CASE WHEN status = 'Concluída'
                              THEN 1 END) AS concluidas
                    FROM atividade";

                await using (var comando =
                    new MySqlCommand(sqlResumo, conexao))
                await using (var leitor =
                    await comando.ExecuteReaderAsync())
                {
                    if (await leitor.ReadAsync())
                    {
                        Pendentes = Convert.ToInt32(leitor["pendentes"]);
                        Concluidas = Convert.ToInt32(leitor["concluidas"]);
                    }
                }

                const string sqlAgenda = @"
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
                    WHERE a.data_planejada BETWEEN @dataInicial AND @dataFinal
                    ORDER BY a.data_planejada, d.nome, c.nome";

                await using (var comando =
                    new MySqlCommand(sqlAgenda, conexao))
                {
                    comando.Parameters.AddWithValue(
    "@dataInicial", DataSelecionada);

                    comando.Parameters.AddWithValue(
                        "@dataFinal", DataFinal);




                    await using var leitor =
                        await comando.ExecuteReaderAsync();

                    while (await leitor.ReadAsync())
                    {
                        Atividades.Add(new AtividadeAgenda
                        {
                            Id = leitor.GetInt32("id"),
                            Disciplina = leitor.GetString("disciplina"),
                            Nome = leitor.GetString("atividade"),
                            DataPlanejada = leitor.GetDateTime("data_planejada"),
                            Status = leitor.GetString("status")
                        });
                    }
                }
            }
            catch (MySqlException)
            {
                MensagemErro = "Não foi possível carregar a agenda.";
            }
        }

        public async Task<IActionResult> OnPostAlterarStatusAsync(
    int id, string novoStatus, DateTime data, string visualizacao)
        {
            if (novoStatus != "Pendente" &&
                novoStatus != "Concluída" &&
                novoStatus != "Cancelada")
            {
                return BadRequest("Status inválido.");
            }

            string connectionString =
                _configuration.GetConnectionString("StudyUp")
                ?? throw new InvalidOperationException(
                    "Conexão StudyUp não configurada.");

            await using var conexao =
                new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            const string sql = @"
        UPDATE atividade
        SET status = @status
        WHERE id = @id";

            await using var comando =
                new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@status", novoStatus);
            comando.Parameters.AddWithValue("@id", id);

            await comando.ExecuteNonQueryAsync();

            return RedirectToPage(new
            {
                data = data.ToString("yyyy-MM-dd"),
                visualizacao = visualizacao == "semana" ? "semana" : "dia"
            });
        }

        public async Task<IActionResult> OnPostRemarcarAsync(
            int id, DateTime novaData, DateTime data, string visualizacao)

        {
            if (novaData == default)
            {
                return BadRequest("Informe uma data válida.");
            }

            string connectionString =
                _configuration.GetConnectionString("StudyUp")
                ?? throw new InvalidOperationException(
                    "Conexão StudyUp não configurada.");

            await using var conexao =
                new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            const string sql = @"
        UPDATE atividade
        SET data_planejada = @novaData
        WHERE id = @id";

            await using var comando =
                new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@novaData", novaData.Date);
            comando.Parameters.AddWithValue("@id", id);

            await comando.ExecuteNonQueryAsync();

            return RedirectToPage(new
            {
                data = data.ToString("yyyy-MM-dd"),
                visualizacao = visualizacao == "semana" ? "semana" : "dia"
            });

        }


        public async Task<IActionResult> OnPostExcluirAsync(
            int id, DateTime data, string visualizacao)
        {
            string connectionString =
                _configuration.GetConnectionString("StudyUp")
                ?? throw new InvalidOperationException(
                    "Conexão StudyUp não configurada.");

            await using var conexao =
                new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            const string sql = @"
        DELETE FROM atividade
        WHERE id = @id";

            await using var comando =
                new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@id", id);

            await comando.ExecuteNonQueryAsync();

            return RedirectToPage(new
            {
                data = data.ToString("yyyy-MM-dd"),
                visualizacao = visualizacao == "semana" ? "semana" : "dia"
            });
        }

        public class AtividadeAgenda
        {
            public int Id { get; set; }
            public string Disciplina { get; set; } = string.Empty;
            public string Nome { get; set; } = string.Empty;
            public DateTime DataPlanejada { get; set; }
            public string Status { get; set; } = string.Empty;
            public bool EstaAtrasada =>
            Status == "Pendente" &&
            DataPlanejada.Date < DateTime.Today;
        }
    }
}
