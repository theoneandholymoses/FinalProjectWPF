namespace FinalProjectWPF.Projects.MyLittleBusiness.Models
{
    class Customer
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<int> Invoices { get; set; }
        public Dictionary<int, bool> Orders { get; set; }

        public Customer() { }

        public Customer(string fullName, string email, string phone)
        {
            FullName = fullName;
            Email = email;
            Phone = phone;
            Invoices = new List<int>();
            Orders = new Dictionary<int, bool>();
        }

        public Customer(int customerID, string fullName, string email, string phone, Invoice invoice)
        {
            CustomerID = customerID;
            FullName = fullName;
            Email = email;
            Orders = new Dictionary<int, bool>();
            Phone = phone;
            Invoices = new List<int>();
            AddInvoice(invoice);
        }

        public void AddInvoice(Invoice invoice)
        {
            if (invoice.InvoiceType == 0)
            {
                Orders.Add(invoice.InvoiceID, false);
            }
            else
            {
                Invoices.Add(invoice.InvoiceID);
            }
        }

        public void ApproveOder(int id)
        {
            Orders[id] = true;
        }
    }
}
