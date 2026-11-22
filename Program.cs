
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Configuração das páginas
builder.Services.AddRazorPages();

var app = builder.Build();

// TESTE DE CONEXÃO COM MYSQL
if (app.Environment.IsDevelopment())
{
    try
    {
        string? connectionString =
            builder.Configuration.GetConnectionString("StudyUp");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Conexão StudyUp não configurada.");
        }

        using var conexao = new MySqlConnection(connectionString);

        conexao.Open();

        Console.WriteLine("StudyUp: conexão com MySQL realizada com sucesso!");

        conexao.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine("StudyUp: erro ao conectar ao MySQL.");
        Console.WriteLine(ex.Message);
    }
}

// Configuração do aplicativo
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
