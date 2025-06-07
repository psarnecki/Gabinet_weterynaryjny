using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.Mappers;
using VetClinicManager.Models;

namespace VetClinicManager.Controllers
{
    public class HealthRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AnimalMapper _animalMapper;

        public HealthRecordsController(ApplicationDbContext context, AnimalMapper animalMapper)
        {
            _context = context;
            _animalMapper = animalMapper;
        }

        // GET: HealthRecords
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HealthRecords.Include(h => h.Animal);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HealthRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecord = await _context.HealthRecords
                .Include(h => h.Animal)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (healthRecord == null)
            {
                return NotFound();
            }
            
            var healthRecordDto = _animalMapper.ToHealthRecordEditVetDto(healthRecord);

            return View(healthRecordDto);
        }

        // GET: HealthRecords/Create
        [HttpGet] // Jawnie GET
        [Authorize(Roles = "Admin,Receptionist,Vet")] // Ograniczenie dostępu
        public async Task<IActionResult> Create(int? animalId) // Przyjmuj animalId przekazane z RedirectToHealthRecord
        {
            // Sprawdzamy, czy animalId zostało przekazane (powinno z RedirectToHealthRecord)
            if (animalId == null)
            {
                 // Jeśli ktoś wejdzie bezpośrednio na /HealthRecords/Create bez animalId, przekieruj
                 TempData["ErrorMessage"] = "Wybierz zwierzę, dla którego chcesz utworzyć kartę zdrowia.";
                 return RedirectToAction("Index", "Animals"); // Przekieruj do listy zwierząt
            }

            // Sprawdzenie, czy zwierzę o podanym ID istnieje i czy NIE MA JUŻ karty zdrowia
            // Tymczasowo, używając _context (DO ZMIANY NA SERWIS)
            var existingAnimal = await _context.Animals
                                               .Include(a => a.HealthRecord) // Dołącz kartę zdrowia, żeby sprawdzić czy już istnieje
                                               .FirstOrDefaultAsync(a => a.Id == animalId.Value);

            if (existingAnimal == null)
            {
                 // Zwierzę nie znalezione
                 return NotFound();
            }

            if (existingAnimal.HealthRecord != null)
            {
                 // Zwierzę JUŻ ma kartę zdrowia
                 TempData["WarningMessage"] = $"Zwierzę '{existingAnimal.Name ?? "o podanym ID"}' już posiada kartę zdrowia. Edytuj istniejącą.";
                 // Przekieruj do szczegółów istniejącej karty zdrowia
                 // Zakładamy, że akcja Details w HealthRecordsController przyjmuje ID KARTY ZDROWIA
                 return RedirectToAction(nameof(Details), new { id = existingAnimal.HealthRecord.Id });
            }

            // Przekaż ID zwierzęcia do widoku, żeby formularz mógł je wysłać w POST
            // Również przekaż imię zwierzęcia do wyświetlenia w nagłówku formularza
            ViewBag.AnimalId = animalId.Value;
            ViewBag.AnimalName = existingAnimal.Name; // Dla nagłówka np. "Utwórz Kartę Zdrowia dla [Imię]"


            // Przygotowanie modelu formularza tworzenia
            // Jeśli widok oczekuje encji HealthRecord (jak scaffolding)
            var newHealthRecord = new HealthRecord { AnimalId = animalId.Value }; // Ustaw AnimalId w nowej encji
            return View(newHealthRecord); // Przekaż encje do widoku
            // TODO: Docelowo, użyj DTO formularza tworzenia (np. HealthRecordCreateDto)
            // var model = new HealthRecordCreateDto { AnimalId = animalId.Value }; // Jeśli DTO ma AnimalId
            // return View(model);
        }

        // POST: HealthRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Create([Bind("Id,AnimalId,IsSterilized,ChronicDiseases,Allergies,Vaccinations,LastVaccinationDate")] HealthRecord healthRecord)
        {
            // TODO: Docelowo, akcja POST powinna przyjąć DTO formularza tworzenia
            // public async Task<IActionResult> Create(HealthRecordCreateDto createDto)

            // !!! WAŻNE SPRAWDZENIE BEZPIECZEŃSTWA !!!
            // Sprawdź, czy AnimalId przesłane w formularzu (lub DTO) jest prawidłowe i czy zwierzę o tym ID nie ma już karty zdrowia
            // Bez tego, złośliwy użytkownik mógłby próbować przypisać kartę do innego zwierzęcia lub nadpisać istniejącą
            // Tymczasowo, używając _context (DO ZMIANY NA SERWIS)
            var existingAnimalWithHealthRecord = await _context.Animals
                                                                .Include(a => a.HealthRecord)
                                                                .FirstOrDefaultAsync(a => a.Id == healthRecord.AnimalId); // Sprawdź przesłane AnimalId

            if (existingAnimalWithHealthRecord == null)
            {
                 // Zwierzę o tym ID nie istnieje
                 ModelState.AddModelError("AnimalId", "Wybrane zwierzę nie istnieje.");
            }
            else if (existingAnimalWithHealthRecord.HealthRecord != null)
            {
                 // Zwierzę już ma kartę zdrowia
                 ModelState.AddModelError("AnimalId", "To zwierzę już posiada kartę zdrowia.");
            }
             // Jeśli AnimalId jest w porządku, modelState.IsValid sprawdzi resztę pól

            if (ModelState.IsValid)
            {
                 // TODO: Docelowo, użyj serwisu HealthRecordService do stworzenia karty zdrowia
                 // var result = await _healthRecordService.CreateHealthRecordAsync(createDto);
                 // if (result.Succeeded) { ... } else { ... obsługa błędów serwisu ... }

                 // Logika scaffoldingu (DO ZMIANY NA SERWIS)
                // Upewniamy się, że Id jest 0 dla nowej encji
                healthRecord.Id = 0; // Dodajemy nową, więc ID powinno być generowane przez bazę
                _context.Add(healthRecord);
                await _context.SaveChangesAsync();
                // TODO: Komunikat sukcesu
                // Po sukcesie, przekieruj do szczegółów NOWEJ karty zdrowia
                return RedirectToAction(nameof(Details), new { id = healthRecord.Id });
            }

            // Jeśli walidacja nie przeszła lub błąd serwisu, wróć do widoku z błędem
            // TODO: Docelowo, widok oczekuje DTO formularza tworzenia
            // TODO: Załaduj ponownie dane potrzebne widokowi (np. dropdown AnimalId, jeśli był używany, choć nie powinien być w formularzu tworzenia)
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id", healthRecord.AnimalId); // DO ZMIANY
            return View(healthRecord); // DO ZMIANY
        }

        // GET: HealthRecords/Edit/5
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecord = await _context.HealthRecords.FindAsync(id);
            if (healthRecord == null)
            {
                return NotFound();
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id", healthRecord.AnimalId);
            return View(healthRecord);
        }

        // POST: HealthRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AnimalId,IsSterilized,ChronicDiseases,Allergies,Vaccinations,LastVaccinationDate")] HealthRecord healthRecord)
        {
            if (id != healthRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(healthRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HealthRecordExists(healthRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id", healthRecord.AnimalId);
            return View(healthRecord);
        }

        // GET: HealthRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecord = await _context.HealthRecords
                .Include(h => h.Animal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (healthRecord == null)
            {
                return NotFound();
            }

            return View(healthRecord);
        }

        // POST: HealthRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var healthRecord = await _context.HealthRecords.FindAsync(id);
            if (healthRecord != null)
            {
                _context.HealthRecords.Remove(healthRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HealthRecordExists(int id)
        {
            return _context.HealthRecords.Any(e => e.Id == id);
        }
    }
}
