﻿namespace ServiceLayer.Models.Publisher
{
    public class PublisherModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? CompanyName { get; set; }
        public string? Description { get; set; }
        public string? HomePage { get; set; }
    }
}
