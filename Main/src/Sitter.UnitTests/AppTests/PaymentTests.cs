using System;
using Braintree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySitterHub.Logic.Payment;

namespace MySitterHub.UnitTests.AppTests
{
    [TestClass]
    public class PaymentTests
    {
        private PaymentManager _paymentManager = new PaymentManager();
        private int _testParentId = 1;
        private int _testSitterId = 3;

        [TestMethod]
        public void TestSimplePayment()
        {
            var payment = new SimplePayment();
            payment.ParentId = _testParentId;
            //payment.Nonce = "fake-valid-nonce";
            payment.Amount = 1000.5M;
            //string token = _paymentManager.GetClientToken(_testParentId.ToString());

            MakePaymentResultSM ret = _paymentManager.MakeSimplePayment(payment);

            Assert.IsTrue(ret.IsSuccess);
        }

        [TestMethod]
        public void TestMarketplacePayment()
        {
            var payment = new MarketPayment();
            payment.ParentId = _testParentId;
            payment.Amount = 1000.5M;
            payment.MySitterHubFee = Convert.ToDecimal(1000.5*.1);
            payment.SitterId = _testSitterId;

            MakePaymentResultSM ret = _paymentManager.MakeMarketplaceTransaction(payment);

            Assert.IsTrue(ret.IsSuccess, "error: " + ret.Error);
        }
        
        [TestMethod]
        public void TestCreateMerchantAccount()
        {
            MerchantAccountRequest request = new MerchantAccountRequest
            {
                Individual = new IndividualRequest
                {
                    FirstName = "Joseph",
                    LastName = "Fluckiger",
                    Email = "joseph@fluckiger.org",
                    Phone = "5129219530",
                    DateOfBirth = "2005-04-25",
                    Ssn = "456-45-4567",
                    Address = new AddressRequest
                    {
                        StreetAddress = "9004 Sunburst Ter",
                        Locality = "Round Rock",
                        Region = "TX",
                        PostalCode = "78681"
                    }
                },
                Funding = new FundingRequest
                {
                    Destination = FundingDestination.MOBILE_PHONE,
                    MobilePhone = "5129219530"
                },
                TosAccepted = true,
                MasterMerchantAccountId = "mz5yt3cr5vpbjvq3",
                Id =  _testSitterId.ToString()  // "joseph_fluckiger_sitter2"
            };

            Result<MerchantAccount> ret = _paymentManager.CreateSubMerchantAccount(request);

            bool iss = ret.IsSuccess();

            Assert.IsTrue(iss);

           

        }

        [TestMethod]
        public void TestUpdateMerchant()
        {
            MerchantAccountRequest requestU = new MerchantAccountRequest
            {
                //Individual = new IndividualRequest
                //{
                //    FirstName = "Joseph22",
                //},
                Funding = new FundingRequest
                {
                    Descriptor = "Blue Ladders",
                    Destination = FundingDestination.BANK,
                    Email = "funding@blueladders.com",
                    MobilePhone = "5555555555",
                    AccountNumber = "1123581321",
                    RoutingNumber = "071101307"
                },
            };
            var retU = _paymentManager.UpdateSubMerchantAccount("joseph_fluckiger_sitter", requestU);
            var iss = retU.IsSuccess();

            MerchantAccount merchantAccount = retU.Target;
            var rn = merchantAccount.FundingDetails.RoutingNumber;
            
        }

        [TestMethod]
        public void TestCreatePaymentMethod()
        {
            var sm = new UpdatePaymentSM();

            //sm.Number = "4111111111111111"; // Visa    //https://developers.braintreepayments.com/ios+dotnet/guides/credit-cards
            sm.CVV = "1111"; // Visa
            sm.Number = "378282246310005"; //Amex
            sm.CVV = "1111"; // Amex

            sm.ExpirationYear = "2017";
            sm.ExpirationMonth = "07";
            sm.Nonce = "fake-valid-nonce";
            sm.Customer = new NewPaymentCustomerSM
            {
                CustomerId = _testParentId.ToString(),
                Phone = "5125551212",
                LastName = "Smith"
            };
            sm.CardholderName = sm.Customer.LastName;

            sm.Token = Guid.NewGuid().ToString();
            UpdatePaymentResultSM result = _paymentManager.CreatePaymentMethod(sm);

            Assert.IsTrue(result.IsSuccess);
        }


        [TestMethod]
        public void CreateCustomer()
        {
            var sm = new NewPaymentCustomerSM
            {
                CustomerId = _testParentId.ToString(),
                Phone = "512-921-9530",
                LastName = "Smith"
            };
            NewCustomerResultSM ret = _paymentManager.CreateCustomer(sm);
            Assert.IsTrue(ret.IsSuccess);

            Customer result = _paymentManager.FindCustomer(ret.NewBraintreeCustomerId);
            Assert.IsTrue(result != null);
        }
    }

    public class BrainTreePayment
    {
        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string EmailAddress { get; set; }

        public string MobileNumber { get; set; }

        public string CardholderName { get; set; }

        public string ExpirationMonth { get; set; }

        public string ExpirationYear { get; set; }

        public string CVV { get; set; }

        public string Number { get; set; }

        public string ClientID { get; set; }

        public decimal Amt { get; set; }
    }
}