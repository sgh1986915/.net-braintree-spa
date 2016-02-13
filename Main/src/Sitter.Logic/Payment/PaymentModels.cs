namespace MySitterHub.Logic.Payment
{
    public class BaseResultSM
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
    }

    public class SimplePayment
    {
        public int ParentId { get; set; }
        public decimal Amount { get; set; }
    }

    public class MarketPayment : SimplePayment
    {
        public int SitterId { get; set; }
        public decimal MySitterHubFee { get; set; }
        //public string Nonce { get; set; }
    }

    public class MakePaymentResultSM : BaseResultSM
    {
    }

    public class UpdatePaymentSM
    {
        public NewPaymentCustomerSM Customer { get; set; }
        //public string CustomerId { get; set; }
        
        public string Token { get; set; }
        public string Number { get; set; }
        public string CVV { get; set; }
        public string ExpirationYear { get; set; }
        public string ExpirationMonth { get; set; }
        public string CardholderName { get; set; }
        public string Nonce { get; set; }
    }

    public class UpdatePaymentResultSM : BaseResultSM
    {
        public string Token { get; set; }
    }

    public class NewPaymentCustomerSM
    {
        public string Phone { get; set; }
        public string CustomerId { get; set; }
        public string LastName { get; set; }
    }

    public class NewCustomerResultSM : BaseResultSM
    {
        public string NewBraintreeCustomerId { get; set; }
    }
}