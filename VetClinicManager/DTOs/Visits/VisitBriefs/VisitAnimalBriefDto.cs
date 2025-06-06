﻿namespace VetClinicManager.DTOs.Visits.VisitBriefs;

public class VisitAnimalBriefDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Species { get; set; }
    public string? Breed { get; set; }
}