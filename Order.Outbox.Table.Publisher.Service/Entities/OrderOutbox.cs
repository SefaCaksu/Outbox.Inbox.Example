using System;
namespace Order.Outbox.Table.Publisher.Service.Entities
{
	public class OrderOutbox
	{
        public Guid IdemPotentToken { get; set; }
        public DateTime OccuredOn { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}

