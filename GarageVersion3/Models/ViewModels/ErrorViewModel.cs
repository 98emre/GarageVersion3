namespace GarageVersion3.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        public string ?ErrorMessage {  get; set; }
    }
}
