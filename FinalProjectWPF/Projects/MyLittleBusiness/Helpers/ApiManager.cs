using FinalProjectWPF.Projects.MyLittleBusiness.Models;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Windows;

namespace FinalProjectWPF.Projects.MyLittleBusiness.Helpers
{
    internal class ApiManager
    {
        public static HttpClient client = new HttpClient();
        private readonly string baseUrl = "https://secure.cardcom.solutions/Interface";
        private readonly string? terminalNumber = Environment.GetEnvironmentVariable("TerminalNumber", EnvironmentVariableTarget.User);
        private readonly string? username = Environment.GetEnvironmentVariable("UserName", EnvironmentVariableTarget.User);
        private readonly string? password = Environment.GetEnvironmentVariable("UserPassword", EnvironmentVariableTarget.User);

        public ApiManager()
        {
            if (!CheckUserInformationExist())
            {
                MessageBox.Show("Please enter API secrets in Environment variables");
            }

        }
        // Method to cancel a deal
        public async Task<string> CancelDeal(int internalDealNumber, decimal? partialSum = null)
        {
            string url;
            if (partialSum != null)
            {
                url = $"{baseUrl}/CancelDeal.aspx?TerminalNumber={terminalNumber}&name={username}&pass={password}&InternalDealNumber={internalDealNumber}&PartialSum={partialSum}";
            }
            else
            {
                url = $"{baseUrl}/CancelDeal.aspx?TerminalNumber={terminalNumber}&name={username}&pass={password}&InternalDealNumber={internalDealNumber}";
            }

            var response = client.PostAsync(url, null).Result; // Synchronously wait for the result
            return response.Content.ReadAsStringAsync().Result; // Read response content
        }

        // Method to download a PDF of the document
        public byte[] DownloadDocumentPDF(int documentNumber, int documentType, bool isOriginal)
        {
            string url = $"{baseUrl}/GetDocumentPDF.aspx?UserName={username}&UserPassword={password}&DocumentNumber={documentNumber}&DocumentType={documentType}&IsOriginal={isOriginal}";
            var response = client.GetAsync(url).Result; // Synchronously wait for the result
            if (response.IsSuccessStatusCode)
            {
                // Read the content as a byte array
                return response.Content.ReadAsByteArrayAsync().Result;
            }

            return null;
        }

        // Method to download HTML of the invoice
        public string DownloadDocumentHTML(int invoiceNumber, int invoiceType, bool getAsOriginal)
        {
            string url = $"{baseUrl}/InvoiceGetHtml.aspx?UserName={username}&UserPassword={password}&InvoiceNumber={invoiceNumber}&InvoiceType={invoiceType}&GetAsOriginal={getAsOriginal}";
            var response = client.GetAsync(url).Result; // Synchronously wait for the result
            return response.Content.ReadAsStringAsync().Result; // Read response content
        }

        // Method to get invoice information
        public string GetInvoiceInfo(int invoiceNumber, int invoiceType)
        {
            string url = $"{baseUrl}/InvoiceGetInfo.aspx?username={username}&userpassword={password}&invoiceNumber={invoiceNumber}&invoiceType={invoiceType}";
            var response = client.GetAsync(url).Result; // Synchronously wait for the result
            return response.Content.ReadAsStringAsync().Result; // Read response content
        }

        // Method to send an invoice copy to an email
        public string SendInvoiceToEmail(int invoiceNumber, int invoiceType, string emailAddress)
        {
            string url = $"{baseUrl}/SendInvoiceCopy.aspx?username={username}&userpassword={password}&InvoiceNumber={invoiceNumber}&invoiceType={invoiceType}&EmailAddress={emailAddress}";
            var response = client.GetAsync(url).Result; // Synchronously wait for the result
            return response.Content.ReadAsStringAsync().Result; // Read response content
        }

        // Method to get customer details
        public string GetCustomerDetails(int accountId)
        {
            string url = $"{baseUrl}/GetAccount.aspx?TerminalNumber={terminalNumber}&UserName={username}&AccountID={accountId}";
            var response = client.GetAsync(url).Result; // Synchronously wait for the result
            return response.Content.ReadAsStringAsync().Result; // Read response content
        }

