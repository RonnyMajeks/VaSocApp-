using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;
using System.Text.Json;
using ClosedXML.Excel;
using System.IO;
using System.Linq;

namespace VaSocApp_System.Pages.StudentAffairs
{
    public class ChartDataItem
    {
        public string label { get; set; } = string.Empty;
        public int value { get; set; }
    }

    public class ReportsModel : PageModel
    {
        private readonly VaSocDbContext _context;
        public ReportsModel(VaSocDbContext context) { _context = context; }

        // Chart Data JSON strings
        public string DiversityChartData { get; set; } = "[]";
        public string YearChartData { get; set; } = "[]";
        public string FacultyChartData { get; set; } = "[]";
        public string SocietyChartData { get; set; } = "[]";

        // Dashboard Metrics
        public int TotalActiveMembers { get; set; }
        public int TotalSocieties { get; set; }
        public int TotalCultures { get; set; }

        public string MostPopularSociety { get; set; } = "N/A";
        public int MostPopularCount { get; set; }
        public string LeastPopularSociety { get; set; } = "N/A";
        public int LeastPopularCount { get; set; }

        public string TopFaculty { get; set; } = "N/A";
        public string TopYear { get; set; } = "N/A";

        public List<string> AiInsights { get; set; } = new List<string>();
        public string? ErrorMessage { get; set; }
        public string? CampusName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterFacultyId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var staff = await _context.StudentAffairs.Include(s => s.Campus)
                .FirstOrDefaultAsync(s => s.PersonId == personId);

            if (staff == null || staff.CampusId == null)
            {
                ErrorMessage = "Your account is not assigned to a campus.";
                return Page();
            }

            CampusName = staff.Campus?.CampusName ?? "Unknown Campus";
            int myCampusId = staff.CampusId.Value;

            var campusSocietyIds = await _context.Societies
                .Where(s => s.CampusId == myCampusId)
                .Select(s => s.SocietyId)
                .ToListAsync();

            TotalSocieties = campusSocietyIds.Count;

            var membershipsQuery = _context.Memberships
                .Where(m => m.Status == "ACCEPTED" && campusSocietyIds.Contains(m.SocietyId ?? 0));

            if (FilterFacultyId.HasValue)
            {
                membershipsQuery = membershipsQuery.Where(m => m.Student != null && m.Student.FacultyId == FilterFacultyId.Value);
            }

            TotalActiveMembers = await membershipsQuery.CountAsync();

            // 1. Cultural Diversity Data (Diversity Reports Requirement)
            var diversityData = await (from m in membershipsQuery
                                       join s in _context.Students on m.StudentId equals s.StudentId
                                       join c in _context.Cultures on s.Cid equals c.Cid into cultureGroup
                                       from c in cultureGroup.DefaultIfEmpty()
                                       group m by (c == null ? "Unknown" : c.CultureName) into g
                                       select new ChartDataItem { label = g.Key, value = g.Count() })
                .OrderByDescending(x => x.value).ToListAsync();
            DiversityChartData = JsonSerializer.Serialize(diversityData);
            TotalCultures = diversityData.Count;

            // 2. Year of Study Data (Tracking Trends Requirement)
            var yearData = await (from m in membershipsQuery
                                  join s in _context.Students on m.StudentId equals s.StudentId
                                  where s.YearOfStudy != null
                                  group m by s.YearOfStudy into g
                                  select new ChartDataItem { label = "Year " + g.Key, value = g.Count() })
                .OrderBy(x => x.label).ToListAsync();
            YearChartData = JsonSerializer.Serialize(yearData);
            TopYear = yearData.OrderByDescending(x => x.value).FirstOrDefault()?.label.Replace("Year ", "") ?? "N/A";

            // 3. Faculty Participation Data
            var facultyData = await (from m in membershipsQuery
                                     join s in _context.Students on m.StudentId equals s.StudentId
                                     join f in _context.Faculties on s.FacultyId equals f.FacultyId into fGroup
                                     from f in fGroup.DefaultIfEmpty()
                                     group m by (f == null ? "Unknown Faculty" : f.FacultyName) into g
                                     select new ChartDataItem { label = g.Key, value = g.Count() })
                .OrderByDescending(x => x.value).ToListAsync();
            FacultyChartData = JsonSerializer.Serialize(facultyData);
            TopFaculty = facultyData.FirstOrDefault()?.label ?? "N/A";

            // 4. Society Popularity Data
            var societyData = await (from m in membershipsQuery
                                     join s in _context.Societies on m.SocietyId equals s.SocietyId into sGroup
                                     from s in sGroup.DefaultIfEmpty()
                                     group m by (s == null ? "Unknown Society" : s.SocietyName) into g
                                     select new ChartDataItem { label = g.Key, value = g.Count() })
                .OrderByDescending(x => x.value).ToListAsync();
            SocietyChartData = JsonSerializer.Serialize(societyData);

            if (societyData.Any())
            {
                MostPopularSociety = societyData.First().label;
                MostPopularCount = societyData.First().value;
                LeastPopularSociety = societyData.Last().label;
                LeastPopularCount = societyData.Last().value;
            }

