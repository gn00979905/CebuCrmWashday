using CebuCrmApi.Data;
using CebuCrmApi.DTOs;
using CebuCrmApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CebuCrmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public DealsController(CrmDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDeals()
        {
            var deals = await BuildDealsQuery().ToListAsync();
            return Ok(deals);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetDeal(int id)
        {
            var deal = await BuildDealsQuery().FirstOrDefaultAsync(d => d.Id == id);
            if (deal == null)
            {
                return NotFound();
            }

            return Ok(deal);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateDeal([FromBody] CreateDealRequest request)
        {
            var lead = await _context.Clients.FindAsync(request.LeadId);
            var unit = await _context.Units.FindAsync(request.UnitId);

            if (lead == null || unit == null)
            {
                return BadRequest("Lead or Unit not found.");
            }

            var deal = new Deal
            {
                LeadId = request.LeadId,
                UnitId = request.UnitId,
                Stage = string.IsNullOrWhiteSpace(request.Stage) ? "Lead" : request.Stage,
                PriceSnapshot = request.PriceSnapshot ?? unit.Price,
                PaymentPlan = request.PaymentPlan,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();

            _context.DealActivities.Add(new DealActivity
            {
                DealId = deal.Id,
                Type = "Deal Created",
                Date = DateTime.UtcNow,
                Note = $"Linked {lead.Name} to {unit.UnitCode}."
            });

            _context.InvestmentSnapshots.Add(new InvestmentSnapshot
            {
                DealId = deal.Id,
                Roi = request.Roi ?? 0,
                RentalEstimate = request.RentalEstimate ?? 0,
                AirbnbEstimate = request.AirbnbEstimate ?? 0,
                UpdatedAt = DateTime.UtcNow
            });

            if (unit.Status == "Available")
            {
                unit.Status = deal.Stage is "Reservation" or "Contract" or "Closed" ? "Reserved" : unit.Status;
            }

            await _context.SaveChangesAsync();

            var created = await BuildDealsQuery().FirstAsync(d => d.Id == deal.Id);
            return CreatedAtAction(nameof(GetDeal), new { id = deal.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeal(int id, [FromBody] UpdateDealRequest request)
        {
            var deal = await _context.Deals
                .Include(d => d.Unit)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (deal == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(request.Stage))
            {
                deal.Stage = request.Stage;
            }

            if (request.PriceSnapshot.HasValue)
            {
                deal.PriceSnapshot = request.PriceSnapshot.Value;
            }

            deal.PaymentPlan = request.PaymentPlan ?? deal.PaymentPlan;
            deal.Notes = request.Notes ?? deal.Notes;

            if (deal.Unit != null)
            {
                deal.Unit.Status = MapUnitStatusFromDealStage(deal.Stage, deal.Unit.Status);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/payments")]
        public async Task<ActionResult> AddPayment(int id, [FromBody] CreateDealPaymentRequest request)
        {
            if (!await _context.Deals.AnyAsync(d => d.Id == id))
            {
                return NotFound();
            }

            _context.DealPayments.Add(new DealPayment
            {
                DealId = id,
                Type = string.IsNullOrWhiteSpace(request.Type) ? "Installment" : request.Type,
                Amount = request.Amount,
                Date = request.Date ?? DateTime.UtcNow,
                Note = request.Note
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/activities")]
        public async Task<ActionResult> AddActivity(int id, [FromBody] CreateDealActivityRequest request)
        {
            if (!await _context.Deals.AnyAsync(d => d.Id == id))
            {
                return NotFound();
            }

            _context.DealActivities.Add(new DealActivity
            {
                DealId = id,
                Type = string.IsNullOrWhiteSpace(request.Type) ? "Follow-up" : request.Type,
                Date = request.Date ?? DateTime.UtcNow,
                Note = request.Note
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/investment")]
        public async Task<ActionResult> UpdateInvestment(int id, [FromBody] UpdateInvestmentSnapshotRequest request)
        {
            var snapshot = await _context.InvestmentSnapshots.FindAsync(id);
            if (snapshot == null)
            {
                snapshot = new InvestmentSnapshot { DealId = id };
                _context.InvestmentSnapshots.Add(snapshot);
            }

            snapshot.Roi = request.Roi;
            snapshot.RentalEstimate = request.RentalEstimate;
            snapshot.AirbnbEstimate = request.AirbnbEstimate;
            snapshot.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<DealListItem> BuildDealsQuery()
        {
            return _context.Deals
                .Include(d => d.Lead)
                .Include(d => d.Unit)
                    .ThenInclude(u => u!.Project)
                .Include(d => d.Payments)
                .Include(d => d.Activities)
                .Include(d => d.InvestmentSnapshot)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DealListItem
                {
                    Id = d.Id,
                    Stage = d.Stage,
                    PriceSnapshot = d.PriceSnapshot,
                    PaymentPlan = d.PaymentPlan,
                    Notes = d.Notes,
                    CreatedAt = d.CreatedAt,
                    Lead = new DealLeadItem
                    {
                        Id = d.Lead!.Id,
                        Name = d.Lead.Name,
                        Nationality = d.Lead.Nationality,
                        Budget = d.Lead.Budget,
                        Source = d.Lead.Source,
                        Status = d.Lead.Status
                    },
                    Project = new DealProjectItem
                    {
                        Id = d.Unit!.Project!.Id,
                        Name = d.Unit.Project.Name,
                        Developer = d.Unit.Project.Developer,
                        Type = d.Unit.Project.Type,
                        Location = d.Unit.Project.Location
                    },
                    Unit = new DealUnitItem
                    {
                        Id = d.Unit.Id,
                        ProjectId = d.Unit.ProjectId,
                        UnitCode = d.Unit.UnitCode,
                        Price = d.Unit.Price,
                        SizeSqm = d.Unit.SizeSqm,
                        Status = d.Unit.Status,
                        PaymentPlan = d.Unit.PaymentPlan,
                        Discount = d.Unit.Discount,
                        FloorPlanUrl = d.Unit.FloorPlanUrl
                    },
                    Investment = d.InvestmentSnapshot == null
                        ? new DealInvestmentItem()
                        : new DealInvestmentItem
                        {
                            Roi = d.InvestmentSnapshot.Roi,
                            RentalEstimate = d.InvestmentSnapshot.RentalEstimate,
                            AirbnbEstimate = d.InvestmentSnapshot.AirbnbEstimate,
                            UpdatedAt = d.InvestmentSnapshot.UpdatedAt
                        },
                    Payments = d.Payments
                        .OrderByDescending(p => p.Date)
                        .Select(p => new DealPaymentItem
                        {
                            Id = p.Id,
                            Type = p.Type,
                            Amount = p.Amount,
                            Date = p.Date,
                            Note = p.Note
                        })
                        .ToList(),
                    Activities = d.Activities
                        .OrderByDescending(a => a.Date)
                        .Select(a => new DealActivityItem
                        {
                            Id = a.Id,
                            Type = a.Type,
                            Date = a.Date,
                            Note = a.Note
                        })
                        .ToList()
                });
        }

        private static string MapUnitStatusFromDealStage(string stage, string currentStatus)
        {
            return stage switch
            {
                "Reservation" => "Reserved",
                "Contract" => "Reserved",
                "Closed" => "Sold",
                _ => currentStatus == "Sold" ? currentStatus : "Available"
            };
        }

        public class DealListItem
        {
            public int Id { get; set; }
            public string Stage { get; set; } = string.Empty;
            public decimal PriceSnapshot { get; set; }
            public string? PaymentPlan { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
            public DealLeadItem Lead { get; set; } = new();
            public DealProjectItem Project { get; set; } = new();
            public DealUnitItem Unit { get; set; } = new();
            public DealInvestmentItem Investment { get; set; } = new();
            public List<DealPaymentItem> Payments { get; set; } = new();
            public List<DealActivityItem> Activities { get; set; } = new();
        }

        public class DealLeadItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Nationality { get; set; }
            public string? Budget { get; set; }
            public string? Source { get; set; }
            public string? Status { get; set; }
        }

        public class DealProjectItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Developer { get; set; }
            public string? Type { get; set; }
            public string? Location { get; set; }
        }

        public class DealUnitItem
        {
            public int Id { get; set; }
            public int ProjectId { get; set; }
            public string UnitCode { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal SizeSqm { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? PaymentPlan { get; set; }
            public string? Discount { get; set; }
            public string? FloorPlanUrl { get; set; }
        }

        public class DealInvestmentItem
        {
            public decimal Roi { get; set; }
            public decimal RentalEstimate { get; set; }
            public decimal AirbnbEstimate { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DealPaymentItem
        {
            public int Id { get; set; }
            public string Type { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public DateTime Date { get; set; }
            public string? Note { get; set; }
        }

        public class DealActivityItem
        {
            public int Id { get; set; }
            public string Type { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public string? Note { get; set; }
        }
    }
}
