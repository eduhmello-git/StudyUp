
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.ComponentModel.DataAnnotations;

namespace StudyUp.Pages
{
    public class AgendarModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public AgendarModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Opcao> Disciplinas { get; set; } = new();
        public List<Opcao> Catalogo { get; set; } = new();

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma disciplina.")]
        public int DisciplinaId { get; set; }

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma atividade.")]
        public int CatalogoId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Informe a data planejada.")]
        [DataType(DataType.Date)]
        public DateTime? DataPlanejada { get; set; }

        public string? MensagemErro { get; set; }

        public async Task OnGetAsync()
        {
            DataPlanejada = DateTime.Today;
            await CarregarOpcoesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CarregarOpcoesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await using var conexao = CriarConexao();
                await conexao.OpenAsync();

                const string sql = @"
                    INSERT INTO atividade
                        (catalogo_id, disciplina_id, data_planejada, status)
                    VALUES
                        (@catalogo, @disciplina, @data, 'Pendente')";

                await using var comando =
                    new MySqlCommand(sql, conexao);

                comando.Parameters.AddWithValue("@catalogo", CatalogoId);
                comando.Parameters.AddWithValue("@disciplina", DisciplinaId);
                comando.Parameters.AddWithValue(
                    "@data", DataPlanejada!.Value.Date);

                await comando.ExecuteNonQueryAsync();

                TempData["Sucesso"] = "Atividade agendada com sucesso!";
                return RedirectToPage();
            }
            catch (MySqlException)
            {
                MensagemErro =
                    "Não foi possível agendar. Confira os dados selecionados.";
                return Page();
            }
        }

        private async Task CarregarOpcoesAsync()
        {
            try
            {
                await using var conexao = CriarConexao();
                await conexao.OpenAsync();

                const string sqlDisciplinas =
                    "SELECT id, nome FROM disciplina ORDER BY nome";

                await using (var comando =
                    new MySqlCommand(sqlDisciplinas, conexao))
                await using (var leitor =
                    await comando.ExecuteReaderAsync())
                {
                    while (await leitor.ReadAsync())
                    {
                        Disciplinas.Add(new Opcao
                        {
                            Id = leitor.GetInt32("id"),
                            Nome = leitor.GetString("nome")
                        });
                    }
                }

                const string sqlCatalogo =
                    "SELECT id, nome FROM catalogo_atividade ORDER BY nome";

                await using (var comando =
                    new MySqlCommand(sqlCatalogo, conexao))
                await using (var leitor =
                    await comando.ExecuteReaderAsync())
                {
                    while (await leitor.ReadAsync())
                    {
                        Catalogo.Add(new Opcao
                        {
                            Id = leitor.GetInt32("id"),
                            Nome = leitor.GetString("nome")
                        });
                    }
                }
            }
            catch (MySqlException)
            {
                MensagemErro = "Não foi possível carregar as opções.";
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

        public class Opcao
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
        }
    }
}
