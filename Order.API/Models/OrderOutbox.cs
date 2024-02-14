﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Order.API.Models
{
	public class OrderOutbox
	{
		[Key]
        public Guid IdemPotentToken { get; set; }
        public DateTime OccuredOn { get; set; }
		public DateTime? ProcessedDate { get; set; }
		public string Type { get; set; }
		public string Payload { get; set; }

		
	}
}

