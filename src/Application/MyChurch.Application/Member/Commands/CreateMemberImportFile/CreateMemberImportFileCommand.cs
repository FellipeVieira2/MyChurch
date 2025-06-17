using Amazon.Runtime.Internal;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyChurch.Application.Dtos;
using MyChurch.Application.Member.Commands.CreateMember;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Entities;
using System.Globalization;
using static MyChurch.Application.Church.Commands.CreateChurchWithAdminMember.CreateChurchWithAdminMemberCommand;

namespace MyChurch.Application.Member.Commands.CreateMemberImportFile
{
    public class CreateMemberImportFileCommand : JwtMemberDto, IRequest<List<int>>
    {
        public IFormFile CsvFile { get; set; }
    }
    public class CreateMemberImportFileCommandHandler : IRequestHandler<CreateMemberImportFileCommand, List<int>>
    {
        private readonly IMediator _mediator;
        public CreateMemberImportFileCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<List<int>> Handle(CreateMemberImportFileCommand request, CancellationToken cancellationToken)
        {
            var result = new List<int>();
            try
            {
                using var stream = request.CsvFile.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HeaderValidated = null,
                    MissingFieldFound = null
                });
                csv.Context.RegisterClassMap<MemberCsvImportModelMap>();
                var records = csv.GetRecords<MemberCsvImportModel>().ToList();
                if (!records.Any())
                    throw new Exception("Nenhum registro encontrado no CSV. Verifique o cabeçalho e o conteúdo do arquivo.");
                foreach (var record in records)
                {
                    try
                    {
                        var command = record.ToCreateMemberCommand();
                        if (command.Name == "Quercio Goes Santos ")
                        {
                            var teste = "x";
                        }
                        command.UserId = request.UserId;
                        var id = await _mediator.Send(command, cancellationToken);
                        result.Add(id);
                    }
                    catch { continue; }
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar o arquivo CSV: {ex.Message}", ex);
            }
            return result;
        }
    }

    public class MemberCsvImportModel
    {
        public string Membro { get; set; }
        public string Sexo { get; set; }
        public string DataDeNascimento { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public string TituloDeEleitor { get; set; }
        public string EstadoCivil { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public string CidadeNaturalidade { get; set; }
        public string EstadoNaturalidade { get; set; }
        public string Contato { get; set; }
        public string CargoNaIgreja { get; set; }
        public string BatismoNaAguas { get; set; }
        public string Ativo { get; set; }

        private string ParseMaritalStatus(string estadoCivil)
        {
            if (string.IsNullOrWhiteSpace(estadoCivil))
                return null;
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Solteiro", "Solteiro" },
                { "Solteira", "Solteiro" },
                { "Casado", "Casado" },
                { "Casada", "Casado" },
                { "Divorciado", "Divorciado" },
                { "Divorciada", "Divorciado" },
                { "Viúvo", "Viuvo" },
                { "Viúva", "Viuvo" },
                { "Viuvo", "Viuvo" },
                { "Viuva", "Viuvo" }
            };
            if (map.TryGetValue(estadoCivil.Trim(), out var value))
                return value;
            return null;
        }

        private string NormalizeDigits(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return new string(value.Where(char.IsDigit).ToArray());
        }

        private DateTime ParseBirthDate(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return DateTime.MinValue;
            string[] formats = { "M/d/yyyy", "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(data, formats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt))
                return dt;
            if (DateTime.TryParse(data, out dt))
                return dt;
            return DateTime.MinValue;
        }

        public CreateMemberCommand ToCreateMemberCommand()
        {
            var documents = new List<CreateMemberCommand.MemberDocumentDtoCreate>();
            var normalizedCPF = NormalizeDigits(CPF);
            var normalizedRG = NormalizeDigits(RG);
            var normalizedTitulo = NormalizeDigits(TituloDeEleitor);
            if (!string.IsNullOrWhiteSpace(normalizedCPF))
            {
                documents.Add(new CreateMemberCommand.MemberDocumentDtoCreate
                {
                    Type = MemberDocumentType.CPF,
                    Number = normalizedCPF
                });
            }
            if (!string.IsNullOrWhiteSpace(normalizedRG))
            {
                documents.Add(new CreateMemberCommand.MemberDocumentDtoCreate
                {
                    Type = MemberDocumentType.RG,
                    Number = normalizedRG
                });
            }
            if (!string.IsNullOrWhiteSpace(normalizedTitulo))
            {
                documents.Add(new CreateMemberCommand.MemberDocumentDtoCreate
                {
                    Type = MemberDocumentType.TituloEleitor,
                    Number = normalizedTitulo
                });
            }
            return new CreateMemberCommand
            {
                Name = Membro,
                Phone = Contato,
                BirthDate = ParseBirthDate(DataDeNascimento),
                IsBaptized = !string.IsNullOrEmpty(BatismoNaAguas),
                IsActive = Ativo == "X",
                MaritalStatus = ParseMaritalStatus(EstadoCivil),
                BirthCity = CidadeNaturalidade,
                BirthState = EstadoNaturalidade,
                BaptizedDate = BatismoNaAguas is not null ? ParseBirthDate(BatismoNaAguas) : null,
                RoleMember = ParseUserRole(CargoNaIgreja),
                Address = string.IsNullOrWhiteSpace(Endereco) ? null : new AddressChurchWithAdminCreate()
                {
                    Street = Endereco ?? string.Empty,
                    City = Cidade ?? string.Empty,    
                    State = "SP",                    
                    ZipCode = NormalizeDigits(CEP),   
                    Country = "Brasil",            
                    Neighborhood = Bairro ?? string.Empty,
                    Number = ExtractNumber(Endereco) ?? string.Empty 
                },
                Documents = documents
            };
        }

        private string ExtractNumber(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco)) return string.Empty;
            var parts = endereco.Split(' ');
            return parts.Length > 1 && int.TryParse(parts.Last(), out _) ? parts.Last() : string.Empty;
        }

        private UserRole ParseUserRole(string cargo)
        {
            if (string.IsNullOrWhiteSpace(cargo))
                return UserRole.Member;

            // Mapeamento explícito dos cargos em português para o enum UserRole
            var map = new Dictionary<string, UserRole>(StringComparer.OrdinalIgnoreCase)
            {
                { "Pastor", UserRole.Minister },
                { "Presbitero", UserRole.Elder },
                { "Presbítero", UserRole.Elder },
                { "Diacono", UserRole.Deacon },
                { "Diácono", UserRole.Deacon },
                { "Obreiro", UserRole.Worker },
                { "Obreira", UserRole.Worker },
                { "Tesoureiro", UserRole.Leader }, // Ajuste conforme necessidade
                { "Cooperador", UserRole.Worker }, // Ajuste conforme necessidade
                { "Membro", UserRole.Member },
                { "Visitante", UserRole.Visitor }
            };

            if (map.TryGetValue(cargo.Trim(), out var role))
                return role;

            // fallback para enum parsing padrão
            if (Enum.TryParse<UserRole>(cargo.Replace(" ", ""), true, out var parsedRole))
                return parsedRole;

            return UserRole.Member;
        }
    }

    public class MemberCsvImportModelMap : ClassMap<MemberCsvImportModel>
    {
        public MemberCsvImportModelMap()
        {
            Map(m => m.Membro).Name("Membro");
            Map(m => m.Sexo).Name("Sexo");
            Map(m => m.DataDeNascimento).Name("Data de nascimento", "DataDeNascimento");
            Map(m => m.RG).Name("RG");
            Map(m => m.CPF).Name("CPF");
            Map(m => m.TituloDeEleitor).Name("Titulo de Eleitor", "TituloDeEleitor");
            Map(m => m.EstadoCivil).Name("Estado civil", "EstadoCivil");
            Map(m => m.Endereco).Name("Endereco", "Endereço");
            Map(m => m.Cidade).Name("Cidade");
            Map(m => m.Bairro).Name("Bairro");
            Map(m => m.CEP).Name("CEP");
            Map(m => m.CidadeNaturalidade).Name("Cidade Naturalidade", "CidadeNaturalidade");
            Map(m => m.EstadoNaturalidade).Name("Estado Naturalidade", "EstadoNaturalidade");
            Map(m => m.Contato).Name("Contato");
            Map(m => m.CargoNaIgreja).Name("Cargo na Igreja", "CargoNaIgreja");
            Map(m => m.BatismoNaAguas).Name("Batismo na águas", "BatismoNaAguas");
            Map(m => m.Ativo).Name("Ativo");
        }
    }
}