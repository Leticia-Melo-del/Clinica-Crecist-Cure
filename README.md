# Clinica Crescit Cura

Sistema de gerenciamento para uma clínica infantojuvenil desenvolvido em C# com .NET, executado via console e com persistência local em arquivos JSON.

O projeto organiza o fluxo de atendimento em três perfis principais:

- `Responsável`: cadastro, agendamento, visualização de consultas, prontuários e gerenciamento de pacientes vinculados.
- `Médico`: agenda profissional, acesso aos dados do paciente e preenchimento de prontuários.
- `Administrador`: gestão de usuários, consultas, pagamentos e corpo clínico.

## Visão Geral

O sistema simula a rotina de uma clínica particular, cobrindo desde o cadastro do responsável legal até o fechamento da consulta. A aplicação carrega dados iniciais para administradores e médicos, permitindo testar os fluxos principais sem preparação manual do ambiente.

Entre os recursos disponíveis:

- cadastro de responsáveis e pacientes
- autenticação por perfil
- agendamento de consultas com validação de conflito de horário
- cancelamento de consultas com confirmação de senha
- registro e consulta de prontuários
- visualização de histórico clínico
- gestão administrativa de pagamentos
- cadastro de novos médicos pelo administrador
- persistência local em JSON

## Tecnologias

- `C#`
- `.NET 10`
- `System.Text.Json` para persistência
- aplicação `Console`

## Estrutura do Projeto

```text
Clinica Crescit Cura/
|-- Dados/           # arquivos JSON com os dados persistidos
|-- Entidades/       # modelos de domínio
|-- Repositorios/    # acesso e persistência dos dados
|-- Servicos/        # regras de negócio e validações
|-- UI/              # menus e interação via console
|-- Program.cs       # ponto de entrada da aplicação
```

## Perfis e Funcionalidades

### Responsável

- realizar cadastro com dados do responsável e do paciente
- marcar consultas
- adicionar observações para o médico no momento do agendamento
- visualizar consultas marcadas e comprovante
- cancelar consulta com confirmação de senha
- consultar diagnósticos e resultados
- editar dados do responsável e dos pacientes
- adicionar novos filhos ao cadastro
- apagar a conta quando não houver consultas pendentes

### Médico

- visualizar agenda profissional
- consultar detalhes clínicos do paciente
- acessar observações informadas pelo responsável
- registrar diagnóstico e recomendações no prontuário

### Administrador

- visualizar usuários cadastrados
- listar consultas agendadas
- acompanhar pagamentos pendentes e realizados
- registrar baixa de pagamento
- consultar o corpo clínico
- cadastrar novos médicos

## Persistência de Dados

Os dados são armazenados localmente em arquivos JSON dentro da pasta `Dados`:

- `administradores.json`
- `cadastros.json`
- `consultas.json`
- `medicos.json`

Isso torna o projeto simples de executar e fácil de demonstrar em ambiente acadêmico ou de portfólio, sem dependência obrigatória de banco de dados.

## Exportação para MySQL

O projeto já possui um serviço chamado `MySqlSyncService`, responsável por gerar um script SQL com base nos dados atuais da aplicação. Essa funcionalidade serve como ponte para uma futura migração da persistência em JSON para MySQL.

No estado atual, a aplicação principal continua operando com arquivos JSON.

## Pré-requisitos

- SDK do `.NET 10` instalado

Para verificar:

```bash
dotnet --version
```

## Como Executar

Na raiz do projeto:

```bash
dotnet build "Clinica Crescit Cura.sln"
dotnet run --project "Clinica Crescit Cura.csproj"
```

## Credenciais Iniciais de Demonstração

Ao iniciar a aplicação, um administrador padrão e uma lista inicial de médicos são carregados automaticamente caso os arquivos de dados estejam vazios.

### Administrador

- E-mail: `admin@clinicacrescitcura.com`
- Senha: `Admin123`

### Médicos

Os médicos iniciais utilizam a senha padrão:

- Senha: `Medico123`

Exemplos de login:

- `ana.silva@clinicacrescitcura.com`
- `marcos.oliveira@clinicacrescitcura.com`
- `beatriz.costa@clinicacrescitcura.com`

## Fluxo Principal da Aplicação

1. O usuário escolhe entre cadastro ou login.
2. No cadastro, o responsável informa seus dados e vincula pelo menos um paciente.
3. Após login, o sistema redireciona para o menu correspondente ao perfil autenticado.
4. Cada perfil acessa operações específicas conforme sua função na clínica.

## Objetivo do Projeto

Este projeto demonstra conceitos importantes de desenvolvimento backend e orientação a objetos, como:

- separação por camadas
- encapsulamento de regras de negócio
- persistência em arquivos
- autenticação simples
- manipulação de coleções e relacionamentos
- organização de fluxo por perfil de usuário

## Observações

- a aplicação é executada inteiramente no terminal
- os dados persistidos ficam salvos localmente no repositório durante o uso
- por utilizar arquivos JSON, o projeto é ideal para estudo, apresentação e evolução incremental

## Próximas Evoluções Sugeridas

- integração completa com banco de dados relacional
- criação de testes automatizados
- interface gráfica ou aplicação web
- controle de permissões mais robusto
- relatórios e dashboards administrativos

