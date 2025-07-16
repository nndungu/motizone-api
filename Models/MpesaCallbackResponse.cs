namespace MotiZone.Models;

public class MpesaCallbackResponse
{
    public class StkCallback {
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public decimal Amount { get; set; }
        public string MpesaReceiptNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string CheckoutRequestID { get; set; }
    }
}