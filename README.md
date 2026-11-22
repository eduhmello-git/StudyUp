StudyUp



\*\*Sistema web de organização e acompanhamento de atividades acadêmicas\*\*



Projeto acadêmico desenvolvido como Prova de Conceito (PoC), com o objetivo de validar uma solução digital para auxiliar estudantes universitários no planejamento e acompanhamento de suas atividades de estudo.



\## 1. Sobre o projeto



O StudyUp é uma aplicação web voltada a estudantes do ensino superior que enfrentam dificuldades para organizar suas atividades acadêmicas e cumprir o planejamento estabelecido.



O projeto surgiu da identificação de um problema recorrente: muitos estudantes conseguem planejar seus estudos, mas encontram dificuldades para acompanhar e executar as tarefas dentro dos prazos definidos.



A proposta do StudyUp é oferecer um ambiente simples e intuitivo para cadastrar disciplinas, organizar atividades e acompanhar pendências.



O sistema foi inicialmente idealizado com recursos de gamificação, notificações e análise de progresso. Entretanto, nesta segunda etapa, foi desenvolvido um Produto Mínimo Viável (MVP), priorizando as funcionalidades essenciais de organização acadêmica.



\## 2. Objetivo da Prova de Conceito (PoC)



Demonstrar a viabilidade técnica de uma aplicação web capaz de integrar o cadastro de disciplinas, o planejamento de atividades e o acompanhamento de tarefas acadêmicas.



A PoC busca validar o fluxo principal de utilização do StudyUp, desde o cadastro de uma disciplina até a conclusão ou remarcação de uma atividade.



\## 3. Público-alvo



Estudantes do ensino superior, especialmente aqueles que precisam conciliar aulas, trabalhos, avaliações e outras responsabilidades acadêmicas.



\## 4. Jornada do usuário



Na primeira etapa do projeto, foi definida uma jornada para a persona \*\*Mariana Oliveira\*\*, estudante universitária que precisa organizar sua rotina de estudos.



A jornada original contempla sete etapas:



| Etapa | Descrição |

| 1. Identificação da necessidade | Mariana identifica uma avaliação e percebe que precisa organizar seus estudos. |

| 2. Planejamento | Cadastra uma atividade e define quando pretende realizá-la. |

| 3. Lembrete | Recebe uma notificação próxima ao horário planejado. |

| 4. Realização da tarefa | Realiza a atividade e registra sua conclusão. |

| 5. Gamificação | Recebe pontos de experiência (XP) e mantém sua sequência de estudos. |

| 6. Acompanhamento | Consulta seu progresso e identifica oportunidades de melhoria. |

| 7. Replanejamento | Altera o planejamento quando surgem imprevistos. |



\### 4.1. Jornada selecionada para o MVP



Considerando o caráter experimental da PoC, a equipe selecionou o fluxo central de organização dos estudos:



1\. Cadastrar uma disciplina.

2\. Selecionar ou cadastrar um tipo de atividade.

3\. Agendar a atividade para uma data planejada.

4\. Consultar a atividade na agenda ou na lista de pendências.

5\. Marcar a atividade como concluída ou remarcar sua execução.



Essa jornada permite validar a integração entre a interface, as regras de negócio e o banco de dados.



As funcionalidades de notificações automáticas, XP, streaks e relatórios analíticos permanecem previstas para versões futuras.



\## 5. Funcionalidades implementadas



\### Gerenciamento de disciplinas

\- Cadastro de disciplinas.

\- Listagem de disciplinas cadastradas.

\- Exclusão de disciplinas sem atividades vinculadas.

\- Bloqueio de exclusão quando existem atividades associadas.



\### Tipos de atividade

\- Cadastro de tipos reutilizáveis, como Estudar, Fazer Exercícios e Assistir Aula.

\- Seleção de tipos previamente cadastrados.

\- Exclusão protegida de tipos de atividade.



\### Agendamento

\- Associação de atividades a disciplinas.

\- Definição da data planejada.

\- Armazenamento das atividades no banco de dados.



