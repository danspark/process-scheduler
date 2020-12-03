# Process Scheduler

- Implementação em C# de um escalonador de processos.

## Dependências

- .NET 5.0

## Como utilizar

Rode o comando: `scheduler -a <fifo|sjf|rr|garantido|loteria> -p "local do arquivo" -q <250| apenas para algorítmos interativos> -s <true|false|modo interativo`

## Como baixar

1. Baixando o último release: [aqui](https://github.com/danspark/process-scheduler/releases)
2. Clonando o repositório (caso tenha o [dotnet](https://dotnet.microsoft.com/download) instalado):
   1. `git clone https://github.com/danspark/process-scheduler`
   2. Build: `dotnet publish -c Release -r win-x64`
