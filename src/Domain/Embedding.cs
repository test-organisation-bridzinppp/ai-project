﻿namespace Domain
{
    public class Embedding
    {
        public required string FileName { get; init; }
        public int Page { get; init; }
        public string PageContent { get; set; } = string.Empty;
        public IEnumerable<float> Vectors { get; set; } = new List<float>();
    }
}