        public bool CheckUserInformationExist()
        {
            return (username != null && password != null && terminalNumber != null);
        }


        public async Task<String> CreateNewInvoice(int invoiceType, string customerName, string email, string userMobile, bool sendByEmail, string language, List<Item> items, decimal? cashPay = null, int? cardValidityMonth = null, int? cardValidityYear = null, long? identityNum = null, long? cardNumber = null, int? securityCVV = null, int? paymentsAmount = null, decimal? cardSum = null, List<CustomPay>? customPays = null, List<Cheque>? cheques = null)
        {
            string urlTemp = "";
            string? cancelDealNumber = "";
            if (invoiceType == 1 && cardNumber == null)
            {
                urlTemp = $"{baseUrl}/CreateInvoice.aspx?";
            }
            else if (invoiceType == 1 && cardNumber != null)
            {
                urlTemp = $"{baseUrl}/Direct2.aspx?";
            }
            else if (invoiceType == 2 && cardNumber == null)
            {
                urlTemp = $"{baseUrl}/CreateInvoice.aspx?";
            }
            else if (invoiceType == 2 && cardNumber != null)
            {
                string cardExipration = cardValidityMonth.ToString() + "" + cardValidityYear.ToString().Substring(2);

                cancelDealNumber = await RefundCreditCard(cardNumber, cardExipration, securityCVV, paymentsAmount, cardSum, customerName);
                MessageBox.Show(cancelDealNumber);
                urlTemp = $"{baseUrl}/CreateInvoice.aspx?";
            }
            else
            {
                urlTemp = $"{baseUrl}/CreateInvoice.aspx?";
            }

            // user password 
            string url = $"{urlTemp}TerminalNumber={terminalNumber}&UserName={username}&InvoiceHead.CustMobilePH={userMobile}&InvoiceHead.Email={email}&InvoiceHead.IsAutoCreateUpdateAccount=true&InvoiceType={invoiceType}&InvoiceHead.CustName={customerName}&InvoiceHead.SendByEmail={sendByEmail}&InvoiceHead.Language={language}";
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                string prefix = i == 0 ? "InvoiceLines" : $"InvoiceLines{i}";
                url += $"&{prefix}.Description={item.Name}&{prefix}.Price={item.Price}&{prefix}.Quantity={item.Quantity}";
            }
            if (invoiceType == 1 || invoiceType == 2)
            {
                if (cashPay != null) { url += $"&cash={cashPay}"; }
                if (cardNumber != null && invoiceType == 1)
                {
                    url += $"&cardnumber={cardNumber}&Cvv={securityCVV}&cardvalidityyear={cardValidityYear}&cardvaliditymonth={cardValidityMonth}&identitynumber={identityNum}&Sum={cardSum}&NumOfPayments={paymentsAmount}";
                }
                else if (cardNumber != null && invoiceType == 2)
                {
                    url += $"&CreditDealNum.DealNumber={cancelDealNumber}";
                }
                if (customPays != null)
                {
                    url += $"&CustomPay.TransactionID={customPays[0].TransactionID}&CustomPay.TranDate={customPays[0].TranDate}&CustomPay.Description={customPays[0].Description}&CustomPay.Asmacta={customPays[0].Asmacta}&CustomPay.Sum={customPays[0].Sum}";
                    if (customPays.Count == 2)
                    {
                        url += $"&CustomPay1.TransactionID={customPays[1].TransactionID}&CustomPay1.TranDate={customPays[1].TranDate}&CustomPay1.Description={customPays[1].Description}&CustomPay1.Asmacta={customPays[1].Asmacta}&CustomPay1.Sum={customPays[1].Sum}";
                    }
                }
                if (cheques != null)
                {
                    string listCheques = FormatChequesForRequest(cheques);
                    StringContent content = new StringContent(listCheques, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = client.PostAsync(url, content).Result; // Synchronously wait for the result
                    return response.Content.ReadAsStringAsync().Result; // Read response content
                }
            }

            var defaultResponse = client.PostAsync(url, null).Result;
            MessageBox.Show(defaultResponse.Content.ReadAsStringAsync().Result);
            return defaultResponse.Content.ReadAsStringAsync().Result; // Read response content
        }

