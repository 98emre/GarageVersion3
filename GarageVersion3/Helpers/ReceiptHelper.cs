using GarageVersion3.Data;
using GarageVersion3.Models;
using GarageVersion3.Models.ViewModels;

namespace GarageVersion3.Helpers
{
    public class ReceiptHelper
    {
        private readonly GarageVersion3Context _context;
        ReceiptHelper(GarageVersion3Context dbContext)
        {
            _context = dbContext;
        }
        Receipt receiptCreator()
        {
            return new Receipt();
        }
        ReceiptViewModel receiptViewModelCreator()
        {
            return new ReceiptViewModel();
        }
    }
}
