namespace MyChurch.Domain.Entities
{
    public class MemberDocument
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public MemberDocumentType Type { get; set; } // Ex: "CPF", "RG", "Título de Eleitor"
        public string Number { get; set; }
    }
}