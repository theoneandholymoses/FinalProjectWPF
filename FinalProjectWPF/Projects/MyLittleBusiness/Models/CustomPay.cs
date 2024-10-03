namespace FinalProjectWPF.Projects.MyLittleBusiness.Models
{
    class CustomPay
    {
        public int TransactionID { get; set; }
        public DateTime TranDate { get; set; }
        public string Description { get; set; }
        public string Asmacta { get; set; }
        public decimal Sum { get; set; }

        public CustomPay(int transactionID, string asmachta, decimal sum)
        {
            TransactionID = transactionID;
            TranDate = DateTime.Now;
            Description = transactionID == 28 ? "Payment from Bit" : "Bank transfer";
            Asmacta = asmachta;
            Sum = sum;
        }
    }
}
