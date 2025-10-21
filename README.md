<h1>🌎 MUNICÍPIO API</h1>

API RESTful desenvolvida em ASP.NET Core 8 para pesquisar e listar municípios brasileiros por Unidade Federativa (UF), utilizando provedores de dados externos e implementando padrões avançados de arquitetura e otimização.

<h1>🚀 ARQUITETURA E DECISÕES TÉCNICAS</h1>

O projeto segue os princípios da Arquitetura Limpa (Clean/Onion Architecture) com ênfase na Separação de Preocupações (SoC) e Injeção de Dependência (DI).

<h2>1. Padrão Decorator (Cache)</h2>

O requisito de cache foi implementado usando o Padrão Decorator.

O serviço de cache (CachedIbgeProvider) envolve o provedor de dados real (IBGE ou Brasil API).

O Controller injeta o Decorator (IIbgeProvider), recebendo o cache de forma transparente.

O cache armazena a lista completa de municípios por UF, garantindo que o hit de cache seja rápido e independente da página solicitada.

<h2>2. Provider Dinâmico</h2>

O provedor de dados (BrasilApiProvider ou IbgeApiProvider) é selecionado dinamicamente durante a inicialização (Program.cs) com base na variável de ambiente IBGE_PROVIDER_TYPE.

O Controller e o Cache dependem apenas da interface IIbgeProvider, isolando a aplicação da fonte de dados externa.

<h2>3. Tratamento Global de Exceções</h2>

