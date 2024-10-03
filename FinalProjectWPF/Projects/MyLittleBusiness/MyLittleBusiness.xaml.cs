using FinalProjectWPF.FileManagment;
using FinalProjectWPF.Projects.MyLittleBusiness.Helpers;
using FinalProjectWPF.Projects.MyLittleBusiness.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FinalProjectWPF.Projects.MyLittleBusiness
{
    /// <summary>
    /// Interaction logic for MyLittleBusiness.xaml
    /// </summary>
    public partial class MyLittleBusiness : Page
    {
        ApiManager AM = new ApiManager();
        BusinessFileManager FM = new BusinessFileManager();
        ObservableCollection<Invoice> invoices;
        ObservableCollection<Customer> customers;
        ObservableCollection<Item> items;
        public MyLittleBusiness()
        {
            InitializeComponent();

            invoices = FM.GetAllInvoices();
            DocumentsData.ItemsSource = invoices;

            items = FM.GetAllItems();
            ItemsData.ItemsSource = items;
            ItemsPopData.ItemsSource = items;


            customers = FM.GetAllCustomers();
            CustomersData.ItemsSource = customers;
            CustomerPopData.ItemsSource = customers;

        }
        private async Task SendInvoiceButton()
        {
            int invoiceType;
            char invoiceTypeKind;
            Grid ItemsGrid = chargeBUT.IsChecked == true ? InvoiceItemLines : OrderItemLines;
            Grid ChuqueGrid = chargeBUT.IsChecked == true ? ChequeLines : ChequeLines;
            if (chargeBUT.IsChecked == true && IsCharge.IsChecked == true)
            {
                invoiceType = 1;
                invoiceTypeKind = 'I';
            }
            else if (chargeBUT.IsChecked == true && IsRefund.IsChecked == true)
            {
                invoiceType = 2;
                invoiceTypeKind = 'I';
            }
            else
            {
                invoiceType = 100;
                invoiceTypeKind = 'O';
            }


            if (!ValidateInvoice(invoiceType, invoiceTypeKind))
            {
                return;
            }

            decimal TotalInvoiceSum = 0;
            List<Item> thisItems = new List<Item>();
            List<Cheque> thisCheques = new List<Cheque>();
            List<CustomPay> thisCustomPays = new List<CustomPay>();
            string ItemsType = $"{invoiceTypeKind}Prod";
            int nextItemID = 0;
            string CustomerType = $"{invoiceTypeKind}customer";
            decimal? cardSum = !string.IsNullOrEmpty(CardSumToBill.Text) &&
                   decimal.TryParse(CardSumToBill.Text, out decimal cardSumToBill)
                   ? cardSumToBill
                   : (decimal?)null;

            Customer thisCustomer = new Customer
                (
                    ((TextBox)this.FindName($"{CustomerType}Name")).Text,
                    ((TextBox)this.FindName($"{CustomerType}Email")).Text,
                    ((TextBox)this.FindName($"{CustomerType}Phone")).Text
                );

            foreach (UIElement element in ItemsGrid.Children)
            {
                // Cast the element to a Grid (or the specific container type)
                if (element is Grid g)
                {
                    // Extract item details by accessing the appropriate controls in the grid
                    TextBox itemIDBox = g.Children[1] as TextBox;
                    TextBox itemNameBox = g.Children[2] as TextBox;
                    TextBox itemPriceBox = g.Children[3] as TextBox;
                    TextBox itemQuantityBox = g.Children[4] as TextBox;
                    TextBox itemTotalPriceBox = g.Children[5] as TextBox;

                    // If the item ID is null or empty, assign the next available ID
                    int itemid = string.IsNullOrEmpty(itemIDBox?.Text) ? (FM.GetNextItemID() + nextItemID++) : int.Parse(itemIDBox.Text);

                    // Parse and create a new Item object with the extracted values

                    thisItems.Add(new Item(
                        itemid,
                        itemNameBox.Text,
                        decimal.Parse(itemPriceBox.Text),
                        int.Parse(itemQuantityBox.Text)
                    ));

                    TotalInvoiceSum += decimal.Parse(itemTotalPriceBox.Text);
                }
            }
            foreach (UIElement element in ChuqueGrid.Children)
            {
                if (element is Grid g)
                {
                    // Ensure the children at the indices are TextBoxes
                    TextBox chequeNumberBox = g.Children[4] as TextBox;
                    TextBox bankNumberBox = g.Children[3] as TextBox;
                    TextBox snifNumberBox = g.Children[2] as TextBox;
                    TextBox accountNumberBox = g.Children[1] as TextBox;
                    TextBox chequeDateBox = g.Children[5] as TextBox;
                    TextBox chequeSumBox = g.Children[6] as TextBox;

                    // Parse the TextBox values, ensuring they're valid before adding to the list
                    if (!string.IsNullOrWhiteSpace(chequeSumBox.Text))
                    {
                        try
                        {
                            int chequeNumber = int.Parse(chequeNumberBox.Text);
                            int bankNumber = int.Parse(bankNumberBox.Text);
                            int snifNumber = int.Parse(snifNumberBox.Text);
                            long accountNumber = long.Parse(accountNumberBox.Text);
                            DateTime chequeDate = DateTime.Parse(chequeDateBox.Text);
                            decimal chequeSum = decimal.Parse(chequeSumBox.Text);

                            // Add the cheque to the list
                            thisCheques.Add(new Cheque(chequeNumber, bankNumber, snifNumber, accountNumber, chequeDate, chequeSum));

                        }
                        catch (FormatException ex)
                        {

                        }
                    }
                }
            }

            if (AsmachtaBit.Text != "" && TotalBitSum.Text != "")
            {
                thisCustomPays.Add(new CustomPay(28, AsmachtaBit.Text, decimal.Parse(TotalBitSum.Text)));
            };
            if (AsmachtaBank.Text != "" && TotalBankSum.Text != "")
            {
                thisCustomPays.Add(new CustomPay(31, AsmachtaBank.Text, decimal.Parse(TotalBankSum.Text)));
            };

            string response = await AM.CreateNewInvoice(
                invoiceType: invoiceType,
                customerName: thisCustomer.FullName,
                email: thisCustomer.Email,
                userMobile: thisCustomer.Phone,
                sendByEmail: true,
                language: "EN",
                items: thisItems,

                cardValidityMonth: cardSum > 0 && !string.IsNullOrEmpty(ExtDateMonth.Text) && int.TryParse(ExtDateMonth.Text, out int extDateMonth) ? extDateMonth : (int?)null,
                cardValidityYear: cardSum > 0 && !string.IsNullOrEmpty(ExtDateYear.Text) && int.TryParse(ExtDateYear.Text, out int extDateYear) ? extDateYear : (int?)null,
                identityNum: cardSum > 0 && !string.IsNullOrEmpty(CardOwnerId.Text) && long.TryParse(CardOwnerId.Text, out long cardOwnerId) ? cardOwnerId : (long?)null,
                cardNumber: cardSum > 0 && !string.IsNullOrEmpty(CardNumber.Text) && long.TryParse(CardNumber.Text, out long cardNumber) ? cardNumber : (long?)null,
                paymentsAmount: cardSum > 0 && !string.IsNullOrEmpty(Payments.Text) && int.TryParse(Payments.Text, out int paymentsAmount) ? paymentsAmount : (int?)null,
                cardSum: cardSum > 0 ? cardSum : (decimal?)null,
                securityCVV: cardSum > 0 && !string.IsNullOrEmpty(Cvv.Text) && int.TryParse(Cvv.Text, out int securityCVV) ? securityCVV : (int?)null,


                cheques: thisCheques.Count > 0 && thisCheques[0].Sum > 0 ? thisCheques : null,
                customPays: thisCustomPays.Count > 0 && thisCustomPays[0].Sum > 0 ? thisCustomPays : null,

                cashPay: !string.IsNullOrEmpty(TotalCash.Text) && decimal.TryParse(TotalCash.Text, out decimal cashPay) ? cashPay : (decimal?)null
            );

            Dictionary<string, string?> responseValues = await AM.ParseResponseOfChargeInvoice(response);

            thisCustomer.CustomerID = int.Parse(responseValues["CustomerID"]);
            Dictionary<int, int> ItemsToSaveOnInvoice = new Dictionary<int, int>();
            foreach (Item item in thisItems)
            {
                ItemsToSaveOnInvoice[item.ItemId] = item.Quantity;
            }
            Invoice thisInvoice = new Invoice(int.Parse(responseValues["InvoiceID"]), invoiceType, thisCustomer.CustomerID, ItemsToSaveOnInvoice, TotalInvoiceSum, responseValues["DealNumber"]);
            thisCustomer.AddInvoice(thisInvoice);

            List<Customer> customers = FM.GetAllCustomers().ToList();
            if (customers.Any(C => C.CustomerID == thisCustomer.CustomerID))
            {
                FM.UpdateCustomer(thisCustomer.CustomerID, thisInvoice);
            }
            else
            {
                FM.CreateCustomer(thisCustomer, thisInvoice);
            }

            List<Item> AllItems = FM.GetAllItems().ToList();
            foreach (Item I in thisItems)
            {
                bool itemExists = AllItems.Any(a => a.ItemId == I.ItemId);

                if (itemExists)
                {
                    FM.UpdateItem(I.ItemId, I.Name, I.Price);
                }
                else
                {
                    FM.CreateItem(I.ItemId, I.Name, I.Price);
                }
            }

            MessageBox.Show("successfully finish invoice send: " + responseValues["InvoiceID"]);
            // update message for user 

            // reset form 
        }

        public void CreateOrder()
        {

        }

        private void Button_Click_ManuNav(object sender, RoutedEventArgs e)
        {
            RadioButton clickedButton = sender as RadioButton;
            Style HiddenStyle = (Style)FindResource("HiddenStyle");
            Style VisibleStyle = (Style)FindResource("VisibleStyle");

            if (clickedButton != null)
            {
                switch (clickedButton.Name)
                {
                    case "chargingBUT":
                        {
                            CleanAllForms();
                            HeadTitle.Text = "Charge Or Refund Manualy";
                            Subtitle.Text = "";
                            ChargesNOrdersTabMenu.Visibility = Visibility.Visible;
                            ReportsTabMenu.Visibility = Visibility.Hidden;
                            ItemsTabMenu.Visibility = Visibility.Hidden;

                            chargeBUT.IsChecked = true;
                            ChargingScreen.Style = VisibleStyle;
                            OrderCreationScreen.Style = HiddenStyle;
                            ReportsTable.Style = HiddenStyle;

                            DocumentsTable.Style = VisibleStyle;
                            DocActionRefund.Visibility = Visibility.Visible;
                            DocActionReports.Visibility = Visibility.Hidden;
                            ItemsTable.Style = HiddenStyle;
                            CustomersTable.Style = HiddenStyle;
                            break;
                        }
                    case "reportsBUT":
                        {
                            CleanAllForms();
                            invoices = FM.GetAllInvoices();
                            HeadTitle.Text = "All Reports";
                            Subtitle.Text = "All Invoices";
                            ReportsTabMenu.Visibility = Visibility.Visible;
                            ChargesNOrdersTabMenu.Visibility = Visibility.Hidden;
                            ItemsTabMenu.Visibility = Visibility.Hidden;

                            allDocBUT.IsChecked = true;
                            ReportsTable.Style = VisibleStyle;
                            OrderCreationScreen.Style = HiddenStyle;
                            ChargingScreen.Style = HiddenStyle;

                            DocumentsTable.Style = VisibleStyle;
                            DocActionRefund.Visibility = Visibility.Hidden;
                            DocActionReports.Visibility = Visibility.Visible;
                            ItemsTable.Style = HiddenStyle;
                            CustomersTable.Style = HiddenStyle;
                            break;
                        }
                    case "itemsBUT":
                        {
                            CleanAllForms();
                            HeadTitle.Text = "Items & Customers";
                            Subtitle.Text = "Items List";
                            ItemsTabMenu.Visibility = Visibility.Visible;
                            ReportsTabMenu.Visibility = Visibility.Hidden;
                            ChargesNOrdersTabMenu.Visibility = Visibility.Hidden;

                            allItemsBUT.IsChecked = true;
                            ReportsTable.Style = VisibleStyle;
                            OrderCreationScreen.Style = HiddenStyle;
                            ChargingScreen.Style = HiddenStyle;

                            DocumentsTable.Style = HiddenStyle;
                            DocActionRefund.Visibility = Visibility.Visible;
                            DocActionReports.Visibility = Visibility.Hidden;
                            ItemsTable.Style = VisibleStyle;
                            CustomersTable.Style = HiddenStyle;
                            break;
                        }
                }
            }
        }

        private void ButtonClick_PayMethods(object sender, RoutedEventArgs e)
        {
            RadioButton clickedButton = sender as RadioButton;
            Style HiddenStyle = (Style)FindResource("HiddenStyle");
            Style VisibleStyle = (Style)FindResource("VisibleStyle");

            if (clickedButton != null)
            {
                switch (clickedButton.Name)
                {
                    case "cashBUT":
                        {
                            if (CashForm == null) { break; }

                            CashForm.Style = VisibleStyle;
                            CreditCaForm.Style = HiddenStyle;
                            ChequeForm.Style = HiddenStyle;
                            BankForm.Style = HiddenStyle;
                            BitForm.Style = HiddenStyle;
                            break;
                        }
                    case "CredBUT":
                        {
                            CreditCaForm.Style = VisibleStyle;
                            CashForm.Style = HiddenStyle;
                            ChequeForm.Style = HiddenStyle;
                            BankForm.Style = HiddenStyle;
                            BitForm.Style = HiddenStyle;
                            break;
                        }
                    case "chequeBUT":
                        {
                            ChequeForm.Style = VisibleStyle;
                            CashForm.Style = HiddenStyle;
                            CreditCaForm.Style = HiddenStyle;
                            BankForm.Style = HiddenStyle;
                            BitForm.Style = HiddenStyle;
                            break;
                        }
                    case "bankBUT":
                        {
                            BankForm.Style = VisibleStyle;
                            CashForm.Style = HiddenStyle;
                            CreditCaForm.Style = HiddenStyle;
                            ChequeForm.Style = HiddenStyle;
                            BitForm.Style = HiddenStyle;
                            break;
                        }
                    case "bitBUT":
                        {
                            BitForm.Style = VisibleStyle;
                            CashForm.Style = HiddenStyle;
                            CreditCaForm.Style = HiddenStyle;
                            ChequeForm.Style = HiddenStyle;
                            BankForm.Style = HiddenStyle;
                            break;
                        }
                }
            }
        }



        private void Button_Click_TabNav(object sender, RoutedEventArgs e)
        {
            RadioButton clickedButton = sender as RadioButton;
            Style HiddenStyle = (Style)FindResource("HiddenStyle");
            Style VisibleStyle = (Style)FindResource("VisibleStyle");

            if (clickedButton != null)
            {
                switch (clickedButton.Name)
                {
                    case "chargeBUT":
                        {
                            if (CashForm == null) { break; }

                            HeadTitle.Text = "Charge Or Refund Manualy";
                            Subtitle.Text = "";
                            ChargingScreen.Style = VisibleStyle;
                            ReportsTable.Style = HiddenStyle;
                            OrderCreationScreen.Style = HiddenStyle;

                            DocumentsTable.Style = HiddenStyle;
                            DocActionRefund.Visibility = Visibility.Visible;
                            DocActionReports.Visibility = Visibility.Hidden;
                            ItemsTable.Style = HiddenStyle;
                            CustomersTable.Style = HiddenStyle;
                            break;
                        }
                    case "refundBUT":
                        {
                            HeadTitle.Text = "Refund By Deal";
                            Subtitle.Text = "Invoices Associated To A Deal";
                            ReportsTable.Style = VisibleStyle;
                            ChargingScreen.Style = HiddenStyle;
                            OrderCreationScreen.Style = HiddenStyle;

                            DocumentsTable.Style = VisibleStyle;
                            DocActionRefund.Visibility = Visibility.Visible;
                            DocActionReports.Visibility = Visibility.Hidden;
                            ItemsTable.Style = HiddenStyle;
                            CustomersTable.Style = HiddenStyle;
                            break;
                        }
                    case "orderBUT":
                        {
                            HeadTitle.Text = "Create New Order";
                            Subtitle.Text = "";
                            OrderCreationScreen.Style = VisibleStyle;
                            ReportsTable.Style = HiddenStyle;
                            ChargingScreen.Style = HiddenStyle;

                            DocumentsTable.Style = HiddenStyle;
                            DocActionRefund.Visibility = Visibility.Visible;
                            DocActionReports.Visibility = Visibility.Hidden;
                            ItemsTable.Style = HiddenStyle;
                            CustomersTable.Style = HiddenStyle;
                            break;
                        }
                    case "allDocBUT":
                        {
                            if (Subtitle == null) { break; }

                            HeadTitle.Text = "All Reports";
                            Subtitle.Text = "Browse All Documents";
                            ReportsTable.Style = VisibleStyle;
                            ChargingScreen.Style = HiddenStyle;
                            OrderCreationScreen.Style = HiddenStyle;

                            DocumentsTable.Style = VisibleStyle;
                            DocActionRefund.Visibility = Visibility.Hidden;
                            DocActionReports.Visibility = Visibility.Visible;
                            ItemsTable.Style = HiddenStyle;
                            CustomersTable.Style = HiddenStyle;
                            break;
                        }
                    case "byCusBUT":
                        {
                            HeadTitle.Text = "All Reports";
                            Subtitle.Text = "Documents By Customer";
                            ReportsTable.Style = VisibleStyle;
                            ChargingScreen.Style = HiddenStyle;
                            OrderCreationScreen.Style = HiddenStyle;

                            // מסנן לפי לקוח
                            DocumentsTable.Style = VisibleStyle;
                            DocActionRefund.Visibility = Visibility.Hidden;
                            DocActionReports.Visibility = Visibility.Visible;
                            ItemsTable.Style = HiddenStyle;
                            CustomersTable.Style = HiddenStyle;
                            break;
                        }
                    case "allItemsBUT":
                        {
                            if (Subtitle == null) { break; }


                            HeadTitle.Text = "Items & Customers";
                            Subtitle.Text = "Items List";
                            ReportsTable.Style = VisibleStyle;
                            ChargingScreen.Style = HiddenStyle;
                            OrderCreationScreen.Style = HiddenStyle;

                            DocumentsTable.Style = HiddenStyle;
                            DocActionRefund.Visibility = Visibility.Visible;
                            DocActionReports.Visibility = Visibility.Hidden;
                            ItemsTable.Style = VisibleStyle;
                            CustomersTable.Style = HiddenStyle;
                            break;
                        }
                    case "allCusBUT":
                        {
                            HeadTitle.Text = "Items & Customers";
                            Subtitle.Text = "Customers List";
                            ReportsTable.Style = VisibleStyle;
                            ChargingScreen.Style = HiddenStyle;
                            OrderCreationScreen.Style = HiddenStyle;

                            DocumentsTable.Style = HiddenStyle;
                            DocActionRefund.Visibility = Visibility.Visible;
                            DocActionReports.Visibility = Visibility.Hidden;
                            ItemsTable.Style = HiddenStyle;
                            CustomersTable.Style = VisibleStyle;
                            break;
                        }
                }
            }
        }


        private void RadioButton_ExitApp(object sender, RoutedEventArgs e)
        {
            // navigate out to home
        }

        private void Button_ClickNewProductLine(object sender, RoutedEventArgs e)
        {
            Grid lines = chargeBUT.IsChecked == true ? InvoiceItemLines : OrderItemLines;
            string kind = chargeBUT.IsChecked == true ? "I" : "O";

            // Increment the row count
            int newRow = lines.RowDefinitions.Count;

            // Add a new RowDefinition to the parent Grid
            lines.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Create a new Grid for the product line
            Grid newProductLine = new Grid { Margin = new Thickness(0, 0, 0, 5) };
            newProductLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            newProductLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
            newProductLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(15, GridUnitType.Star) });
            newProductLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
            newProductLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });
            newProductLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });

            // Set the row where this new Grid will be placed
            Grid.SetRow(newProductLine, newRow);

            // Add the Button
            Button deleteButton = new Button { HorizontalAlignment = HorizontalAlignment.Left, Style = (Style)FindResource("TopButton") };
            deleteButton.Content = new Image { Source = new BitmapImage(new Uri("/Assets/DeleteIcon.png", UriKind.Relative)), Width = 15, Height = 15 };
            deleteButton.Click += (sender, e) => DeleteProductLine(newProductLine); // Assign the event handler
            //Grid.SetColumn(deleteButton, 0);
            newProductLine.Children.Add(deleteButton);

            // Add TextBoxes for Product ID, Description, Price, Quantity, and Total Price
            TextBox productIDTextBox = new TextBox { Name = $"{kind}prodID", Margin = new Thickness(5, 0, 5, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            Grid.SetColumn(productIDTextBox, 1);
            newProductLine.Children.Add(productIDTextBox);

            TextBox descriptionTextBox = new TextBox { Name = $"{kind}prodDescription", Margin = new Thickness(5, 0, 5, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            Grid.SetColumn(descriptionTextBox, 2);
            newProductLine.Children.Add(descriptionTextBox);

            TextBox priceTextBox = new TextBox { Name = $"{kind}prodPrice", Margin = new Thickness(5, 0, 5, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            priceTextBox.TextChanged += InvoiceLine_TextChanged;
            Grid.SetColumn(priceTextBox, 3);
            newProductLine.Children.Add(priceTextBox);

            TextBox quantityTextBox = new TextBox { Name = $"{kind}prodQuantity", Text = "1", Margin = new Thickness(5, 0, 5, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            quantityTextBox.TextChanged += InvoiceLine_TextChanged;
            Grid.SetColumn(quantityTextBox, 4);
            newProductLine.Children.Add(quantityTextBox);

            TextBox totalPriceTextBox = new TextBox { Name = $"{kind}prodTotalPrice", Text = "0.00", Margin = new Thickness(5, 0, 5, 0), IsReadOnly = true, Style = (Style)FindResource("TextBoxFilterBar") };
            Grid.SetColumn(totalPriceTextBox, 5);
            newProductLine.Children.Add(totalPriceTextBox);

            // Add the new Grid to the ItemsLines Grid
            lines.Children.Add(newProductLine);
        }

        private void DeleteProductLine(Grid productLine)
        {
            Grid lines = chargeBUT.IsChecked == true ? InvoiceItemLines : OrderItemLines;

            // Get the row index of the grid to be removed
            int rowIndex = Grid.GetRow(productLine);

            // Remove the grid from the ItemsLines grid
            lines.Children.Remove(productLine);

            // Adjust the rows of the grids below the deleted one
            for (int i = rowIndex + 1; i < lines.RowDefinitions.Count; i++)
            {
                foreach (UIElement child in lines.Children)
                {
                    if (Grid.GetRow(child) == i)
                    {
                        Grid.SetRow(child, i - 1); // Move the row up by one
                    }
                }
            }

            // Remove the corresponding RowDefinition
            lines.RowDefinitions.RemoveAt(rowIndex);
            UpdateInvoiceTotalPrice();
        }

        private void Button_ClickNewChequeLine(object sender, RoutedEventArgs e)
        {
            Grid lines = ChequeLines;  // Assuming the Grid is named "ChequeForm"

            // Increment the row count
            int newRow = lines.RowDefinitions.Count;

            // Add a new RowDefinition to the parent Grid
            lines.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Create a new Grid for the cheque line
            Grid newChequeLine = new Grid { Margin = new Thickness(0, 0, 0, 5) };
            newChequeLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            newChequeLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
            newChequeLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
            newChequeLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
            newChequeLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
            newChequeLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
            newChequeLine.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });

            // Set the row where this new Grid will be placed
            Grid.SetRow(newChequeLine, newRow);

            // Add the Delete Button
            Button deleteButton = new Button { HorizontalAlignment = HorizontalAlignment.Left, Style = (Style)FindResource("TopButton") };
            deleteButton.Content = new Image { Source = new BitmapImage(new Uri("/Assets/DeleteIcon.png", UriKind.Relative)), Width = 15, Height = 15 };
            deleteButton.Click += (sender, e) => DeleteChequeLine(newChequeLine); // Assign the event handler
            newChequeLine.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 0);

            // Add TextBoxes for Account Number, Branch Number, Bank Number, Cheque Number, Date, Cheque Amount
            TextBox accountNumberTextBox = new TextBox { Name = "AccountNumber", Margin = new Thickness(5, 0, 10, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            Grid.SetColumn(accountNumberTextBox, 1);
            newChequeLine.Children.Add(accountNumberTextBox);

            TextBox branchNumberTextBox = new TextBox { Name = "BranchNumber", Margin = new Thickness(5, 0, 10, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            Grid.SetColumn(branchNumberTextBox, 2);
            newChequeLine.Children.Add(branchNumberTextBox);

            TextBox bankNumberTextBox = new TextBox { Name = "BankNumber", Margin = new Thickness(5, 0, 10, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            Grid.SetColumn(bankNumberTextBox, 3);
            newChequeLine.Children.Add(bankNumberTextBox);

            TextBox chequeNumberTextBox = new TextBox { Name = "ChequeNumber", Margin = new Thickness(5, 0, 10, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            Grid.SetColumn(chequeNumberTextBox, 4);
            newChequeLine.Children.Add(chequeNumberTextBox);

            TextBox dateTextBox = new TextBox { Name = "ChequeDate", Margin = new Thickness(5, 0, 10, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            Grid.SetColumn(dateTextBox, 5);
            newChequeLine.Children.Add(dateTextBox);

            TextBox chequeAmountTextBox = new TextBox { Name = "ChequeAmount", Margin = new Thickness(5, 0, 10, 0), Style = (Style)FindResource("TextBoxFilterBar") };
            chequeAmountTextBox.TextChanged += PaymentSumUpdated;
            Grid.SetColumn(chequeAmountTextBox, 6);
            newChequeLine.Children.Add(chequeAmountTextBox);

            // Add the new Grid to the ChequeForm Grid
            lines.Children.Add(newChequeLine);
        }

        private void DeleteChequeLine(Grid ChequeLine)
        {
            Grid lines = ChequeLines; // Assuming the Grid is named "ChequeForm"

            // Get the row index of the grid to be removed
            int rowIndex = Grid.GetRow(ChequeLine);

            // Remove the grid from the ChequeForm grid
            lines.Children.Remove(ChequeLine);

            // Adjust the rows of the grids below the deleted one
            for (int i = rowIndex + 1; i < lines.RowDefinitions.Count; i++)
            {
                foreach (UIElement child in lines.Children)
                {
                    if (Grid.GetRow(child) == i)
                    {
                        Grid.SetRow(child, i - 1); // Move the row up by one
                    }
                }
            }

            // Remove the corresponding RowDefinition
            lines.RowDefinitions.RemoveAt(rowIndex);
            UpdateInvoiceBalance();
        }


        private void InvoiceLine_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null) return;

            Grid parentGrid = textBox.Parent as Grid;

            if (parentGrid == null) return;

            TextBox? priceTextBox = parentGrid.Children
                .OfType<TextBox>()
                .FirstOrDefault(tb => tb.Name.Contains("prodPrice"));

            TextBox? quantityTextBox = parentGrid.Children
                .OfType<TextBox>()
                .FirstOrDefault(tb => tb.Name.Contains("prodQuantity"));

            TextBox? totalPriceTextBox = parentGrid.Children
                .OfType<TextBox>()
                .FirstOrDefault(tb => tb.Name.Contains("prodTotalPrice"));

            if (priceTextBox == null || quantityTextBox == null || totalPriceTextBox == null) return;

            decimal price = 0;
            int quantity = 1;

            if (decimal.TryParse(priceTextBox.Text, out decimal parsedPrice))
            {
                price = parsedPrice;
            }
            if (int.TryParse(quantityTextBox.Text, out int parsedQuantity))
            {
                quantity = parsedQuantity;
            }

            decimal totalPrice = price * quantity;
            totalPriceTextBox.Text = totalPrice.ToString();
            UpdateInvoiceTotalPrice();
        }

        private void UpdateInvoiceTotalPrice()
        {
            Grid lines = chargeBUT.IsChecked == true ? InvoiceItemLines : OrderItemLines;
            decimal totalPrice = 0;

            foreach (Grid g in lines.Children)
            {
                if (g.Children[5] is TextBox totalPriceTextBox)
                {
                    if (decimal.TryParse(totalPriceTextBox.Text, out decimal lineTotal))
                    {
                        totalPrice += lineTotal;
                    }
                }
            }
            if (lines == InvoiceItemLines)
            {
                InvoiceTotalSum.Text = totalPrice.ToString();
                DealTotalSum.Text = $"Total Price: {totalPrice.ToString()}";
                UpdateInvoiceBalance();
            }
            else
            {
                OrderTotalSum.Text = totalPrice.ToString();
                if (totalPrice > 0)
                {
                    CreateOrderButton.IsEnabled = true;
                }
                if (totalPrice == 0)
                {
                    CreateOrderButton.IsEnabled = false;
                }

            }
        }

        private void UpdateInvoiceBalance()
        {
            decimal totalPriceToPay = decimal.Parse(InvoiceTotalSum.Text);
            decimal totalPayed = 0;

            decimal.TryParse(TotalCash?.Text ?? "0", out decimal cashValue);
            decimal.TryParse(CardSumToBill?.Text ?? "0", out decimal cardValue);
            decimal.TryParse(TotalBankSum?.Text ?? "0", out decimal bankValue);
            decimal.TryParse(TotalBitSum?.Text ?? "0", out decimal bitValue);

            totalPayed = cashValue + cardValue + bankValue + bitValue;

            if (ChequeLines != null)
            {
                foreach (Grid g in ChequeLines.Children)
                {
                    if (g.Children[6] is TextBox ChequeAmount)
                    {
                        if (decimal.TryParse(ChequeAmount.Text, out decimal lineTotal))
                        {
                            totalPayed += lineTotal;
                        }
                    }
                }
            }
            if (BalanceDueSum != null)
            {
                BalanceDueSum.Text = $"Balance Due: {totalPayed - totalPriceToPay}";

            }
            if ((totalPayed - totalPriceToPay) == 0 && totalPriceToPay > 0)
            {
                CreateInvoiceButton.IsEnabled = true;
            }
            else
            {
                CreateInvoiceButton.IsEnabled = false;
            }
        }

        private void PaymentSumUpdated(object sender, TextChangedEventArgs e)
        {
            UpdateInvoiceBalance();
        }
        public void InitilizeDataForInvoice()
        {
            InitilizeDataForCustomers();
            InitilizeDataForItems();

        }
        public void InitilizeDataForCustomers()
        {
            FM.GetAllCustomers();
        }

        public void InitilizeDataForItems()
        {
            FM.GetAllItems();
        }

        // reports **************************************************
        // get the data related to other files in the reports (customerName => invoice report) 
        // validate buttons according to invoice type & deal refunded 
        // manage items in table - add functions  
        public void CopyCustomerToInvoice()
        {

        }

        public void CopyInvoiceToRefund()
        {

        }

        public void CopyOrderToInvoice()
        {

        }
        public void CancelInvoice()
        {

        }

        public void EditCustomerTable()
        {

        }

        // make some tooltip - or pop-up to show more details of the invoice 
        public void ShowInvoiceDetails()
        {
            //AM.GetInvoiceDetails();
        }


        // remember to also bind to the invoice 
        public void SendToEmail(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            // Extract the bound Invoice object from the Tag
            if (clickedButton != null && clickedButton.Tag is Invoice invoice)
            {
                // Access DocumentNumber and DocumentType
                int invoiceNumber = invoice.InvoiceID;
                int invoiceType = invoice.InvoiceType;
                Customer? thisCus = FM.GetCustomerById(invoice.CustomerID);
                if (thisCus != null)
                {
                    string email = thisCus.Email;
                    string response = AM.SendInvoiceToEmail(invoiceNumber, invoiceType, email);
                    MessageBox.Show(response);
                }
            }

        }

        // reset in all forms - and reload the data for all reports 
        public void ResetFormFields()
        {

        }
        private bool ValidateInvoice(int invoiceType, char invoiceKind)
        {
            // Validate Customer Information
            string customerName = ((TextBox)this.FindName($"{invoiceKind}customerName")).Text;
            string customerPhone = ((TextBox)this.FindName($"{invoiceKind}customerPhone")).Text;
            string customerEmail = ((TextBox)this.FindName($"{invoiceKind}customerEmail")).Text;

            if (string.IsNullOrWhiteSpace(customerName))
            {
                MessageBox.Show("Customer name is required.");
                return false;
            }

            if (!IsValidPhone(customerPhone))
            {
                MessageBox.Show("Phone number is invalid.");
                return false;
            }

            if (!IsValidEmail(customerEmail))
            {
                MessageBox.Show("Email is invalid.");
                return false;
            }

            // Validate Item Rows
            Grid itemsGrid = chargeBUT.IsChecked == true ? InvoiceItemLines : OrderItemLines;
            foreach (UIElement element in itemsGrid.Children)
            {
                if (element is Grid g)
                {
                    string itemName = ((TextBox)g.FindName($"{invoiceKind}prodDescription")).Text;
                    string itemPrice = ((TextBox)g.FindName($"{invoiceKind}prodPrice")).Text;
                    string itemQuantity = ((TextBox)g.FindName($"{invoiceKind}prodQuantity")).Text;

                    if (string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(itemPrice) || string.IsNullOrWhiteSpace(itemQuantity))
                    {
                        MessageBox.Show("All item fields must be filled.");
                        return false;
                    }

                    // Ensure price and quantity are valid numbers
                    if (!decimal.TryParse(itemPrice, out _) || !int.TryParse(itemQuantity, out _))
                    {
                        MessageBox.Show("Item price and quantity must be valid numbers.");
                        return false;
                    }
                }
            }

            if (invoiceType != 0)
            {
                // Validate Cheque Rows
                Grid chequeGrid = chargeBUT.IsChecked == true ? ChequeLines : ChequeLines;
                foreach (UIElement element in chequeGrid.Children)
                {
                    if (element is Grid g)
                    {
                        TextBox chequeNumberBox = g.Children[4] as TextBox;
                        TextBox bankNumberBox = g.Children[3] as TextBox;
                        TextBox snifNumberBox = g.Children[2] as TextBox;
                        TextBox accountNumberBox = g.Children[1] as TextBox;
                        TextBox chequeDateBox = g.Children[5] as TextBox;
                        TextBox chequeSumBox = g.Children[6] as TextBox;

                        if (!string.IsNullOrWhiteSpace(chequeSumBox.Text))
                        {
                            if (string.IsNullOrWhiteSpace(chequeNumberBox.Text) || string.IsNullOrWhiteSpace(bankNumberBox.Text) ||
                                string.IsNullOrWhiteSpace(snifNumberBox.Text) || string.IsNullOrWhiteSpace(accountNumberBox.Text) ||
                                string.IsNullOrWhiteSpace(chequeDateBox.Text))
                            {
                                MessageBox.Show("All cheque fields must be filled.");
                                return false;
                            }

                            // Validate cheque sum and date
                            if (!decimal.TryParse(chequeSumBox.Text, out _))
                            {
                                MessageBox.Show("Cheque sum must be a valid number.");
                                return false;
                            }

                            if (!DateTime.TryParse(chequeDateBox.Text, out _))
                            {
                                MessageBox.Show("Cheque date must be in a valid format.");
                                return false;
                            }
                        }
                    }
                }

                // Validate Payment Methods
                if (chargeBUT.IsChecked == true)
                {
                    string cardSum = CardSumToBill.Text;
                    string totalCash = TotalCash.Text;
                    string totalBitSum = TotalBitSum.Text;
                    string totalBankSum = TotalBankSum.Text;

                    // Ensure at least one payment method is used
                    bool hasPaymentMethod = false;

                    // Validate Credit Card
                    if (!string.IsNullOrWhiteSpace(cardSum))
                    {
                        hasPaymentMethod = true;

                        if (string.IsNullOrWhiteSpace(CardNumber.Text) || string.IsNullOrWhiteSpace(Cvv.Text) ||
                            string.IsNullOrWhiteSpace(ExtDateMonth.Text) || string.IsNullOrWhiteSpace(ExtDateYear.Text) ||
                            string.IsNullOrWhiteSpace(CardOwnerId.Text) || string.IsNullOrWhiteSpace(CardSumToBill.Text))
                        {
                            MessageBox.Show("All credit card fields must be filled when using a credit card.");
                            return false;
                        }
                    }

                    // Validate Custom Payments (Bit/Bank)
                    if (!string.IsNullOrWhiteSpace(totalBitSum) || !string.IsNullOrWhiteSpace(totalBankSum))
                    {
                        hasPaymentMethod = true;

                        if (!string.IsNullOrWhiteSpace(totalBitSum) && string.IsNullOrWhiteSpace(AsmachtaBit.Text))
                        {
                            MessageBox.Show("Asmachta for Bit payment must be filled when using Bit payment.");
                            return false;
                        }

                        if (!string.IsNullOrWhiteSpace(totalBankSum) && string.IsNullOrWhiteSpace(AsmachtaBank.Text))
                        {
                            MessageBox.Show("Asmachta for Bank payment must be filled when using Bank payment.");
                            return false;
                        }
                    }

                    // Validate Cash Payment
                    if (!string.IsNullOrWhiteSpace(totalCash))
                    {
                        hasPaymentMethod = true;

                        if (!decimal.TryParse(totalCash, out _))
                        {
                            MessageBox.Show("Total cash must be a valid number.");
                            return false;
                        }
                    }

                    if (!hasPaymentMethod)
                    {
                        MessageBox.Show("At least one payment method must be provided.");
                        return false;
                    }
                }
            }

            // All validations passed
            return true;
        }

        // Validation Helper Methods
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Simple phone validation; could be extended using Regex
            return !string.IsNullOrWhiteSpace(phone);
        }


        private async void SendInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            await SendInvoiceButton();
        }

        private async void Button_ClickDownloadInvoice(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            // Extract the bound Invoice object from the Tag
            if (clickedButton != null && clickedButton.Tag is Invoice invoice)
            {
                // Access DocumentNumber and DocumentType
                int invoiceNumber = invoice.InvoiceID;
                int invoiceType = invoice.InvoiceType;

                // Call the download method and get the PDF as a byte array
                byte[] pdfData = AM.DownloadDocumentPDF(invoiceNumber, invoiceType, true);

                if (pdfData != null)
                {
                    // Save the PDF file as a binary file
                    string filePath = $"{invoiceNumber.ToString()}_{DateTime.Now.Hour}.pdf";
                    File.WriteAllBytes(filePath, pdfData);

                    MessageBox.Show("File downloaded successfully to " + filePath);
                }
                else
                {
                    MessageBox.Show("Failed to download the document.");
                }
            }
        }
        //private async void Button_ClickShowInvoice(object sender, RoutedEventArgs e)
        //{
        //    Button clickedButton = sender as Button;

        //    // Extract the bound Invoice object from the Tag
        //    if (clickedButton != null && clickedButton.Tag is Invoice invoice)
        //    {
        //        // Access InvoiceNumber and InvoiceType
        //        int invoiceNumber = invoice.InvoiceID;
        //        int invoiceType = invoice.InvoiceType;

        //        // Call the download method to get the HTML content as a string
        //        string htmlContent = AM.DownloadDocumentHTML(invoiceNumber, invoiceType, true);

        //        if (!string.IsNullOrEmpty(htmlContent))
        //        {
        //            // Load the HTML content into the WebBrowser control
        //            InvoiceWebBrowser.Visibility = Visibility.Visible;
        //            Test1.Visibility = Visibility.Hidden;
        //            InvoiceWebBrowser.NavigateToString(htmlContent);

        //        }
        //        else
        //        {
        //            MessageBox.Show("Failed to retrieve the HTML content.");
        //        }

        //    }
        //    MessageBox.Show("success");
        //}
        public void CleanAllForms()
        {
            IcustomerName.Text = "";
            IcustomerPhone.Text = "";
            IcustomerEmail.Text = "";
            IprodID.Text = "";
            IprodDescription.Text = "";
            IprodPrice.Text = "";
            IprodQuantity.Text = "1";
            IprodTotalPrice.Text = "0.00";
            InvoiceTotalSum.Text = "0.00";
            TotalCash.Text = "";
            CardNumber.Text = "";
            ExtDateYear.SelectedIndex = 0;
            ExtDateMonth.SelectedIndex = 0;
            Cvv.Text = "";
            CardOwnerId.Text = "";
            CardOwnerName.Text = "";
            Payments.SelectedIndex = 0;
            CardSumToBill.Text = "";
            AccountNumber.Text = "";
            BranchNumber.Text = "";
            BankNumber.Text = "";
            ChequeNumber.Text = "";
            ChequeDate.Text = "";
            ChequeAmount.Text = "";
            AsmachtaBank.Text = "";
            TotalBankSum.Text = "";
            AsmachtaBit.Text = "";
            TotalBitSum.Text = "";
            OcustomerName.Text = "";
            OcustomerPhone.Text = "";
            OcustomerEmail.Text = "";
            OprodID.Text = "";
            OprodDescription.Text = "";
            OprodPrice.Text = "";
            OprodQuantity.Text = "";
            OprodTotalPrice.Text = "";
            OrderTotalSum.Text = "";

            while (InvoiceItemLines.Children.Count > 1)
            {
                InvoiceItemLines.Children.RemoveAt(1);
            }
            while (OrderItemLines.Children.Count > 1)
            {
                OrderItemLines.Children.RemoveAt(1);
            }
            while (ChequeLines.Children.Count > 1)
            {
                ChequeLines.Children.RemoveAt(1);
            }
        }

        private void Button_ClickGetUser(object sender, RoutedEventArgs e)
        {
            PopUpWindow.IsOpen = true;
            AddItemPop.Visibility = Visibility.Hidden;
            EditItemPop.Visibility = Visibility.Hidden;
            GetItemPop.Visibility = Visibility.Hidden;
            GetCustomerPop.Visibility = Visibility.Visible;
        }

        private void Button_ClickGetItem(object sender, RoutedEventArgs e)
        {
            PopUpWindow.IsOpen = true;
            AddItemPop.Visibility = Visibility.Hidden;
            EditItemPop.Visibility = Visibility.Hidden;
            GetItemPop.Visibility = Visibility.Visible;
            GetCustomerPop.Visibility = Visibility.Hidden;
        }
        private void Button_ClickCreateNewItem(object sender, RoutedEventArgs e)
        {
            PopUpWindow.IsOpen = true;
            AddItemPop.Visibility = Visibility.Visible;
            EditItemPop.Visibility = Visibility.Hidden;
            GetItemPop.Visibility = Visibility.Hidden;
            GetCustomerPop.Visibility = Visibility.Hidden;
        }
        private void Button_ClickEditItem(object sender, RoutedEventArgs e)
        {
            PopUpWindow.IsOpen = true;
            AddItemPop.Visibility = Visibility.Hidden;
            EditItemPop.Visibility = Visibility.Visible;
            GetItemPop.Visibility = Visibility.Hidden;
            GetCustomerPop.Visibility = Visibility.Hidden;
        }


    }

}

