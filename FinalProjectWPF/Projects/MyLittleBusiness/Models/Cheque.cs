namespace FinalProjectWPF.Projects.MyLittleBusiness.Models
{
    class Cheque
    {
        public int ChequeNumber { get; set; }
        public int BankNumber { get; set; }
        public int SnifNumber { get; set; }
        public long AccountNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Sum { get; set; }
        public Cheque(int chequeNumber, int bankNumber, int snifNumber, long accountNumber, DateTime date, decimal sum)
        {
            ChequeNumber = chequeNumber;
            BankNumber = bankNumber;
            SnifNumber = snifNumber;
            AccountNumber = accountNumber;
            Date = date;
            Sum = sum;
        }
    }
}