            // 5. Generate AI Insights & Recommendations (Reports & Analytics)
            GenerateAiInsights(yearData, facultyData, societyData);

            return Page();
        }

        private void GenerateAiInsights(List<ChartDataItem> yearData, List<ChartDataItem> facultyData, List<ChartDataItem> societyData)
        {
            AiInsights.Clear();

            if (societyData.Any())
            {
                AiInsights.Add($"<strong>Society Engagement:</strong> <span class='text-success fw-bold'>{MostPopularSociety}</span> leads with {MostPopularCount} members. <span class='text-danger fw-bold'>{LeastPopularSociety}</span> ({LeastPopularCount} members) needs revitalization campaigns.");
            }

            if (yearData.Any())
            {
                var lowestYear = yearData.OrderBy(x => x.value).First();
                AiInsights.Add($"<strong>Academic Year Targeting:</strong> Lowest participation in <strong>{lowestYear.label}</strong> ({lowestYear.value} members). Launch a '{lowestYear.label} Society Drive' early in the semester.");
            }

            if (facultyData.Count > 1)
            {
                var lowestFaculty = facultyData.Last();
                AiInsights.Add($"<strong>Faculty Outreach:</strong> <strong>{lowestFaculty.label}</strong> is underrepresented. Schedule society showcases in {lowestFaculty.label} buildings.");
            }

            AiInsights.Add($"<strong>Cultural Diversity:</strong> <strong>{TotalCultures}</strong> cultures represented. Recommend 'Cultural Heritage Week' to attract unaffiliated students.");
        }

        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            var personId = HttpContext.Session.GetInt32("PersonId");
            if (personId == null) return RedirectToPage("/Login");

            var staff = await _context.StudentAffairs.Include(s => s.Campus).FirstOrDefaultAsync(s => s.PersonId == personId);
            if (staff == null || staff.CampusId == null) return RedirectToPage("/StudentAffairs/Reports");

            int myCampusId = staff.CampusId.Value;
            var campusSocietyIds = await _context.Societies.Where(s => s.CampusId == myCampusId).Select(s => s.SocietyId).ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                // Sheet 1: Cultural Diversity (Diversity Report)
                var ws1 = workbook.Worksheets.Add("Cultural Diversity");
                ws1.Cell("A1").Value = "Culture";
                ws1.Cell("B1").Value = "Member Count";
                var divData = await (from m in _context.Memberships join s in _context.Students on m.StudentId equals s.StudentId join c in _context.Cultures on s.Cid equals c.Cid into cg from c in cg.DefaultIfEmpty() where m.Status == "ACCEPTED" && campusSocietyIds.Contains(m.SocietyId ?? 0) group m by (c == null ? "Unknown" : c.CultureName) into g select new { Culture = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).ToListAsync();
                for (int i = 0; i < divData.Count; i++) { ws1.Cell(i + 2, 1).Value = divData[i].Culture; ws1.Cell(i + 2, 2).Value = divData[i].Count; }

                // Sheet 2: Faculty Participation
                var ws2 = workbook.Worksheets.Add("Faculty Participation");
                ws2.Cell("A1").Value = "Faculty";
                ws2.Cell("B1").Value = "Active Members";
                var facData = await (from m in _context.Memberships join s in _context.Students on m.StudentId equals s.StudentId join f in _context.Faculties on s.FacultyId equals f.FacultyId into fg from f in fg.DefaultIfEmpty() where m.Status == "ACCEPTED" && campusSocietyIds.Contains(m.SocietyId ?? 0) group m by (f == null ? "Unknown" : f.FacultyName) into g select new { Faculty = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).ToListAsync();
                for (int i = 0; i < facData.Count; i++) { ws2.Cell(i + 2, 1).Value = facData[i].Faculty; ws2.Cell(i + 2, 2).Value = facData[i].Count; }

                // Sheet 3: Society Popularity
                var ws3 = workbook.Worksheets.Add("Society Popularity");
                ws3.Cell("A1").Value = "Society Name";
                ws3.Cell("B1").Value = "Active Members";
                var socData = await (from m in _context.Memberships join s in _context.Societies on m.SocietyId equals s.SocietyId into sg from s in sg.DefaultIfEmpty() where m.Status == "ACCEPTED" && campusSocietyIds.Contains(m.SocietyId ?? 0) group m by (s == null ? "Unknown" : s.SocietyName) into g select new { Society = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).ToListAsync();
                for (int i = 0; i < socData.Count; i++) { ws3.Cell(i + 2, 1).Value = socData[i].Society; ws3.Cell(i + 2, 2).Value = socData[i].Count; }

                // Format all sheets
                foreach (var ws in workbook.Worksheets)
                {
                    ws.Range("A1:B1").Style.Font.Bold = true;
                    ws.Range("A1:B1").Style.Fill.BackgroundColor = XLColor.FromHtml("#6f42c1");
                    ws.Range("A1:B1").Style.Font.FontColor = XLColor.White;
                    ws.Columns().AdjustToContents();
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"VaSocApp_Diversity_Report_{CampusName}_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
        }
    }
}