        public string GetInvoiceDetails(int invoiceId, int invoiceType)
        {
            string url = $"{baseUrl}/CreateInvoice.aspx?UserPassword={password}&UserName={username}&invoiceNumber={invoiceId}&invoiceType={invoiceType}";
            var response = client.GetAsync(url).Result; // Synchronously wait for the result
            return response.Content.ReadAsStringAsync().Result; // Read response content
        }

        public async Task<string> RefundCreditCard(long? cardNum, string cardExpiration, int? cvv, int? paymentCount, decimal? amount, string fullName)
        {
            var requestBody = new
            {
                TerminalNumber = terminalNumber,
                ApiName = username,
                Amount = amount,
                CardNumber = cardNum,
                CardExpirationMMYY = cardExpiration,
                CVV2 = cvv,
                NumOfPayments = paymentCount,
                CardOwnerInformation = new
                {
                    FullName = fullName
                },
                ISOCoinId = 1,
                Advanced = new
                {
                    ApiPassword = password,
                    IsRefund = true
                }
            };

            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string url = "https://secure.cardcom.solutions/api/v11/Transactions/Transaction";
            var response = await client.PostAsync(url, content);

            // Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseData);
                if (responseObject.TryGetProperty("TranzactionId", out var tranzactionId))
                {
                    string dealNumber = tranzactionId.GetInt64().ToString();
                    MessageBox.Show($"Transaction ID: {dealNumber}");
                    return dealNumber;
                }
            }
            else
            {
                // Log error response
                string errorResponse = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Error Response: {errorResponse}");
            }

            return null;
        }


        public string FormatChequesForRequest(List<Cheque> cheques)
        {
            List<string> queryString = new List<string>();
            for (int i = 0; i < cheques.Count; i++)
            {
                Cheque cheque = cheques[i];
                string prefix = i == 0 ? "Cheque" : $"Cheque{i}";

                queryString.Add($"{prefix}.ChequeNumber={cheque.ChequeNumber}");
                queryString.Add($"{prefix}.DateCheque={cheque.Date.ToString("M/d/yyyy", CultureInfo.InvariantCulture)}");
                queryString.Add($"{prefix}.Sum={cheque.Sum}");
                queryString.Add($"{prefix}.SnifNumber={cheque.SnifNumber}");
                queryString.Add($"{prefix}.AccountNumber={cheque.AccountNumber}");
            }

            return string.Join("&", queryString);
        }

        // modify - need to move to mainwindow.cs 26/09
        // need to check if there is a deal number or not 
        public async Task<Dictionary<string, string?>> ParseResponseOfChargeInvoice(string response)
        {
            var parsedResponse = HttpUtility.ParseQueryString(response);

            // Initialize the dictionary to store extracted values
            Dictionary<string, string?> extractedValues = new Dictionary<string, string?>
            {
                { "InvoiceID", null },
                { "DealNumber", null },
                { "ResponseCode", null },
                { "CustomerID", null }
            };

            // Extract InvoiceID
            string? invoiceId = parsedResponse["InvoiceNumber"];
            if (string.IsNullOrEmpty(invoiceId))
            {
                invoiceId = parsedResponse["InvoiceResponse.InvoiceNumber"];
            }
            extractedValues["InvoiceID"] = invoiceId;

            // Extract DealNumber
            string? dealNumber = parsedResponse["InternalDealNumber"];
            if (string.IsNullOrEmpty(dealNumber))
            {
                dealNumber = parsedResponse["InvoiceResponse.InternalDealNumber"];
            }
            extractedValues["DealNumber"] = dealNumber;

            // Extract ResponseCode
            string? responseCode = parsedResponse["ResponseCode"];
            if (string.IsNullOrEmpty(responseCode))
            {
                responseCode = parsedResponse["InvoiceResponse.ResponseCode"];
            }
            extractedValues["ResponseCode"] = responseCode;

            // Extract CustomerID
            string? customerId = parsedResponse["AccountID"];
            if (string.IsNullOrEmpty(customerId))
            {
                customerId = parsedResponse["InvoiceResponse.AccountID"];
            }
            extractedValues["CustomerID"] = customerId;

            return extractedValues;
        }


    }
}