\### Agenda acadêmica

\- Visualização diária.

\- Visualização semanal.

\- Identificação de atividades atrasadas.

\- Contadores de atividades pendentes e concluídas.



\### Gerenciamento de atividades

\- Conclusão de atividades.

\- Cancelamento e reabertura.

\- Remarcação da data planejada.

\- Exclusão de atividades agendadas.

\- Consulta de todas as atividades pendentes, independentemente da data.



\## 6. Tecnologias utilizadas



| Tecnologia | Aplicação |

| C# | Desenvolvimento da lógica do sistema |

| ASP.NET Core | Framework da aplicação web |

| Razor Pages | Construção das páginas e integração entre interface e backend |

| HTML | Estrutura das páginas |

| CSS | Estilização e adaptação visual |

| MySQL | Banco de dados relacional |

| MySqlConnector | Comunicação entre a aplicação C# e o MySQL |

| Visual Studio 2026 | Ambiente de desenvolvimento |

| Git e GitHub | Versionamento e armazenamento do código-fonte |



\## 7. Arquitetura do sistema



O StudyUp utiliza uma arquitetura de aplicação web baseada em ASP.NET Core Razor Pages, com integração a um banco de dados relacional MySQL.



O funcionamento pode ser representado pelo seguinte fluxo:



\*\*Usuário → Interface Razor Pages → Lógica C# → MySqlConnector → Banco MySQL\*\*



\### Componentes principais



\*\*Frontend:\*\* páginas Razor, HTML e CSS responsáveis pela apresentação das informações e interação com o estudante.



\*\*Backend:\*\* classes C# associadas às páginas Razor, responsáveis por receber requisições, validar informações e executar operações de cadastro, consulta, atualização e exclusão.



\*\*Banco de dados:\*\* MySQL, responsável pela persistência das disciplinas, tipos de atividade e agendamentos.



\*\*Integração:\*\* biblioteca MySqlConnector, utilizada para executar comandos SQL parametrizados.



O projeto foi desenvolvido como uma aplicação integrada, sem a implementação de uma API REST independente nesta versão.



\## 8. Modelagem do banco de dados



O banco de dados `studyup` possui três tabelas principais.



\### disciplina



Armazena as disciplinas cadastradas.



| Campo | Tipo | Descrição |

| id | INT | Chave primária |

| nome | VARCHAR(100) | Nome da disciplina |



\### catalogo\_atividade



Armazena os tipos de atividade reutilizáveis.



| Campo | Tipo | Descrição |

| id | INT | Chave primária |

| nome | VARCHAR(100) | Nome do tipo de atividade |



\### atividade



Armazena os agendamentos realizados pelo estudante.



| Campo | Tipo | Descrição |

| id | INT | Chave primária |

| catalogo\_id | INT | Chave estrangeira para catalogo\_atividade |

| disciplina\_id | INT | Chave estrangeira para disciplina |

| data\_cadastro | TIMESTAMP | Data de cadastro |

| data\_planejada | DATE | Data prevista para a atividade |

| status | VARCHAR(30) | Situação da atividade |



\### Relacionamentos



\- Uma disciplina pode possuir várias atividades agendadas.

\- Um tipo de atividade pode ser utilizado em vários agendamentos.

\- Cada agendamento está associado a uma disciplina e a um tipo de atividade.



As chaves estrangeiras contribuem para a integridade dos dados e impedem a exclusão indevida de disciplinas ou tipos que ainda possuem atividades vinculadas.



\## 9. Como executar o projeto



\### Pré-requisitos



\- Visual Studio com suporte ao desenvolvimento ASP.NET Core.

\- SDK do .NET compatível com a versão definida no arquivo `StudyUp.csproj`.

\- MySQL Server.

\- MySQL Workbench, recomendado para administração do banco.

\- Acesso ao código-fonte do repositório.



\### Passo 1 — Obter o código



Clone o repositório:



```bash

git clone https://github.com/eduhmello-git/StudyUp.git

```



Ou baixe o código-fonte pela opção \*\*Code → Download ZIP\*\* do GitHub.



