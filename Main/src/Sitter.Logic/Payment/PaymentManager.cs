using System;
using System.Linq;
using Amazon.KeyManagementService.Model;
using Amazon.Runtime;
using Braintree;
using MySitterHub.Logic.Util;
using Environment = Braintree.Environment;

namespace MySitterHub.Logic.Payment
{
    public class PaymentManager
    {
        private BraintreeGateway _gateway;

        public PaymentManager()
        {
            _gateway = new BraintreeGateway
            {
                Environment = Environment.SANDBOX,
                MerchantId = "t5n73yh25jb5sxxf",
                PublicKey = "5srydbsj263fpy7d",
                PrivateKey = "eab6d68d548b38a7fcf052a755b5cf2e"
            };
        }

        

        public string GetClientToken(string customerId)
        {
            Customer cust = FindCustomer(customerId);
            return cust.DefaultPaymentMethod.Token;
        }

        public Customer FindCustomer(string customerId)
        {
            Customer cust = _gateway.Customer.Find(customerId);
            return cust;
        }

        public MerchantAccount FindMerchant(int sitterId)
        {
            try
            {
                var merchant = _gateway.MerchantAccount.Find(sitterId.ToString());
                return merchant;
            }
            catch (Braintree.Exceptions.NotFoundException nfex)
            {
                return null;
            }
        }

        public NewCustomerResultSM CreateCustomer(NewPaymentCustomerSM newCust)
        {
            var request = new CustomerRequest
            {
                Phone = newCust.Phone,
                Id = newCust.CustomerId
            };

            Result<Customer> result = _gateway.Customer.Create(request);

            var ret = new NewCustomerResultSM
            {
                IsSuccess = result.IsSuccess()
            };

            if (ret.IsSuccess)
            {
                ret.NewBraintreeCustomerId = result.Target.Id;
            }

            return ret;
        }

        public UpdatePaymentResultSM CreatePaymentMethod(UpdatePaymentSM sm)
        {
            var updateResult = new UpdatePaymentResultSM();

            // STEP - If customer not found, create
            Customer customer = FindCustomer(sm.Customer.CustomerId);
            if (customer == null)
            {
                NewCustomerResultSM createCustomerResult = CreateCustomer(sm.Customer);
                if (!updateResult.IsSuccess)
                {
                    updateResult.IsSuccess = false;
                    updateResult.Error = string.Format("Customer Not found with id {0}, and update to create. Message: {1}", sm.Customer.CustomerId, createCustomerResult.Error);
                    return updateResult;
                }
            }

            var updateRequest = new PaymentMethodRequest
            {
                Options = new PaymentMethodOptionsRequest
                {
                    MakeDefault = true,
                }
            };

            updateRequest.Number = sm.Number;
            updateRequest.CVV = sm.CVV;
            updateRequest.ExpirationYear = sm.ExpirationYear;
            updateRequest.ExpirationMonth = sm.ExpirationMonth;
            updateRequest.CardholderName = sm.CardholderName;
            updateRequest.CustomerId = sm.Customer.CustomerId;
            updateRequest.PaymentMethodNonce = sm.Nonce;
            updateRequest.Token = sm.Token;

            Result<PaymentMethod> btRet = _gateway.PaymentMethod.Create(updateRequest);

            if (btRet.IsSuccess())
            {
                updateResult.IsSuccess = true;
            }
            else
            {
                updateResult.IsSuccess = false;
                updateResult.Error = btRet.Message;
            }

            return updateResult;
        }
 
        public MakePaymentResultSM MakeSimplePayment(SimplePayment sm)
        {
            var result = new MakePaymentResultSM();

            Customer cust = FindCustomer(sm.ParentId.ToString());
            if (cust == null)
            {
                result.IsSuccess = false;
                result.Error = "Customer not found " + sm.ParentId;
                return result;
            }
            if (cust.DefaultPaymentMethod == null || cust.DefaultPaymentMethod.Token == null)
            {
                result.IsSuccess = false;
                result.Error = "Customer default payment method not found ";
                return result;
            }

            var request = new TransactionRequest
            {
                Amount = sm.Amount,
                PaymentMethodToken = cust.DefaultPaymentMethod.Token,
                CustomerId = sm.ParentId.ToString(),
                Channel = LogicConstants.braintreeChannel,
            };

            Result<Transaction> saleResult = _gateway.Transaction.Sale(request);

            if (saleResult.IsSuccess())
            {
                result.IsSuccess = true;
            }
            else
            {
                if (saleResult.Errors != null && saleResult.Errors.All().Count > 0)
                {
                    result.Error = string.Join("~~", saleResult.Errors.All().Select(m => m.Message));
                }
            }

            return result;
        }

        public MakePaymentResultSM MakeMarketplaceTransaction(MarketPayment sm)
        {
            var result = new MakePaymentResultSM();

            Customer cust = FindCustomer(sm.ParentId.ToString());
            if (cust == null)
            {
                result.IsSuccess = false;
                result.Error = "Customer not found " + sm.ParentId;
                return result;
            }
            if (cust.DefaultPaymentMethod == null || cust.DefaultPaymentMethod.Token == null)
            {
                result.IsSuccess = false;
                result.Error = "Customer default payment method not found ";
                return result;
            }

            var sitter = FindMerchant(sm.SitterId);
            if (sitter == null)
            {
                result.IsSuccess = false;
                result.Error = "Sitter not found " + sm.SitterId;
                return result;
            }
            if (!sitter.IsSubMerchant)
                throw new Exception("sitter is not submerchant");

            var request = new TransactionRequest
            {
                CustomerId = sm.ParentId.ToString(),
                PaymentMethodToken = cust.DefaultPaymentMethod.Token,
                MerchantAccountId = sitter.Id,
                Amount = sm.Amount,
                ServiceFeeAmount = sm.MySitterHubFee,
                Channel = LogicConstants.braintreeChannel,
            };

            Result<Transaction> saleResult = _gateway.Transaction.Sale(request);

            if (saleResult.IsSuccess())
            {
                result.IsSuccess = true;
            }
            else
            {
                if (saleResult.Errors != null && saleResult.Errors.All().Count > 0)
                {
                    result.Error = string.Join("~~", saleResult.Errors.All().Select(m => m.Message));
                }
            }

            return result;
        }

        public bool DeletePaymentMethod(string token)
        {
            _gateway.PaymentMethod.Delete(token);
            return true;
        }

        public bool MarketPlaceTransaction()
        {
            return true;
        }

        public Result<MerchantAccount> CreateSubMerchantAccount(MerchantAccountRequest request)
        {
            Result<MerchantAccount> result = _gateway.MerchantAccount.Create(request);
            return result;
        }

        public Result<MerchantAccount> UpdateSubMerchantAccount(string merchantId, MerchantAccountRequest request)
        {
            Result<MerchantAccount> result = _gateway.MerchantAccount.Update(merchantId, request);
            return result;
        }

    }
}