Para garantir a qualidade e o princípio DRY (Don't Repeat Yourself):

O Controller é um Thin Controller (sem blocos try-catch).

Falhas de API externa (HttpResponseMessage não-sucesso) lançam uma exceção customizada: ProviderIndisponivelException.

Um Filtro de Exceção Global (ProviderExceptionFilter) intercepta esta exceção e mapeia-a automaticamente para o código HTTP 503 Service Unavailable, garantindo uma resposta consistente e semântica.

<h1>📈 ESTRATÉGIA DE CRESCIMENTO E ESCALABILIDADE</h1>

O projeto foi estruturado para suportar a adição de novos endpoints e funcionalidades de forma modular, minimizando o risco de regressão.

<h2>Adição de Novos Endpoints</h2>

Qualquer nova funcionalidade deve seguir a Estrutura de Camadas existente para manter a coesão:

Interface de Serviço (Services/Interfaces): Crie uma nova interface (ex: ICidadeService) que define o contrato da nova funcionalidade.

Implementação de Serviço (Services/Implementation): Crie a classe que implementa a lógica de negócio (ex: CidadeService).

Novo Controller (Controllers): Crie um novo Controller (ex: CidadesController) que injeta a nova interface de serviço.

Injeção de Dependência (Program.cs): Registre a nova interface e sua implementação no pipeline de DI (ex: builder.Services.AddScoped<ICidadeService, CidadeService>()).

<h2>Expansão de Provedores Externos</h2>

Para adicionar um novo provedor (ex: dados populacionais, clima, etc.), utilize o Padrão de Provedor atual:

Crie uma nova interface (ex: IClimaProvider).

Crie as classes de implementação (ex: OpenWeatherProvider).

A lógica de injeção dinâmica deve ser replicada para o novo Provider, permitindo alternar a fonte de dados via variável de ambiente, se necessário.

<h1>📦 COMO EXECUTAR O PROJETO</h1>

<h2>Pré-requisitos</h2>

.NET 8 SDK - https://dotnet.microsoft.com/pt-br/download/dotnet/8.0

Docker - https://www.docker.com/products/docker-desktop/

git - https://git-scm.com/install/

<h2>Clonar o Repositório</h2>

Para iniciar o trabalho, clone o projeto usando o Git:

git clone https://github.com/adrianonobrega/MunicipioApi.git

cd MunicipioApi


<h2>1. Variáveis de Ambiente</h2>

O projeto requer a variável de ambiente <strong>IBGE_PROVIDER_TYPE</strong> configurada para um dos seguintes valores:
| Valor | Provider Usado |
| :--- | :--- |
| IBGE | Serviço oficial do IBGE. |
| BRASILAPI | Brasil API. |

Configurar o valor da variável no appsettings

<img width="418" height="204" alt="image" src="https://github.com/user-attachments/assets/38d178e5-0e93-435f-bc9f-93a588c29c79" />

No exemplo acima estamos usando o BRASIL_API, se quisermos usar o provider IBGE_API, comente a linha 11 e descomente a linha 12.

<h2>2. Execução via Docker (Recomendada)</h2>

Construir a Imagem: Navegue até a raiz do projeto (onde está o Dockerfile).

docker build -t municipioapi .


Rodar o Container: Mapeie a porta 5000 do host para a porta 8080 do container e defina o Provider.

docker run -d -p 5000:8080 -e "IBGE_PROVIDER_TYPE=BRASILAPI" --name municipio-app municipioapi


A API estará disponível em http://localhost:5000.

<h2>3. Execução Local</h2>

dotnet run --project MunicipioApi


<h1>📋 ENDPOINT DA API</h1>

<h2>Listar Municípios por UF</h2>

Retorna uma lista paginada de municípios para a UF solicitada.

Método: GET
URI: /api/municipios/{uf}

<h3>Parâmetros de Rota ({uf})</h3>

<img width="720" height="106" alt="image" src="https://github.com/user-attachments/assets/aee90743-69e3-4811-8fe5-822ffd74e22e" />


Parâmetros de Query (Paginação)

<img width="880" height="156" alt="image" src="https://github.com/user-attachments/assets/d1ea08a8-257c-452b-a799-47d321961c0e" />

Respostas (Exemplos)

<img width="1027" height="278" alt="image" src="https://github.com/user-attachments/assets/9a9b2579-5774-4d3f-8c81-f961ae8c9ef3" />


Estrutura do PagedResponse

<img width="340" height="268" alt="image" src="https://github.com/user-attachments/assets/bc1ba931-7162-41be-a468-d71f879a1e21" />

<h1>🧪 TESTES E QUALIDADE</h1>

<h2>Executando os Testes</h2>

Para rodar todos os testes unitários e de integração do projeto, utilize o comando dotnet test na raiz do seu projeto (onde o arquivo .sln está localizado):

cd Tests

dotnet test


<h2>Testes Unitários</h2>

Foco: Lógica de mapeamento (Provider -> MunicipioResponse), Lógica de Cache (HIT/MISS) e funções de paginação.

<h2>Testes de Integração</h2>

Foco: Testar o pipeline HTTP completo: injeção dinâmica do provider, resposta 200 OK com paginação, e a correta ativação do filtro de exceção para retornar 503.


<h1>🚀 Teste Rápido - Consulta de Municípios por UF</h1>

Para testar a API de forma interativa sem sair da documentação, use o arquivo Municipio.html. Este arquivo demonstra o consumo do endpoint de consulta de municípios por Unidade Federativa (UF) com paginação, utilizando a URL da API e os parâmetros de paginação.

<h2>🔗 URL Base da API</h2>

A URL base é definida no script do arquivo Municipio.html e possui duas opções:

<img width="612" height="112" alt="image" src="https://github.com/user-attachments/assets/3700064e-77f7-47c2-bd41-c8ecd2fef274" />

<h2>💡 Observações de Funcionamento</h2>

Alternando entre Ambientes (Local vs. Produção): Para testar a API localmente, você deve editar o arquivo Municipio.html e seguir a convenção definida no código:

<img width="667" height="120" alt="image" src="https://github.com/user-attachments/assets/94884e9e-5b45-4f2c-b4d6-a58f96ad1ce0" />

Basta comentar a linha da URL hospedada (Produção) e descomentar a linha da URL local:

<img width="664" height="112" alt="image" src="https://github.com/user-attachments/assets/b22321a9-f12a-4c20-88f1-a0b39ed0b78e" />

O script utiliza as variáveis de estado (state.currentPage, state.pageSize, state.currentUf) para gerenciar a paginação e a busca.

A função fetchMunicipios() constrói a URL e faz a requisição à API.

Se estiver usando a URL hospedada (Render), o tempo de resposta pode ser maior na primeira requisição após um período de inatividade.

