namespace VisssStock.Application.DTOs.ProjectDTOs
{
    public class TransactionRequestDto
    {

        public int TypeId { get; set; }

        public int StockId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public int Time { get; set; }
    }

    public class TransactionResponseDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TypeId { get; set; }

        public int StockId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public int Time { get; set; }

        public StockResponseDTO Stock { get; set; } = null!;
    }

    public class TransactionFillterDto
    {
        public int Id { get; set; }

        public int TypeId { get; set; }

        public int StockId { get; set; }

        public int FromQuantity { get; set; }

        public int ToQuantity { get; set; }

        public double FromPrice { get; set; }

        public double ToPrice { get; set; }

        public int FromTime { get; set; }

        public int ToTime { get; set; }
    }
}
