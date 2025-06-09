using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class MemberDocumentDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public MemberDocumentType Type { get; set; } // Ex: "CPF", "RG", "Título de Eleitor"
        public string Number { get; set; }

        public static MemberDocumentDto New(MemberDocument memberDocument)
        {
            return new MemberDocumentDto()
            {
                Id = memberDocument.Id,
                MemberId = memberDocument.MemberId,
                Type = memberDocument.Type,
                Number = memberDocument.Number
            };
        }
    }
}