\### Passo 2 — Abrir o projeto



Abra a solução `StudyUp.slnx` no Visual Studio.



Aguarde a restauração das dependências do projeto.



\### Passo 3 — Preparar o banco de dados



Inicie o MySQL Server e execute o script de criação do banco e das tabelas disponibilizado com o projeto.



O banco utilizado pela aplicação deve se chamar `studyup`.



\### Passo 4 — Configurar a conexão



A aplicação utiliza a configuração `ConnectionStrings:StudyUp`.



No Visual Studio, configure a string de conexão utilizando o recurso \*\*Gerenciar Segredos do Usuário (User Secrets)\*\*.



Exemplo de estrutura:



```json

{

&#x20; "ConnectionStrings": {

&#x20;   "StudyUp": "Server=localhost;Port=3306;Database=studyup;User ID=SEU\_USUARIO;Password=SUA\_SENHA;"

&#x20; }

}

```



Substitua os valores de exemplo pelas credenciais do seu ambiente MySQL.



\*\*Importante:\*\* não publique senhas reais no GitHub nem as adicione aos arquivos de configuração versionados.



\### Passo 5 — Executar



Com o banco de dados configurado e o servidor MySQL em execução:



1\. Abra o projeto no Visual Studio.

2\. Compile a solução.

3\. Execute a aplicação.

4\. Acesse o endereço local informado pelo Visual Studio.

5\. Cadastre uma disciplina e um tipo de atividade para iniciar os testes.



\## 10. Testes e validação



Durante o desenvolvimento, foram realizados testes manuais das principais funcionalidades do MVP.



Entre os cenários verificados estão:



\- Cadastro e listagem de disciplinas.

\- Cadastro e reutilização de tipos de atividade.

\- Agendamento e persistência de atividades.

\- Consulta das agendas diária e semanal.

\- Visualização de pendências independentemente da data.

\- Conclusão e remarcação de atividades.

\- Exclusão de atividades.

\- Bloqueio da exclusão de disciplinas e tipos de atividade com vínculos existentes.

\- Preservação da visualização selecionada após operações de atualização.



Os testes realizados permitiram identificar e corrigir problemas de navegação e confirmar o funcionamento das operações integradas ao banco de dados.



\## 11. Limitações e melhorias futuras



O MVP foi desenvolvido com escopo reduzido para validar o fluxo principal do produto.



As seguintes funcionalidades permanecem previstas para versões futuras:



\- Autenticação e contas individuais de estudantes.

\- Notificações e lembretes automáticos.

\- Gamificação com XP, níveis e sequências de estudo.

\- Relatórios de produtividade e constância.

\- Histórico de remarcações e análises de planejamento.

\- Recursos adicionais de personalização.



\## 12. Equipe



O StudyUp é um projeto acadêmico desenvolvido com participação dos seguintes integrantes:



\- Heloisa de Oliveira Sabino

\- Jose Eduardo de Melo Junior

\- Tatiane Grace Lemes Ferreira

\- Guilherme Rodrigues Silva

\- Raissa Ximenes Lima



\### Organização do desenvolvimento



A definição da proposta, o levantamento de requisitos e as decisões sobre o produto foram realizados coletivamente, por meio de discussões e reuniões da equipe.



A implementação técnica do MVP foi centralizada em um integrante e realizada em um único ambiente de desenvolvimento, abrangendo frontend, backend e integração com o banco de dados.



Essa organização permitiu conciliar as decisões coletivas com a construção e validação de um protótipo funcional.



\## 13. Considerações finais



O desenvolvimento do StudyUp demonstrou a viabilidade técnica de uma solução web para organização de atividades acadêmicas.



A Prova de Conceito permitiu transformar parte da jornada originalmente planejada em um sistema funcional, integrando interface, regras de negócio e persistência de dados.



Embora ainda não contemple todas as funcionalidades propostas para o produto final, o MVP estabelece uma base para futuras evoluções, especialmente nas áreas de gamificação, notificações e análise do comportamento de estudo.

