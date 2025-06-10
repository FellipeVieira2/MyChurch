namespace MyChurch.Application.Dtos
{
    public class BankingInfoDto
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string Agency { get; set; }
        public string Account { get; set; }
        public string AccountType { get; set; }
        public string HolderName { get; set; }
        public string HolderDocument { get; set; }
        public string PixKey { get; set; }
        public string PixKeyType { get; set; }

        public static BankingInfoDto New(Domain.Entities.BankingInfo info)
        {
            return new BankingInfoDto
            {
                Id = info.Id,
                BankName = info.BankName,
                Agency = info.Agency,
                Account = info.Account,
                AccountType = info.AccountType,
                HolderName = info.HolderName,
                HolderDocument = info.HolderDocument,
                PixKey = info.PixKey,
                PixKeyType = info.PixKeyType
            };
        }
    }
}