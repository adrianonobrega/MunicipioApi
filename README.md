<h1>🌎 MUNICÍPIO API: DESAFIO TÉCNICO DEV FULLSTACK .NET</h1>

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

<h2>4. Paginação do Lado do Servidor</h2>

A lógica de paginação (PaginationParams e ToPagedResponse) foi movida para o Service Layer (Providers), garantindo que a lista retornada ao Controller já esteja formatada corretamente.

<h1>📦 COMO EXECUTAR O PROJETO</h1>

<h2>Pré-requisitos</h2>

.NET 8 SDK

Docker (Opcional, mas recomendado)

<h2>1. Variáveis de Ambiente</h2>

O projeto requer a variável de ambiente IBGE_PROVIDER_TYPE configurada para um dos seguintes valores:
| Valor | Provider Usado |
| :--- | :--- |
| IBGE | Serviço oficial do IBGE. |
| BRASILAPI | Brasil API. |

<h2>2. Execução via Docker (Recomendada)</h2>

Construir a Imagem: Navegue até a raiz do projeto (onde está o Dockerfile).

docker build -t municipioapi .


Rodar o Container: Mapeie a porta 5000 do host para a porta 8080 do container e defina o Provider.

docker run -d -p 5000:8080 -e "IBGE_PROVIDER_TYPE=BRASILAPI" --name municipio-app municipioapi


A API estará disponível em http://localhost:5000.

<h2>3. Execução Local</h2>

dotnet run --project MunicipioApi.Api


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

🧪 Testes

O projeto contém testes unitários e de integração para garantir a manutenibilidade e a funcionalidade.

Testes Unitários

Foco: Lógica de mapeamento (Provider -> MunicipioResponse), Lógica de Cache (HIT/MISS) e funções de paginação.

Testes de Integração

Foco: Testar o pipeline HTTP completo: injeção dinâmica do provider, resposta 200 OK com paginação, e a correta ativação do filtro de exceção para retornar 503.

