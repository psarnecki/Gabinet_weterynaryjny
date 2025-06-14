﻿using VetClinicManager.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinicManager.Models;

public class Visit {
    
    [Key] 
    public int Id { get; set; }
  
    [Required]
    [MaxLength(150)]
    public string Title { get; set; }

    [MaxLength(5000)]
    public string? Description { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public VisitStatus Status { get; set; }
    public VisitPriority Priority { get; set; }

    [Required]
    public int AnimalId { get; set; }
    public Animal Animal { get; set; }
    
    [ForeignKey("AssignedVet")]
    public string? AssignedVetId { get; set; }
    
    public User? AssignedVet { get; set; } // główny lekarz przypisany do wizyty

    public ICollection<VisitUpdate> Updates { get; set; } = new List<VisitUpdate>();
}